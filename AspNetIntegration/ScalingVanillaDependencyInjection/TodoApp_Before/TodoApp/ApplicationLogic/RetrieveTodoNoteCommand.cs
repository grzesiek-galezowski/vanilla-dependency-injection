using TodoApp.ApplicationLogic.Ports;

namespace TodoApp.ApplicationLogic;

public class RetrieveTodoNoteCommand(
  Guid id,
  IGetTodoNoteResponseInProgress responseInProgress,
  ITodoNoteDao inMemoryTodoNoteDao)
  : ITodoAppCommand
{
  public async Task Execute(CancellationToken cancellationToken)
  {
    var note = await inMemoryTodoNoteDao.ReadNoteById(id, cancellationToken);
    await responseInProgress.Success(note, cancellationToken);
  }
}