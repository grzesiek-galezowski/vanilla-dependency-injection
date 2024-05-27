using SimpleInjector;

namespace DiFrameworkCons.CompileTimeVsRuntime.CircularDependencies.SimpleInjector;

public static class _2_LambdaRegistration
{
  /// <summary>
  /// Because SimpleInjector's Verify method tries to resolve all registered types,
  /// it will also catch circular dependencies with lambda registrations.
  /// </summary>
  [Test]
  public static void ShouldDetectCircularDependencyDuringConfigVerification()
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
}