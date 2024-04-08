namespace TodoApp.Endpoints;

public interface IEndpoint
{
  Task Handle(HttpContext context);
}