using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using NUnit.Framework;

namespace DiFrameworkPros;

public class DependencyReplacement
{
  /// <summary>
  /// DI Containers allow replacing registrations in already filled container
  /// builder. This can be useful for tests, where we can replace arbitrary
  /// production dependency with one made specifically for tests.
  ///
  /// Most of the time this implies bad design, but can be useful sometimes
  /// when dealing with frameworks like asp.net core. 
  /// </summary>
  [Test]
  public void ShouldBeAbleToOverrideArbitraryDependencyInContainerUsingAutofac()
  {
    var builder = new ContainerBuilder();

    builder.RegisterType<SomeLogic>().As<ISomeLogic>().SingleInstance();
    builder.RegisterType<TroublesomeDependency>().As<ITroublesomeDependency>().SingleInstance();

    var troublesomeDependencyMock = Substitute.For<ITroublesomeDependency>();
    builder.Register(_ => troublesomeDependencyMock);

    using var container = builder.Build();

    container.Resolve<ISomeLogic>().Execute();

    troublesomeDependencyMock.Received(1).DoSomething();
  }

  [Test]
  public void ShouldBeAbleToOverrideArbitraryDependencyInContainerUsingMsDi()
  {
    var builder = new ServiceCollection();

    builder.AddSingleton<ISomeLogic, SomeLogic>();
    builder.AddSingleton<ITroublesomeDependency, TroublesomeDependency>();

    var troublesomeDependencyMock = Substitute.For<ITroublesomeDependency>();
    builder.Replace(new ServiceDescriptor(typeof(ITroublesomeDependency), _ => troublesomeDependencyMock, ServiceLifetime.Singleton));

    using var container = builder.BuildServiceProvider();

    container.GetRequiredService<ISomeLogic>().Execute();

    troublesomeDependencyMock.Received(1).DoSomething();
  }

  /// <summary>
  /// When doing vanilla dependency injection, we don't have this powerful
  /// mechanism at our disposal, but when we model composition root as an object,
  /// we can use virtual factory methods and provide subclasses that override
  /// these methods to provide test dependencies.
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
}