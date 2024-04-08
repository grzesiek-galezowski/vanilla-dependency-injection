namespace TodoApp.ApplicationLogic.AddNewTodoNote;

public class ReplacementConversion(string oldValue, string newValue)
{
  public string Apply(string content)
  {
    return content.Replace(oldValue, newValue);
  }
}