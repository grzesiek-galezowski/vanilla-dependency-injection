using ApplicationLogic.Ports;
using NSubstitute;

namespace ApplicationLogicTests.AppLogicTests.Automation;

public class AddTodoNoteResponse(
  IAddTodoResponseInProgress responseInProgress,
  CancellationToken cancellationToken)
{
  public async Task ShouldContain(Guid noteId)
  {
    await responseInProgress.Received(1).Success(
      new TodoNoteMetadataDto(noteId),
      cancellationToken);
  }
}