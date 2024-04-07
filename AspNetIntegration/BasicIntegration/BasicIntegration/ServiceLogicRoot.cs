using BasicIntegration.Controllers;
using BasicIntegration.Dto;

namespace BasicIntegration;

public class ServiceLogicRoot
{
  private readonly List<WeatherForecastDto> _controllerState = new();
  private readonly ILoggerFactory _loggerFactory;

  public ServiceLogicRoot(ILoggerFactory loggerFactory)
  {
    SaveWeatherForecastEndpoint = new SaveWeatherForecastEndpoint(_controllerState,
      loggerFactory.CreateLogger<SaveWeatherForecastEndpoint>());
    RetrieveWeatherForecastEndpoint = new RetrieveWeatherForecastEndpoint(_controllerState,
      loggerFactory.CreateLogger<RetrieveWeatherForecastEndpoint>());
    _loggerFactory = loggerFactory;
  }

  public WeatherForecastController CreateWeatherForecastController() =>
    new(_loggerFactory.CreateLogger<WeatherForecastController>(),
      _controllerState);

  public IEndpoint RetrieveWeatherForecastEndpoint { get; set; }
  public IEndpoint SaveWeatherForecastEndpoint { get; init; }
}