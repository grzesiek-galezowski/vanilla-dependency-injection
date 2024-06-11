namespace DiFrameworkPros._1_Autowiring.VanillaDi;

public static class _2_LocalFunctionsForTransients
{
  /// <summary>
  /// ... can use normal C# to our advantages to minimize
  /// the repetition
  /// </summary>
  [Test]
  public static void ShouldManuallyWireBasicDependencies2()
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
}