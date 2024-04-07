using BasicIntegration.Dto;

namespace BasicIntegration;

public class ServiceLogicRoot
{
  private readonly List<WeatherForecastDto> _controllerState = new();
  private readonly ILoggerFactory _loggerFactory;

  public ServiceLogicRoot(ILoggerFactory loggerFactory)
  {
    SaveWeatherForecastEndpoint = new SaveWeatherForecastEndpoint(
      _controllerState,
      loggerFactory.CreateLogger<SaveWeatherForecastEndpoint>());
    RetrieveWeatherForecastEndpoint = new RetrieveWeatherForecastEndpoint(
      _controllerState,
      loggerFactory.CreateLogger<RetrieveWeatherForecastEndpoint>());
    WeatherForecastSignalRApi = new WeatherForecastSignalRApi(
      _controllerState,
      loggerFactory.CreateLogger<WeatherForecastSignalRApi>());
    _loggerFactory = loggerFactory;
  }

  //Controllers
  public WeatherForecastController CreateWeatherForecastController() =>
    new(_loggerFactory.CreateLogger<WeatherForecastController>(),
      _controllerState);

  //Minimal API:
  public IEndpoint RetrieveWeatherForecastEndpoint { get; }
  public IEndpoint SaveWeatherForecastEndpoint { get; }

  //SignalR API:
  public WeatherForecastSignalRApi WeatherForecastSignalRApi { get; }

}