namespace DiFrameworkPros._1_AutomaticDependencyResolution;

public class AutomaticDependencyResolution_VanillaDi
{
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
  /// we can sometimes do that using a more typical syntax
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