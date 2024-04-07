using BasicIntegration.Dto;

namespace BasicIntegration;

public class RetrieveWeatherForecastEndpoint(
  List<WeatherForecastDto> controllerState,
  ILogger<RetrieveWeatherForecastEndpoint> logger)
  : IEndpoint
{
  public async Task Handle(HttpContext context)
  {
    logger.LogInformation("Get called");
    await Results.Ok(controllerState.Last()).ExecuteAsync(context);
  }
}