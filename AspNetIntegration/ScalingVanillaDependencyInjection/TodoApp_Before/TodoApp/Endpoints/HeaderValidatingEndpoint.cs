namespace TodoApp.Endpoints;

public class HeaderValidatingEndpoint(
  string headerName,
  string expectedValue,
  IEndpoint next) : IEndpoint
{
  public async Task Handle(HttpContext context)
  {
    if (!context.Request.Headers[headerName].Any())
    {
      await Results.BadRequest().ExecuteAsync(context);
    }
    else if (context.Request.Headers[headerName].First() != expectedValue)
    //bug test with multiple header values??
    {
      await Results.BadRequest().ExecuteAsync(context);
    }
    else
    {
      await next.Handle(context);
    }
  }
}