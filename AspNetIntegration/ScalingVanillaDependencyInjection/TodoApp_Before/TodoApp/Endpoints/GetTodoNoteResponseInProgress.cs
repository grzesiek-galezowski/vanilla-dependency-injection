using TodoApp.ApplicationLogic.Ports;

namespace TodoApp.Endpoints;

public class GetTodoNoteResponseInProgress(HttpContext context) : IGetTodoNoteResponseInProgress
{
  public async Task Success(TodoNoteDto note, CancellationToken cancellationToken)
  {
    await Results.Ok(note).ExecuteAsync(context);
  }
}