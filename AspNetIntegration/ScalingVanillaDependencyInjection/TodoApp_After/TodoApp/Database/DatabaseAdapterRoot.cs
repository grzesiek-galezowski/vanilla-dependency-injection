namespace TodoApp.Database;

public class DatabaseAdapterRoot(string filePath)
{
  public InMemoryTodoNoteDao TodoNoteDao { get; } =
    new InMemoryTodoNoteDao(
      new FileStorage(filePath),
      new IdGenerator(),
      new DataConversions());
}