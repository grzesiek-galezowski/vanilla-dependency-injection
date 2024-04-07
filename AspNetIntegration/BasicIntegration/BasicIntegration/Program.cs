using BasicIntegration.Dto;

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
    b.Services.AddSwaggerGen();

    var app = b.Build();

    app.Services.GetRequiredService<ServiceLogicRoot>(); //eager initialization

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
      app.UseSwagger();
      app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();
    app.MapPost("MinimalWeatherForecast", async (HttpContext context, ServiceLogicRoot serviceLogicRoot) =>
    {
      await serviceLogicRoot.SaveWeatherForecastEndpoint.Handle(context);
    });

    app.MapGet("MinimalWeatherForecast", async (HttpContext context, ServiceLogicRoot serviceLogicRoot) =>
    {
      await serviceLogicRoot.RetrieveWeatherForecastEndpoint.Handle(context);
    });

    app.Run();
  }
}