using Microsoft.Extensions.Options;
using TodoApp.Bootstrap;
using TodoApp.Database;
using TodoApp.Endpoints;

namespace TodoApp;

public class Program
{
  public static void Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddSingleton(ctx => 
      new ServiceLogicRoot(
        ctx.GetRequiredService<IOptions<DatabaseOptions>>().Value));
    builder.Services.AddSingleton<IEndpointsRoot>(
      ctx => ctx.GetRequiredService<ServiceLogicRoot>());

    builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection("Database"));

    var app = builder.Build();

    app.MapPost("/todo", async (HttpContext context) =>
    {
      await context.Endpoints().AddTodoEndpoint.Handle(context);
    });

    app.MapGet("/todo/{id}", async (HttpContext context) =>
    {
      await context.Endpoints().RetrieveTodoNoteEndpoint.Handle(context);
    });

    app.Run();
  }
}