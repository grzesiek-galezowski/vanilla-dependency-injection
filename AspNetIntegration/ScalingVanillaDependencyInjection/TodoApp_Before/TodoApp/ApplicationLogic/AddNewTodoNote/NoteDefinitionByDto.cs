using TodoApp.ApplicationLogic.Ports;

namespace TodoApp.ApplicationLogic.AddNewTodoNote;

public interface ITodoNoteDefinition
{
  Task PersistIn(ITodoNoteDao inMemoryTodoNoteDao,
    IAfterTodoNotePersistenceSteps afterPersistenceSteps,
    CancellationToken cancellationToken);

  void Correct();
}

public class NoteDefinitionByDto(
  NewTodoNoteDefinitionDto newTodoNoteDefinitionDto,
  IWordConversion conversion) : ITodoNoteDefinition
{
  public async Task PersistIn(
      ITodoNoteDao inMemoryTodoNoteDao,
      IAfterTodoNotePersistenceSteps afterPersistenceSteps,
      CancellationToken cancellationToken)
  {
    var todoNoteMetadataDto = await inMemoryTodoNoteDao.Save(
      newTodoNoteDefinitionDto,
      cancellationToken);
    await afterPersistenceSteps.ExecuteFor(todoNoteMetadataDto, cancellationToken);
  }

  public void Correct()
  {
    var content = conversion.Apply(newTodoNoteDefinitionDto.Content);

    newTodoNoteDefinitionDto = newTodoNoteDefinitionDto with
    {
      Content = content
    };
  }
}