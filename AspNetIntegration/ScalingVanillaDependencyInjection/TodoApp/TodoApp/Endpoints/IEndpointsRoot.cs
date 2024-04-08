namespace TodoApp.Endpoints;

public interface IEndpointsRoot
{
  IEndpoint AddTodoEndpoint { get; }
  IEndpoint RetrieveTodoNoteEndpoint { get; }
}