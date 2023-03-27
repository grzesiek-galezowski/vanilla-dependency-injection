using Autofac;
using NUnit.Framework;

namespace DiFrameworkPros;

/// <summary>
/// This shows auto wiring
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
  public void ShouldAutoWireBasicDependencies()
  {
    var containerBuilder = new ContainerBuilder();
    containerBuilder.RegisterType<Person>().SingleInstance();
    containerBuilder.RegisterType<Kitchen>().SingleInstance();
    containerBuilder.RegisterType<Knife>().SingleInstance();
    containerBuilder.RegisterType<Logger>().InstancePerDependency();
    containerBuilder.RegisterType<LoggingChannel>().SingleInstance();

    using (var container = containerBuilder.Build())
    {
      var person = container.Resolve<Person>();
    }
  }

  /// <summary>
  /// This example shows that manual DI is at disadvantage
  /// because of having to repeat the creation of logger
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
  /// ... although we can use normal C# to our advantages to minimize
  /// the repetition
  /// </summary>
  [Test]
  public void ShouldManuallyWireBasicDependencies2()
  {
    var loggingChannel = new LoggingChannel(); // this could also be a field
    Logger GetLogger()
    {
      return new Logger(loggingChannel);
    }
    
    var person = new Person(
      new Kitchen(
        new Knife(
          GetLogger()),
        GetLogger()),
      GetLogger());
  }

  /// <summary>
  /// ... if we use a separate class as a Composition Root,
  /// we can do that using a more typical syntax
  /// </summary>
  [Test]
  public void ShouldManuallyWireBasicDependencies3()
  {
    var person = new CompositionRoot().GetPerson();
  }

  private class CompositionRoot
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
}

public class Person
{
  public Person(Kitchen kitchen, Logger logger)
  {
  }
}

public class Kitchen
{
  public Kitchen(Knife knife, Logger logger)
  {
  }
}

public class Knife
{
  public Knife(Logger logger)
  {
  }
}

public class Logger
{
  public Logger(LoggingChannel loggingChannel)
  {
  }
}

public class LoggingChannel
{
}