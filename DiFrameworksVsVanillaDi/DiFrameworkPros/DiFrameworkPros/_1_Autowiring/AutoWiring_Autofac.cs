using Autofac;

namespace DiFrameworkPros._1_Autowiring;

public static class AutoWiring_Autofac
{
  [Test]
  public static void ShouldAutoWireBasicDependenciesUsingAutofac()
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