using SimpleInjector;

namespace DiFrameworkCons.CompileTimeVsRuntime.CircularDependencies.SimpleInjector;

public static class _1_TypeRegistrations
{
  /// <summary>
  /// SimpleInjector has a Verify method that among others, tries to resolve
  /// all rules, and can thus detect misconfigurations.
  ///
  /// If this method is not used, then circular dependencies cause exceptions
  /// at dependency resolution time (though unlikely someone would not use Verify).
  /// </summary>
  [Test]
  public static void ShouldDetectCircularDependencyDuringConfigVerification()
  {
    //GIVEN
    using var container = new Container();
    container.Register<One>();
    container.Register<Two>();
    container.Register<Three>();

    Invoking(() => container.Verify(VerificationOption.VerifyAndDiagnose))
      .Should().Throw<InvalidOperationException>();

    //WHEN
    //THEN
    Invoking(container.GetInstance<One>).Should().Throw<ActivationException>();
  }
}