using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RaceDay.Api.Data;
using Xunit;

namespace RaceDay.Tests;

public class ApiSmokeTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ApiSmokeTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<RaceDayDbContext>));
                if (descriptor is not null) services.Remove(descriptor);
                services.AddDbContext<RaceDayDbContext>(options => options.UseInMemoryDatabase("RaceDayTestDb"));
            });
        });
    }

    [Fact]
    public async Task Events_ArePubliclyReadable()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/events");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Profile_RequiresLogin()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/users/me");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Registration_RejectsInvalidRole()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/auth/register", new
        {
            fullName = "Test User", email = "invalid-role@example.com",
            password = "Password123!", role = "Admin", phone = "0000000000"
        });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
