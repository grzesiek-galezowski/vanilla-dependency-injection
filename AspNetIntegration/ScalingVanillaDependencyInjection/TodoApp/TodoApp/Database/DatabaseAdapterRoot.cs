namespace TodoApp.Database;

public class DatabaseAdapterRoot
{
    public DatabaseAdapterRoot(string filePath)
    {
        TodoNoteDao = new InMemoryTodoNoteDao(filePath);
    }

    public InMemoryTodoNoteDao TodoNoteDao { get; }
}