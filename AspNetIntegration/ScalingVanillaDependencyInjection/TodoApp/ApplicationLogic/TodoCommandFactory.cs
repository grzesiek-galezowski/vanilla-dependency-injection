using ApplicationLogic.AddNewTodoNote;
using ApplicationLogic.Ports;

namespace ApplicationLogic;

public class TodoCommandFactory(ITodoNoteDao inMemoryTodoNoteDao) : ITodoCommandFactory
{
  public ITodoAppCommand CreateAddTodoCommand(
    NewTodoNoteDefinitionDto newTodoNoteDefinitionDto,
    IAddTodoResponseInProgress addTodoResponseInProgress)
  {
    return new AddTodoCommand(
      inMemoryTodoNoteDao,
      new NotifyRequesterOnSuccessfulNotePersistence(addTodoResponseInProgress),
      new NoteDefinitionByDto(newTodoNoteDefinitionDto));
  }

  public ITodoAppCommand CreateRetrieveTodoNoteCommand(
    Guid id,
    IGetTodoNoteResponseInProgress responseInProgress)
  {
    return new RetrieveTodoNoteCommand(
      id,
      responseInProgress,
      inMemoryTodoNoteDao);
  }
}