using System.Net.Http.Json;
using BasicIntegration;
using BasicIntegration.Dto;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace Tests;

public class Tests
{
  [Test]
  public async Task ShouldAllowAccessingAController()
  {
    await using var webApp = new WebApplicationFactory<Program>()
      .WithWebHostBuilder(b => b.UseTestServer(o => o.PreserveExecutionContext = true));
    using var httpClient = webApp.CreateClient();

    var input = new WeatherForecastDto(DateOnly.MinValue, 123, "lol");
    await httpClient.PostAsJsonAsync("WeatherForecast", input);
    var response = await httpClient.GetFromJsonAsync<WeatherForecastDto>("WeatherForecast");
    Assert.That(response, Is.EqualTo(input));
  }

  [Test]
  public async Task ShouldAllowAccessingAMinimalApiEndpoint()
  {
    await using var webApp = new WebApplicationFactory<Program>()
      .WithWebHostBuilder(b => b.UseTestServer(o => o.PreserveExecutionContext = true));
    using var httpClient = webApp.CreateClient();

    var input = new WeatherForecastDto(DateOnly.MinValue, 123, "lol");
    await httpClient.PostAsJsonAsync("MinimalWeatherForecast", input);
    var response = await httpClient.GetFromJsonAsync<WeatherForecastDto>("MinimalWeatherForecast");
    Assert.That(response, Is.EqualTo(input));
  }

}