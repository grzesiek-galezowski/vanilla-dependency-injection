using ApplicationLogic.Ports;

namespace TodoApp.Endpoints;

public class AddTodoResponseInProgress(HttpContext context) : IAddTodoResponseInProgress
{
  public async Task Success(TodoNoteMetadataDto todoNoteMetadataDto, CancellationToken cancellationToken)
  {
    await Results.Ok(todoNoteMetadataDto).ExecuteAsync(context);
  }
}