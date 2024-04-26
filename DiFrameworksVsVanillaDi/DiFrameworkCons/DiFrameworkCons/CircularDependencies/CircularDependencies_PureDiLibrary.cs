using Pure.DI;

namespace DiFrameworkCons.CircularDependencies;

public static class CircularDependencies_PureDiLibrary
{
  /// <summary>
  /// Pure.DI impressively shows the circular dependency at
  /// the time of compilation. Just uncomment the commented
  /// out line to see the compile error.
  /// </summary>
  [Test]
  public static void ShouldShowFailureWhenCircularDependencyIsDiscoveredWithPureDiLibrary()
  {
    DI.Setup($"{nameof(CircularDependencies_PureDiLibrary)}CompositionRoot")
      //uncomment to show compile error: .Root<One>()
      ;

    _ = new CircularDependencies_PureDiLibraryCompositionRoot();
  }
}