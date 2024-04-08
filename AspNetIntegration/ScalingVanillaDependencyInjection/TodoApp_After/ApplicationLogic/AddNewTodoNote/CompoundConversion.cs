namespace ApplicationLogic.AddNewTodoNote;

public class CompoundConversion(IEnumerable<IWordConversion> wordConversions) : IWordConversion
{
  public string Apply(string content)
  {
    return wordConversions.Aggregate(content, (current, conversion) => conversion.Apply(current));
  }
}