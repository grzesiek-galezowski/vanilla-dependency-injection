using ApplicationLogic.Ports;
using Core.NullableReferenceTypesExtensions;

namespace TodoApp.Endpoints;

public class AddTodoEndpoint(ITodoCommandFactory todoCommandFactory) : IEndpoint
{
  public async Task Handle(HttpContext context)
  {
    var newTodoNoteDefinitionDto =
      (await context.Request.ReadFromJsonAsync<NewTodoNoteDefinitionDto>())
      .OrThrow(); //bug better validation later
    var addTodoResponseInProgress = new AddTodoResponseInProgress(context);

    var addTodoCommand = todoCommandFactory
      .CreateAddTodoCommand(newTodoNoteDefinitionDto, addTodoResponseInProgress);
    await addTodoCommand.Execute(context.RequestAborted);
  }
}