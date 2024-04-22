using NSubstitute;
using NUnit.Framework;

namespace DiFrameworkPros._5_DependencyReplacement;

public class DependencyReplacement_VanillaDi
{
  /// <summary>
  /// When doing vanilla dependency injection, we don't have this powerful
  /// mechanism at our disposal, but when we model composition root as an object,
  /// we can use virtual factory methods and provide subclasses that override
  /// these methods to provide test dependencies.
  ///
  /// If we really want to.
  ///
  /// I actually never had to do this IRL btw.
  /// </summary>
  [Test]
  public void ShouldBeAbleToOverrideArbitraryDependency()
  {
    var logicRootForTests = new LogicRootForTests();
    logicRootForTests.GetSomeLogic().Execute();

    logicRootForTests.TroublesomeDependency.Received(1).DoSomething();
  }
}

public class LogicRoot
{
  private readonly SomeLogic _someLogic;

  public LogicRoot()
  {
    _someLogic = new SomeLogic(CreateTroublesomeDependency());
  }

  protected virtual ITroublesomeDependency CreateTroublesomeDependency()
  {
    return new TroublesomeDependency();
  }

  public SomeLogic GetSomeLogic()
  {
    return _someLogic;
  }
}

public class LogicRootForTests : LogicRoot
{
  protected override ITroublesomeDependency CreateTroublesomeDependency()
  {
    return TroublesomeDependency;
  }

  public ITroublesomeDependency TroublesomeDependency { get; }
    = Substitute.For<ITroublesomeDependency>();
}