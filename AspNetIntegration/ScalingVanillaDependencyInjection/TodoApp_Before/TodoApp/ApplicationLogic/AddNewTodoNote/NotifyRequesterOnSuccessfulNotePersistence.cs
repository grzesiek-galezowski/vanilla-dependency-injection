using TodoApp.ApplicationLogic.Ports;

namespace TodoApp.ApplicationLogic.AddNewTodoNote;

public interface IAfterTodoNotePersistenceSteps
{
  Task ExecuteFor(
    TodoNoteMetadataDto todoNoteMetadataDto,
    CancellationToken cancellationToken);
}

public class NotifyRequesterOnSuccessfulNotePersistence(IAddTodoResponseInProgress addTodoResponseInProgress)
  : IAfterTodoNotePersistenceSteps
{
  public async Task ExecuteFor(
      TodoNoteMetadataDto todoNoteMetadataDto,
      CancellationToken cancellationToken)
  {
    await addTodoResponseInProgress.Success(todoNoteMetadataDto, cancellationToken);
  }
}