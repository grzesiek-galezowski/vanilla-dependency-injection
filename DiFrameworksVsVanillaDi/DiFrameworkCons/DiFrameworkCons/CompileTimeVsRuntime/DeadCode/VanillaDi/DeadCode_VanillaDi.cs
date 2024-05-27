namespace DiFrameworkCons.CompileTimeVsRuntime.DeadCode.VanillaDi;

public class DeadCode_VanillaDi
{
  /// <summary>
  /// "Dead" dependencies can be clearly visible in the imperative code when
  /// doing Vanilla DI - below, the `deadCode` will be marked by the IDE as unused.
  /// </summary>
  [Test]
  public void VanillaDiContainsDeadCode()
  {
    //GIVEN
    var consumer = new DependencyConsumer(new Dependency());
    var deadCode = new DeadCode_VanillaDi();

    //WHEN

    //THEN
    consumer.Should().NotBeNull();
  }
}