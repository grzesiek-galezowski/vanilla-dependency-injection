using SimpleInjector;
using SimpleInjector.Diagnostics;

namespace DiFrameworkPros._1_AutomaticDependencyResolution;

public static class SimpleInjectorExtensions
{
  public static void RegisterInstancePerDependency<T>(this Container container) where T : class
  {
    container.Register<T>();
    var registration = container.GetRegistration(typeof(T)).Registration;
    registration.SuppressDiagnosticWarning(DiagnosticType.LifestyleMismatch, "instance per dependency lifestyle");
  }
}