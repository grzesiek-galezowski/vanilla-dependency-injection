namespace TodoApp.Database;

public class DatabaseAdapterRoot(string filePath)
{
  public InMemoryTodoNoteDao TodoNoteDao { get; } = new(filePath);
}