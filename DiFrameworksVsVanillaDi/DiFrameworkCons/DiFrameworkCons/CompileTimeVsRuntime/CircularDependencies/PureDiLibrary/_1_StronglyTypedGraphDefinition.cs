using Pure.DI;

namespace DiFrameworkCons.CompileTimeVsRuntime.CircularDependencies.PureDiLibrary;

public static class _1_StronglyTypedGraphDefinition
{
  /// <summary>
  /// Pure.DI impressively shows the circular dependency at
  /// the time of compilation. Just uncomment the commented
  /// out line in the <see cref="Composition17"/> to see the compile error.
  /// </summary>
  [Test]
  public static void ShouldShowFailureWhenCircularDependencyIsDiscoveredWithPureDiLibrary()
  {
    var composition = new Composition17();
  }
}

partial class Composition17
{
  public void Setup()
  {
    DI.Setup(nameof(Composition17))
      //uncomment to show compile error DIE003 Cyclic dependency has been found
      //.Root<One>()
      ;

  }
}