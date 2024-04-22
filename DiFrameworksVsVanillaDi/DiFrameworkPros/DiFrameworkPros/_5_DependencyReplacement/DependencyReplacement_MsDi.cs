using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using NUnit.Framework;

namespace DiFrameworkPros._5_DependencyReplacement;

public static class DependencyReplacement_MsDi
{
  [Test]
  public static void ShouldBeAbleToOverrideArbitraryDependencyInContainerUsingMsDi()
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
}