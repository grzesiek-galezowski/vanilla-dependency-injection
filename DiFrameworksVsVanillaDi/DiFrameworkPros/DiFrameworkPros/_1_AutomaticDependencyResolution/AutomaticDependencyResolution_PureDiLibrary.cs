using Pure.DI;

namespace DiFrameworkPros._1_AutomaticDependencyResolution;

public class AutomaticDependencyResolution_PureDiLibrary
{
  /// <summary>
  /// The Pure.DI library generates code.
  /// It supports resolution of not registered classes
  /// And named Graph roots.
  ///
  /// Funnily enough, it can still throw exceptions when using the
  /// .Resolve<> call on something that is not a root.
  /// </summary>
  [Test]
  public void ShouldGenerateCodeToWireBasicDependencies()
  {
    var container = new Composition();

    //singletons
    var person1 = container.Resolve<Person>();
    var person2 = container.Johnny;
    person1.Should().BeSameAs(person2);

    //transients
    var logger1 = container.Resolve<Logger>();
    var logger2 = container.NewLogger;
    var logger3 = container.NewLogger;
    logger1.Should().NotBeSameAs(logger2);
    logger2.Should().NotBeSameAs(logger3);
  }
}

partial class Composition 
{
  public void Setup()
  {
    DI.Setup(nameof(Composition))
      .Bind().As(Lifetime.Singleton).To<Person>().Root<Person>("Johnny")
      .Bind().As(Lifetime.Singleton).To<Kitchen>()
      .Bind().As(Lifetime.Singleton).To<Knife>()
      .Bind().As(Lifetime.Singleton).To<LoggingChannel>()
      //.Bind().As(Lifetime.Transient).To<Logger>() //not necessary to register transients
      .Root<Logger>("NewLogger");
  }
}