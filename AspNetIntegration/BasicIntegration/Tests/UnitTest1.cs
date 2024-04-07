using System.Net.Http.Json;
using BasicIntegration;
using BasicIntegration.Dto;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;

namespace Tests;

public class Tests
{
  [Test]
  public async Task ShouldAllowAccessingAController()
  {
    //GIVEN
    await using var webApp = new WebApplicationFactory<Program>()
      .WithWebHostBuilder(b => b.UseTestServer(o => o.PreserveExecutionContext = true));
    using var httpClient = webApp.CreateClient();

    var input = new WeatherForecastDto(DateOnly.MinValue, 123, "lol");
    await httpClient.PostAsJsonAsync("WeatherForecast", input);

    //WHEN
    var response = await httpClient.GetFromJsonAsync<WeatherForecastDto>("WeatherForecast");

    //THEN
    Assert.That(response, Is.EqualTo(input));
  }

  [Test]
  public async Task ShouldAllowAccessingAMinimalApiEndpoint()
  {
    //GIVEN
    await using var webApp = new WebApplicationFactory<Program>()
      .WithWebHostBuilder(b => b.UseTestServer(o => o.PreserveExecutionContext = true));
    using var httpClient = webApp.CreateClient();

    var input = new WeatherForecastDto(DateOnly.MinValue, 123, "lol");
    await httpClient.PostAsJsonAsync("MinimalWeatherForecast", input);

    //WHEN
    var response = await httpClient.GetFromJsonAsync<WeatherForecastDto>("MinimalWeatherForecast");

    //THEN
    Assert.That(response, Is.EqualTo(input));
  }

  [Test]
  public async Task ShouldAllowAccessingASignalRApi()
  {
    //GIVEN
    await using var webApp = new WebApplicationFactory<Program>()
      .WithWebHostBuilder(b => b.UseTestServer(o => o.PreserveExecutionContext = true));

    var connection = new HubConnectionBuilder()
      .WithUrl("https://localhost/WeatherForecastHub", o =>
      {
        o.Transports = HttpTransportType.WebSockets;
        o.SkipNegotiation = true;
        o.HttpMessageHandlerFactory = _ => webApp.Server.CreateHandler();
        o.WebSocketFactory = async (context, cancellationToken) =>
        {
          var wsClient = webApp.Server.CreateWebSocketClient();
          return await wsClient.ConnectAsync(context.Uri, cancellationToken);
        };
      })
      .Build();

    await connection.StartAsync();
    Assert.That(connection.State, Is.EqualTo(HubConnectionState.Connected));

    var input = new WeatherForecastDto(DateOnly.MinValue, 123, "lol");
    await connection.InvokeAsync<WeatherForecastDto>("Save", input);

    //WHEN
    var response = await connection.InvokeAsync<WeatherForecastDto>("Get");

    //THEN
    Assert.That(response, Is.EqualTo(input));
  }
}