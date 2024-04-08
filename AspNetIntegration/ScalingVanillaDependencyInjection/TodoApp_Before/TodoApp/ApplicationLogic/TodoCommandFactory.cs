using System.Collections.ObjectModel;
using TodoApp.ApplicationLogic.AddNewTodoNote;
using TodoApp.ApplicationLogic.Ports;

namespace TodoApp.ApplicationLogic;

public class TodoCommandFactory(
  ITodoNoteDao inMemoryTodoNoteDao,
  List<ReplacementConversion> conversions) : ITodoCommandFactory
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