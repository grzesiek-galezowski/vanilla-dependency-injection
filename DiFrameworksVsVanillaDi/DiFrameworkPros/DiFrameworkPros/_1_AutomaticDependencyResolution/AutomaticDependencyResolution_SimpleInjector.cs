using DiFrameworkPros.HelperCode;
using Microsoft.Extensions.DependencyInjection;
using SimpleInjector;

namespace DiFrameworkPros._1_AutomaticDependencyResolution;

public static class AutomaticDependencyResolution_SimpleInjector
{
  /// <summary>
  /// SimpleInjector can automatically find the required types and create
  /// its instances, as long as those types are concrete.
  /// We can change the default lifestyle to e.g. make the instances singletons,
  /// but that impacts the whole container.
  ///
  /// Unregistered concrete types do not take full part in container
  /// configuration verification.
  ///
  /// Also see
  /// https://docs.simpleinjector.org/en/latest/resolving-unregistered-concrete-types-is-disallowed-by-default.html#what-s-the-problem 
  /// </summary>
  [Test]
  public static void ShouldAutomaticallyResolvePublicTransientDependenciesUsingSimpleInjector()
  {
    var container = new Container();
    container.Options.ResolveUnregisteredConcreteTypes = true;
    container.Options.DefaultLifestyle = Lifestyle.Singleton;

    container.RegisterInstancePerDependency<Logger>();

    container.Verify();

    var person1 = container.GetRequiredService<Person>();
    var person2 = container.GetRequiredService<Person>();

    person1.Should().BeSameAs(person2);
  }

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