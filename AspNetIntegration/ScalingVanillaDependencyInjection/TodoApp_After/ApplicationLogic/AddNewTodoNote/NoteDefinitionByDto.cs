using ApplicationLogic.Ports;

namespace ApplicationLogic.AddNewTodoNote;

public interface ITodoNoteDefinition
{
  Task PersistIn(ITodoNoteDao inMemoryTodoNoteDao,
    IAfterTodoNotePersistenceSteps afterPersistenceSteps,
    CancellationToken cancellationToken);

  void Correct();
}

public class NoteDefinitionByDto(NewTodoNoteDefinitionDto newTodoNoteDefinitionDto, IEnumerable<IWordConversion> conversions) : ITodoNoteDefinition
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
    foreach (var conversion in conversions)
    {
      newTodoNoteDefinitionDto = newTodoNoteDefinitionDto with
      {
        Content = conversion.Apply(newTodoNoteDefinitionDto.Content)
      };
    }
  }
}