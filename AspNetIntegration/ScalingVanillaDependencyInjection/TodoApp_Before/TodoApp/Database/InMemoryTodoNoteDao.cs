using System.Text.Json;
using ApplicationLogic.Ports;
using Core.NullableReferenceTypesExtensions;

namespace TodoApp.Database;

public class InMemoryTodoNoteDao(string filePath) : ITodoNoteDao
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
    var newGuid = Guid.NewGuid();
    var todoNoteDto = new TodoNoteDto(newTodoNoteDefinitionDto.Title, newTodoNoteDefinitionDto.Content, newGuid);
    var notes = (await ReadTodoNoteDtos(cancellationToken)).Append(todoNoteDto);
    await Save(notes, cancellationToken);
    return new TodoNoteMetadataDto(newGuid);
  }

  private async Task Save(IEnumerable<TodoNoteDto> notes, CancellationToken cancellationToken)
  {
    var serializedDto = JsonSerializer.Serialize(notes);
    await File.WriteAllTextAsync(filePath, serializedDto, cancellationToken);
  }

  private async Task<TodoNoteDto[]> ReadTodoNoteDtos(CancellationToken cancellationToken)
  {
    var fileText = await File.ReadAllTextAsync(filePath, cancellationToken);
    if (fileText.Length == 0)
    {
      return [];
    }
    var notes = JsonSerializer.Deserialize<TodoNoteDto[]>(fileText).OrThrow();
    return notes;
  }
}