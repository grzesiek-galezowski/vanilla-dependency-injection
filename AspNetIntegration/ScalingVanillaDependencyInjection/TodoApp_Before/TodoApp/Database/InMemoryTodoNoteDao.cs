using TodoApp.ApplicationLogic.Ports;

namespace TodoApp.Database;

public class InMemoryTodoNoteDao(FileStorage fileStorage, IdGenerator idGenerator, DataConversions dataConversions) : ITodoNoteDao
{
  public async Task<TodoNoteDto> ReadNoteById(Guid noteId, CancellationToken cancellationToken)
  {
    var notes = await ReadTodoNoteDtos(cancellationToken);
    return notes.First(note => note.Id == noteId);
  }

  public async Task<TodoNoteMetadataDto> Save(
    NewTodoNoteDefinitionDto newTodoNoteDefinitionDto,
    CancellationToken cancellationToken)
  {
    var newGuid = idGenerator.Generate();
    var todoNoteDto = dataConversions.ToTodoNoteDtoFrom(newTodoNoteDefinitionDto, newGuid);
    var notes = (await ReadTodoNoteDtos(cancellationToken)).Append(todoNoteDto);
    await fileStorage.Save(notes, cancellationToken);
    return new TodoNoteMetadataDto(newGuid);
  }

  private async Task<TodoNoteDto[]> ReadTodoNoteDtos(CancellationToken cancellationToken)
  {
    return await fileStorage.GetValue<TodoNoteDto[]>([], cancellationToken);
  }
}