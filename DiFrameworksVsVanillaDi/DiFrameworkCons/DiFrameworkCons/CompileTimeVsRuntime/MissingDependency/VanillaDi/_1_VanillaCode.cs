namespace DiFrameworkCons.CompileTimeVsRuntime.MissingDependency.VanillaDi;

//todo add descriptions
public class _1_VanillaCode
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