using BasicIntegration.Dto;
using Core.NullableReferenceTypesExtensions;

namespace BasicIntegration;

public class SaveWeatherForecastEndpoint(
  List<WeatherForecastDto> controllerState,
  ILogger logger) : IEndpoint
{
  public async Task Handle(HttpContext context)
  {
    logger.LogInformation("Post called");
    var dto = (await context.Request.ReadFromJsonAsync<WeatherForecastDto>()).OrThrow();
    controllerState.Add(dto);
    await Results.Ok(dto).ExecuteAsync(context);
  }
}