using Microsoft.Extensions.DependencyInjection;
using SimpleInjector;
using SimpleInjector.Diagnostics;

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
    RegisterInstancePerDependency<Logger>(container);
    container.RegisterSingleton<LoggingChannel>();

    container.Verify();

    var person1 = container.GetRequiredService<Person>();
    var person2 = container.GetRequiredService<Person>();

    person1.Should().BeSameAs(person2);
  }

  private static void RegisterInstancePerDependency<T>(Container container) where T : class
  {
    container.Register<T>();
    var registration = container.GetRegistration(typeof(T)).Registration;
    registration.SuppressDiagnosticWarning(DiagnosticType.LifestyleMismatch, "instance per dependency lifestyle");
  }
}