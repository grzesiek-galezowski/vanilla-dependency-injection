using ApplicationLogic.Ports;

namespace TodoApp.Database;

public class DataConversions
{
  public TodoNoteDto ToTodoNoteDtoFrom(
    NewTodoNoteDefinitionDto newTodoNoteDefinitionDto, Guid newGuid)
  {
    return new TodoNoteDto(newTodoNoteDefinitionDto.Title, newTodoNoteDefinitionDto.Content, newGuid);
  }
}