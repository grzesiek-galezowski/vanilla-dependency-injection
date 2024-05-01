using Microsoft.Extensions.DependencyInjection;
using SimpleInjector;

namespace DiFrameworkPros._1_AutomaticDependencyResolution;

public static class AutomaticDependencyResolution_SimpleInjector
{
  [Test]
  public static void ShouldAutomaticallyResolveBasicDependenciesUsingSimpleInjector()
  {
    var container = new Container();

    container.RegisterSingleton<Person>();
    container.RegisterSingleton<Kitchen>();
    container.RegisterSingleton<Knife>();
    container.RegisterInstancePerDependency<Logger>();
    container.RegisterSingleton<LoggingChannel>();

    container.Verify();

    var person1 = container.GetRequiredService<Person>();
    var person2 = container.GetRequiredService<Person>();

    person1.Should().BeSameAs(person2);
  }
}