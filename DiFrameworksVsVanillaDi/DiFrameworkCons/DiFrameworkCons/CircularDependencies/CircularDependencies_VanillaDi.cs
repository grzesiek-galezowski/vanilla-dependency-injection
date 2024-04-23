namespace DiFrameworkCons.CircularDependencies;

public class CircularDependencies_VanillaDi
{
  /// <summary>
  /// Vanilla Dependency Injection makes circular dependencies very hard to pull off.
  /// You have to really try and ignore modern C# diagnostics like nullable reference types.
  /// </summary>
  [Test]
  public void ShouldShowFailureWhenCircularDependencyIsDiscoveredWithVanillaDI()
  {
    // This will not compile
    // var one = new One(new Two(new Three(one)));

    // This potentially could happen but very unlikely when using nullable reference types as errors + var
    One one = null!;
    var two = new Two(new Three(one));
    one = new One(two);
    one.Two.Three.One.Should().BeNull();
  }

}