using ApplicationLogic.AddNewTodoNote;
using ApplicationLogic.Ports;

namespace ApplicationLogic;

public class TodoCommandFactory(
  ITodoNoteDao inMemoryTodoNoteDao,
  IWordConversion conversion) : ITodoCommandFactory
{
  public ITodoAppCommand CreateAddTodoCommand(
    NewTodoNoteDefinitionDto newTodoNoteDefinitionDto,
    IAddTodoResponseInProgress addTodoResponseInProgress)
  {
    return new AddTodoCommand(
      inMemoryTodoNoteDao,
      new NotifyRequesterOnSuccessfulNotePersistence(addTodoResponseInProgress),
      new NoteDefinitionByDto(newTodoNoteDefinitionDto, conversion));
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