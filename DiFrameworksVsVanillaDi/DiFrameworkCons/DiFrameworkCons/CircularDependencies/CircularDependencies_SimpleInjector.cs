using SimpleInjector;

namespace DiFrameworkCons.CircularDependencies;

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
    var container = new SimpleInjector.Container();
    container.Register<One>(Lifestyle.Transient);
    container.Register(() => new Two(container.GetInstance<Three>()));
    container.Register<Three>();

    Invoking(() => container.Verify(VerificationOption.VerifyAndDiagnose))
      .Should().Throw<InvalidOperationException>();

    //WHEN
    //THEN
    Invoking(container.GetInstance<One>).Should().Throw<ActivationException>();
  }
}