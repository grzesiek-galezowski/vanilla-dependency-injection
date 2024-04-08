using ApplicationLogic.Ports;

namespace ApplicationLogic.AddNewTodoNote;

public interface ITodoNoteDefinition
{
  Task PersistIn(ITodoNoteDao inMemoryTodoNoteDao,
    IAfterTodoNotePersistenceSteps afterPersistenceSteps,
    CancellationToken cancellationToken);

  void Correct();
}

public class NoteDefinitionByDto(NewTodoNoteDefinitionDto newTodoNoteDefinitionDto) : ITodoNoteDefinition
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
    newTodoNoteDefinitionDto = newTodoNoteDefinitionDto with
    {
      Content = newTodoNoteDefinitionDto.Content.Replace("truck", "duck")
    };
  }
}