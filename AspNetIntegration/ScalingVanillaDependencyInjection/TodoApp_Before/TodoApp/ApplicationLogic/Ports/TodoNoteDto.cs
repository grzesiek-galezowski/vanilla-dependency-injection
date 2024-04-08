namespace TodoApp.ApplicationLogic.Ports;

public record TodoNoteDto(string Title, string Content, Guid Id);