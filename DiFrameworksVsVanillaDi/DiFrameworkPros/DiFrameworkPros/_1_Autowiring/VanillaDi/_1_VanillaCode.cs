namespace DiFrameworkPros._1_Autowiring.VanillaDi;

public static class _1_VanillaCode
{
  /// <summary>
  /// This example shows that manual DI is at disadvantage
  /// because of having to repeat the creation of logger,
  /// however...
  /// </summary>
  [Test]
  public static void ShouldManuallyWireBasicDependencies()
  {
    var loggingChannel = new LoggingChannel();
    var person = new Person(
      new Kitchen(
        new Knife(
          new Logger(loggingChannel)),
        new Logger(loggingChannel)),
      new Logger(loggingChannel));
  }
}