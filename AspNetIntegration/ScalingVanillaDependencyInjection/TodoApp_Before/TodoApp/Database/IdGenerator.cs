namespace TodoApp.Database;

public class IdGenerator
{
  public Guid Generate()
  {
    return Guid.NewGuid();
  }
}