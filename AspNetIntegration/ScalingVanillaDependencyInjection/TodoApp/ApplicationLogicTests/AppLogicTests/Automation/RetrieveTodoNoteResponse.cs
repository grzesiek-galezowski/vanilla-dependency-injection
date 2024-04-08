using ApplicationLogic.Ports;
using NSubstitute;

namespace ApplicationLogicTests.AppLogicTests.Automation;

public class RetrieveTodoNoteResponse(
  IGetTodoNoteResponseInProgress responseInProgress,
  CancellationToken cancellationToken)
{
  public async Task ShouldContainNoteBasedOn(NewTodoNoteDefinitionDto newTodoNoteDefinitionDto, Guid noteId)
  {
    await responseInProgress.Received(1).Success(new TodoNoteDto(
      newTodoNoteDefinitionDto.Title,
      newTodoNoteDefinitionDto.Content,
      noteId),
      cancellationToken);
  }
}