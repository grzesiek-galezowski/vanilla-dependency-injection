using ApplicationLogic.AddNewTodoNote;
using ApplicationLogic.Ports;

namespace ApplicationLogic;

public class TodoCommandFactory(
  ITodoNoteDao inMemoryTodoNoteDao,
  List<IWordConversion> conversions) : ITodoCommandFactory
{
  public ITodoAppCommand CreateAddTodoCommand(
    NewTodoNoteDefinitionDto newTodoNoteDefinitionDto,
    IAddTodoResponseInProgress addTodoResponseInProgress)
  {
    return new AddTodoCommand(
      inMemoryTodoNoteDao,
      new NotifyRequesterOnSuccessfulNotePersistence(addTodoResponseInProgress),
      new NoteDefinitionByDto(newTodoNoteDefinitionDto, conversions));
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