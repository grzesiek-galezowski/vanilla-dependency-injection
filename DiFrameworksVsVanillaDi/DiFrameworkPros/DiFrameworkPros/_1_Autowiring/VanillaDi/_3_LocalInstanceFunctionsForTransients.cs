namespace DiFrameworkPros._1_Autowiring.VanillaDi;

public class _3_LocalInstanceFunctionsForTransients
{
  /// <summary>
  /// ... if we use a separate class as a Composition Root,
  /// we can sometimes do that using a more typical syntax
  /// (see inside the CompositionRoot class)
  /// </summary>
  [Test]
  public static void ShouldManuallyWireBasicDependencies3()
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