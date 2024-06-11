using DiFrameworkPros.HelperCode;
using Microsoft.Extensions.DependencyInjection;
using SimpleInjector;

namespace DiFrameworkPros._1_Autowiring.SimpleInjector;

public static class _2_ResolvingUnregisteredTypes
{
  [Test]
  public static void ShouldAutomaticallyResolveBasicDependenciesUsingSimpleInjector()
  {
    using var container = new Container();

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