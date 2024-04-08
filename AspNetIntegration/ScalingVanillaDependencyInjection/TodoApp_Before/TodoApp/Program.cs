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

    var endpointsRoot = app.Services.GetRequiredService<IEndpointsRoot>();

    app.MapPost("/todo", endpointsRoot.AddTodoEndpoint.Handle);
    app.MapGet("/todo/{id}", endpointsRoot.RetrieveTodoNoteEndpoint.Handle);

    app.Run();
  }
}