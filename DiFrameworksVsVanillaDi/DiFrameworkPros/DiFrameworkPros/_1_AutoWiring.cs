using Autofac;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace DiFrameworkPros;

/// <summary>
/// This example shows auto wiring
/// - get an instance without invoking the constructor.
/// - introduce Knife dependency to Person - no need to touch composition root
///
/// Auto-wiring is optimized for reusing registrations.
/// If a new registration depends only on existing registration,
/// there is nothing else to do besides adding this one new registration.
/// </summary>
public class AutoWiring
{
  [Test]
  public void ShouldAutoWireBasicDependenciesUsingAutofac()
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

  [Test]
  public void ShouldAutoWireBasicDependenciesUsingMsDi()
  {
    var builder = new ServiceCollection();
    builder.AddSingleton<Person>();
    builder.AddSingleton<Kitchen>();
    builder.AddSingleton<Knife>();
    builder.AddTransient<Logger>();
    builder.AddSingleton<LoggingChannel>();

    using var container = builder.BuildServiceProvider();

    var person = container.GetRequiredService<Person>();
  }

  /// <summary>
  /// This example shows that manual DI is at disadvantage
  /// because of having to repeat the creation of logger,
  /// however...
  /// </summary>
  [Test]
  public void ShouldManuallyWireBasicDependencies()
  {
    var loggingChannel = new LoggingChannel();
    var person = new Person(
      new Kitchen(
        new Knife(
          new Logger(loggingChannel)),
        new Logger(loggingChannel)),
      new Logger(loggingChannel));
  }
  
  /// <summary>
  /// ... can use normal C# to our advantages to minimize
  /// the repetition
  /// </summary>
  [Test]
  public void ShouldManuallyWireBasicDependencies2()
  {
    var loggingChannel = new LoggingChannel(); // this could also be a field

    var person = new Person(
      new Kitchen(
        new Knife(
          GetLogger()),
        GetLogger()),
      GetLogger());
    return;

    Logger GetLogger()
    {
      return new Logger(loggingChannel);
    }
  }

  /// <summary>
  /// ... if we use a separate class as a Composition Root,
  /// we can do that using a more typical syntax
  /// (see inside the CompositionRoot class)
  /// </summary>
  [Test]
  public void ShouldManuallyWireBasicDependencies3()
  {
    var person = new CompositionRoot().GetPerson();
  }

}

file class CompositionRoot
{
  private readonly LoggingChannel _loggingChannel;

  public CompositionRoot()
  {
    _loggingChannel = new LoggingChannel();
  }
    
  private Logger GetLogger()
  {
    return new Logger(_loggingChannel);
  }

  public Person GetPerson()
  {
    return new Person(
      new Kitchen(
        new Knife(
          GetLogger()),
        GetLogger()),
      GetLogger());
  }
}


file class Person(Kitchen kitchen, Logger logger);
file class Kitchen(Knife knife, Logger logger);
file class Knife(Logger logger);

file class Logger(LoggingChannel loggingChannel);

file class LoggingChannel;
