namespace BasicIntegration;

public class Program
{
  public static void Main(string[] args)
  {
    var b = WebApplication.CreateBuilder(args);

    // Add services to the container.

    b.Services.AddSingleton<ServiceLogicRoot>();
    b.Services.AddTransient(c => c.GetRequiredService<ServiceLogicRoot>().CreateWeatherForecastController());

    b.Services.AddControllers().AddControllersAsServices();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    b.Services.AddEndpointsApiExplorer();
    b.Services.AddSignalR();
    b.Services.AddSwaggerGen();

    var app = b.Build();

    var root = app.Services.GetRequiredService<ServiceLogicRoot>(); //eager initialization

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
      app.UseSwagger();
      app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();
    app.MapPost("MinimalWeatherForecast", root.SaveWeatherForecastEndpoint.Handle);
    app.MapGet("MinimalWeatherForecast", root.RetrieveWeatherForecastEndpoint.Handle);
    app.MapHub<WeatherForecastHub>("/WeatherForecastHub");

    app.Run();
  }
}