using BasicIntegration.Dto;
using Microsoft.AspNetCore.Mvc;

namespace BasicIntegration.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController(
  ILogger<WeatherForecastController> logger,
  ICollection<WeatherForecastDto> controllerState) : ControllerBase
{
  [HttpGet]
  public async Task<WeatherForecastDto> Get()
  {
    logger.LogInformation("Get called");
    return controllerState.Last();
  }

  [HttpPost]
  public async Task<WeatherForecastDto> Save(WeatherForecastDto dto)
  {
    logger.LogInformation("Post called");
    controllerState.Add(dto);
    return dto;
  }
}