using Pure.DI;

namespace DiFrameworkCons.CompileTimeVsRuntime.MissingDependency.PureDiLibrary;

public class _1_TypeRegistration
{
  [Test]
  public static void ShouldShowFailureWhenMissingDependencyIsDiscovered()
  {
    //GIVEN
    var composition = new Composition6();

    //WHEN
    //This resolves because Two is bound in the generator,
    //but scroll down and comment out binding for ITwo
    //in Composition6 and compile error will show up.
    var resolvedInstance = composition.One;

    //THEN
    resolvedInstance.Should().BeOfType<One>();
  }
}

partial class Composition6
{
  public void Setup()
  {
    DI.Setup(nameof(Composition6))
      .Bind().As(Lifetime.Transient).To<One>()
      //Comment out the line below to see the compile error
      .Bind<ITwo>().As(Lifetime.Transient).To<Two>()
      .Root<One>("One");
  }
}