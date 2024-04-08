using ApplicationLogic.Ports;
using Core.NullableReferenceTypesExtensions;

namespace TodoApp.Endpoints;

public class RetrieveTodoNoteEndpoint(ITodoCommandFactory todoCommandFactory) : IEndpoint
{
  public async Task Handle(HttpContext context)
  {
    //bug there has to be a better way
    Guid id = Guid.Parse(context.GetRouteValue("id").OrThrow().ToString().OrThrow());
    var responseInProgress = new GetTodoNoteResponseInProgress(context);

    var command = todoCommandFactory.CreateRetrieveTodoNoteCommand(id, responseInProgress);
    await command.Execute(context.RequestAborted);
  }
}