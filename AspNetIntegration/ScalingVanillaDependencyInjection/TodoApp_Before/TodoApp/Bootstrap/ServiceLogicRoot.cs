using Microsoft.Net.Http.Headers;
using TodoApp.ApplicationLogic;
using TodoApp.ApplicationLogic.AddNewTodoNote;
using TodoApp.Database;
using TodoApp.Endpoints;

namespace TodoApp.Bootstrap;

public class ServiceLogicRoot : IEndpointsRoot
{
  public ServiceLogicRoot(DatabaseOptions databaseOptions)
  {
    var todoCommandFactory = new TodoCommandFactory(
      new InMemoryTodoNoteDao(
        new FileStorage(databaseOptions.Path),
        new IdGenerator(),
        new DataConversions()),
      new CompoundConversion(
      [
        new ReplacementConversion("truck", "duck"),
        new ReplacementConversion("dick", "thick"),
        new ReplacementConversion("freaking", "flarking")
      ]));

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