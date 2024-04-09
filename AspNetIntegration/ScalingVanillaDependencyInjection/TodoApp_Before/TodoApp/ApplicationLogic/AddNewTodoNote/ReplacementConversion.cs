namespace TodoApp.ApplicationLogic.AddNewTodoNote;

public interface IWordConversion
{
  string Apply(string content);
}

public class ReplacementConversion(string oldValue, string newValue) : IWordConversion
{
  public string Apply(string content)
  {
    return content.Replace(oldValue, newValue);
  }
}