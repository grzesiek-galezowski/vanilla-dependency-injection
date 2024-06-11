using Autofac;

namespace DiFrameworkPros._1_Autowiring.Autofac;

public static class _1_ExplicitRegistration
{
  [Test]
  public static void ShouldAutomaticallyResolveBasicDependencies()
  {
    var builder = new ContainerBuilder();
    builder.RegisterType<Person>().SingleInstance();
    builder.RegisterType<Kitchen>().SingleInstance();
    builder.RegisterType<Knife>().SingleInstance();
    builder.RegisterType<Logger>().InstancePerDependency();
    builder.RegisterType<LoggingChannel>().SingleInstance();

    using var container = builder.Build();

    var person1 = container.Resolve<Person>();
    var person2 = container.Resolve<Person>();

    person1.Should().BeSameAs(person2);
  }
}