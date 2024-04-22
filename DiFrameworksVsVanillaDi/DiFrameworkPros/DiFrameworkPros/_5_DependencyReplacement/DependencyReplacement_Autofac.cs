using Autofac;

namespace DiFrameworkPros._5_DependencyReplacement;

public static class DependencyReplacement_Autofac
{
  /// <summary>
  /// DI Containers allow replacing registrations in already filled container
  /// builder. This can be useful for tests, where we can replace arbitrary
  /// production dependency with one made specifically for tests.
  ///
  /// Most of the time this implies bad design, but can be useful sometimes
  /// when dealing with frameworks like Asp.NET Core. 
  /// </summary>
  [Test]
  public static void ShouldBeAbleToOverrideArbitraryDependencyInContainerUsingAutofac()
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
}