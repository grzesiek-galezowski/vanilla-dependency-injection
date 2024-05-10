namespace DiFrameworkCons.CompileTimeVsRuntime.MissingDependency;

//todo add descriptions
public class MissingDependency_VanillaDi
{
  [Test]
  public void ShouldShowCompileErrorInCaseOfMissingDependency()
  {
    var one = new One(
      // comment out the line below to see the compile error:
      new Two()
    );
  }
}