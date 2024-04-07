using BasicIntegration.Dto;
using Microsoft.AspNetCore.SignalR;

namespace BasicIntegration;

public class WeatherForecastHub(ServiceLogicRoot serviceLogicRoot) : Hub
{
  public async Task<WeatherForecastDto> Save(WeatherForecastDto dto)
  {
    await serviceLogicRoot.WeatherForecastSignalRApi.Save(Context, dto);
    return dto;
  }

  public async Task<WeatherForecastDto> Get()
  {
    return await serviceLogicRoot.WeatherForecastSignalRApi.Get(Context);
  }
}