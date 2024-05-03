namespace DiFrameworkPros._4_AssemblyScanning;

public class AssemblyScanning_VanillaDi
{
  /// <summary>
  /// Vanilla dependency injection doesn't have any alternative.
  /// The whole point of Vanilla DI is to create the dependencies manually
  /// instead of using reflection.
  /// </summary>
  [Test]
  public void ShouldShowDependencyGraphAssembledWithoutAssemblyScanning()
  {
    var myRepository = new MyRepository();
    Interface1 i1 = myRepository;
    Interface2 i2 = myRepository;
  }
}