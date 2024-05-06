using Pure.DI;

namespace DiFrameworkCons.CompileTimeVsRuntime.DeadCode;

public class DeadCode_PureDiLibrary
{
  /// <summary>
  /// BUG: fill
  /// </summary>
  [Test]
  public static void ContainerContainsSomeDeadCodeWithMsDi()
  {
    //GIVEN
    var composition = new Composition4();

    //WHEN
    var resolvedInstance = composition.Consumer;

    //THEN
    resolvedInstance.Should().NotBeNull();
  }

}

partial class Composition4
{
  public void Setup()
  {
    DI.Setup(nameof(Composition4))
      .Bind().As(Lifetime.Singleton).To<Dependency>()
      .Bind<DependencyConsumer>()
        .As(Lifetime.Singleton)
        .To<DependencyConsumer>()
        .Root<DependencyConsumer>("Consumer")
      //dead code
      .Bind().As(Lifetime.Singleton).To<DeadCode_VanillaDi>();
  }
}