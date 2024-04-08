using TodoApp.Endpoints;

namespace TodoApp;

internal static class DiContainerExtensions
{
  public static IEndpointsRoot Endpoints(this HttpContext context)
  {
    return context.RequestServices.GetRequiredService<IEndpointsRoot>();
  }
}