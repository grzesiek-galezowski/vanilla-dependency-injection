using ApplicationLogic.Ports;
using Microsoft.Net.Http.Headers;

namespace TodoApp.Endpoints;

public class EndpointsAdapterRoot(ITodoCommandFactory todoCommandFactory) : IEndpointsRoot
{
  public IEndpoint RetrieveTodoNoteEndpoint { get; } = new RetrieveTodoNoteEndpoint(todoCommandFactory);
  public IEndpoint AddTodoEndpoint { get; } = new HeaderValidatingEndpoint(HeaderNames.Accept, "application/json",
    new AddTodoEndpoint(todoCommandFactory));
}