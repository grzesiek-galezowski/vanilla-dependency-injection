using SimpleInjector;

namespace DiFrameworkCons.CompileTimeVsRuntime.CircularDependencies;

public static class CircularDependencies_SimpleInjector
{
  /// <summary>
  /// Simple injector has a Verify method that among others, tries to resolve
  /// all rules, and can thus detect misconfigurations.
  ///
  /// If this method is not used, then circular dependencies cause exceptions
  /// at dependency resolution time (though unlikely someone would not use Verify).
  /// </summary>
  [Test]
  public static void ShouldShowFailureWhenCircularDependencyIsDiscoveredWithSimpleInjector()
  {
    //GIVEN
    using var container = new Container();
    container.Register<One>(Lifestyle.Transient);
    container.Register(() => new Two(container.GetInstance<Three>()));
    container.Register<Three>();

    Invoking(() => container.Verify(VerificationOption.VerifyAndDiagnose))
      .Should().Throw<InvalidOperationException>();

    //WHEN
    //THEN
    Invoking(container.GetInstance<One>).Should().Throw<ActivationException>();
  }


  /// <summary>
  /// In case the cycle is not referenced by any registered type,
  /// the ResolveUnregisteredConcreteTypes option leads to verification
  /// passing despite there being a cycle.
  /// </summary>
  [Test]
  public static void ShouldShowFailureWhenResolvingCircularDependency()
  {
    //GIVEN
    using var container = new Container();
    container.Options.ResolveUnregisteredConcreteTypes = true;

    //this passes because the cycle is not referenced by any registered type
    container.Verify(VerificationOption.VerifyAndDiagnose);

    //WHEN
    //THEN
    Invoking(container.GetInstance<One>).Should().Throw<ActivationException>();
  }
}