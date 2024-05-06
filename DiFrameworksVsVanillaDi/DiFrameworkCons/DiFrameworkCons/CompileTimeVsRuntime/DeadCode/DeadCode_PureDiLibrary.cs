using Pure.DI;

namespace DiFrameworkCons.CompileTimeVsRuntime.DeadCode;

/// <summary>
/// Even though Pure DI is a code generator,
/// the IDE does not show which parts of the generated code
/// are not used...
/// </summary>
public class DeadCode_PureDiLibrary
{
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
      //dead code
      .Bind().As(Lifetime.Singleton).To<DeadCode_VanillaDi>()
      .Root<Dependency>("Dependency")
      //end dead code;
      .Root<DependencyConsumer>("Consumer");
  }
}