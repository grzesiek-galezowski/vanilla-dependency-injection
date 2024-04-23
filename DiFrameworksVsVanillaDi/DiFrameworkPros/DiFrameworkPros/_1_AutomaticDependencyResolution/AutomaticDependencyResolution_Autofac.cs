using Autofac;

namespace DiFrameworkPros._1_AutomaticDependencyResolution;

public static class AutomaticDependencyResolution_Autofac
{
  [Test]
  public static void ShouldAutomaticallyResolveBasicDependenciesUsingAutofac()
  {
    var builder = new ContainerBuilder();
    builder.RegisterType<Person>().SingleInstance();
    builder.RegisterType<Kitchen>().SingleInstance();
    builder.RegisterType<Knife>().SingleInstance();
    builder.RegisterType<Logger>().InstancePerDependency();
    builder.RegisterType<LoggingChannel>().SingleInstance();

    using var container = builder.Build();

    var person = container.Resolve<Person>();
  }
}