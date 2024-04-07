namespace BasicIntegration;

public interface IEndpoint
{
  Task Handle(HttpContext context);
}