using BasicIntegration.Dto;
using Microsoft.AspNetCore.SignalR;

namespace BasicIntegration;

public class WeatherForecastSignalRApi(List<WeatherForecastDto> controllerState, ILogger logger)
{
  public async Task Save(HubCallerContext context, WeatherForecastDto dto)
  {
    logger.LogInformation("Post called");
    controllerState.Add(dto);
  }

  public async Task<WeatherForecastDto> Get(HubCallerContext context)
  {
    logger.LogInformation("Get called");
    return controllerState.Last();
  }
}