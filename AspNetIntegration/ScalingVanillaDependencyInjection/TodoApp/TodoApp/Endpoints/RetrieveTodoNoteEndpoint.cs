using ApplicationLogic.Ports;
using Core.NullableReferenceTypesExtensions;

namespace TodoApp.Endpoints;

public class RetrieveTodoNoteEndpoint : IEndpoint
{
    private readonly ITodoCommandFactory _todoCommandFactory;

    public RetrieveTodoNoteEndpoint(ITodoCommandFactory todoCommandFactory)
    {
        _todoCommandFactory = todoCommandFactory;
    }

    public async Task Handle(HttpContext context)
    {
        //bug there has to be a better way
        Guid id = Guid.Parse(context.GetRouteValue("id").OrThrow().ToString().OrThrow());
        var responseInProgress = new GetTodoNoteResponseInProgress(context);

        var command = _todoCommandFactory.CreateRetrieveTodoNoteCommand(id, responseInProgress);
        await command.Execute(context.RequestAborted);
    }
}