namespace DiFrameworkCons.MultipleRecipes.Decorators._1_Simple;

public interface IAnswer
{
  IAnswer NestedAnswer { get; }
}

public record TracedAnswer(IAnswer NestedAnswer) : IAnswer;
public record SynchronizedAnswer(IAnswer NestedAnswer, int X) : IAnswer;
public record Answer : IAnswer
{
  public IAnswer NestedAnswer => null!;
}