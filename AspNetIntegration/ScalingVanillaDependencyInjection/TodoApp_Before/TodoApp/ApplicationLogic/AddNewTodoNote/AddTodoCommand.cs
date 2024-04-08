using TodoApp.ApplicationLogic.Ports;

namespace TodoApp.ApplicationLogic.AddNewTodoNote;

public class AddTodoCommand(
  ITodoNoteDao inMemoryTodoNoteDao,
  IAfterTodoNotePersistenceSteps afterPersistenceSteps,
  ITodoNoteDefinition todoNoteDefinition)
  : ITodoAppCommand
{
  public async Task Execute(CancellationToken cancellationToken)
  {
    todoNoteDefinition.Correct();
    await todoNoteDefinition.PersistIn(
      inMemoryTodoNoteDao,
      afterPersistenceSteps,
      cancellationToken);
  }
}