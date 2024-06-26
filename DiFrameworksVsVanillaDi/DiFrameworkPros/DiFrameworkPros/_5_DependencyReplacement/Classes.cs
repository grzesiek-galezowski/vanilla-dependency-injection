namespace DiFrameworkPros._5_DependencyReplacement;

public interface ITroublesomeDependency
{
  void DoSomething();
}

public class TroublesomeDependency : ITroublesomeDependency
{
  public void DoSomething()
  {
    throw new NotImplementedException();
  }
}

public interface ISomeLogic
{
  void Execute();
}

public class SomeLogic : ISomeLogic
{
  private readonly ITroublesomeDependency _dep;

  public SomeLogic(ITroublesomeDependency dep)
  {
    _dep = dep;
  }

  public void Execute()
  {
    _dep.DoSomething();
  }
}