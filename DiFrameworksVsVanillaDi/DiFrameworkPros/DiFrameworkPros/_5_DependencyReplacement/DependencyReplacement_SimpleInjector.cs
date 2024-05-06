using Microsoft.Extensions.DependencyInjection;
using SimpleInjector;

namespace DiFrameworkPros._5_DependencyReplacement;

public static class DependencyReplacement_SimpleInjector
{
  /// <summary>
  /// SimpleInjector disallows replacing registrations unless
  /// it is explicitly turned on with AllowOverridingRegistrations.
  /// This check is handy as it prevents some of the errors that
  /// could be caused by code merge accidents. 
  ///
  /// One can only enable the option near the point where
  /// the replacement is needed and then instantly disable it again.
  /// </summary>
  [Test]
  public static void ShouldBeAbleToOverrideArbitraryDependencyInContainerUsingSimpleInjector()
  {
    using var container = new Container();
    container.RegisterSingleton<ISomeLogic, SomeLogic>();
    container.RegisterSingleton<ITroublesomeDependency, TroublesomeDependency>();

    var troublesomeDependencyMock = Substitute.For<ITroublesomeDependency>();

    container.Options.AllowOverridingRegistrations = true;
    container.RegisterInstance(troublesomeDependencyMock);
    container.Options.AllowOverridingRegistrations = false;

    container.GetRequiredService<ISomeLogic>().Execute();

    troublesomeDependencyMock.Received(1).DoSomething();
  }
}