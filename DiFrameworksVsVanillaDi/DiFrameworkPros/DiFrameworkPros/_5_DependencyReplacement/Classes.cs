public interface ITroublesomeDependency
{
  void DoSomething();
}

public class TroublesomeDependency : ITroublesomeDependency
{
  public void DoSomething()
  {
    throw new System.NotImplementedException();
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