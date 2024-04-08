using ApplicationLogic;
using Microsoft.Net.Http.Headers;
using TodoApp.Database;
using TodoApp.Endpoints;

namespace TodoApp.Bootstrap;

public class ServiceLogicRoot : IEndpointsRoot
{
  public ServiceLogicRoot(DatabaseOptions databaseOptions)
  {
    var todoCommandFactory = new TodoCommandFactory(
      new InMemoryTodoNoteDao(databaseOptions.Path));

    AddTodoEndpoint = new HeaderValidatingEndpoint(
      HeaderNames.Accept,
      "application/json",
      new AddTodoEndpoint(todoCommandFactory));

    RetrieveTodoNoteEndpoint =
      new RetrieveTodoNoteEndpoint(todoCommandFactory);

  }

  public IEndpoint AddTodoEndpoint { get; }

  public IEndpoint RetrieveTodoNoteEndpoint { get; }
}