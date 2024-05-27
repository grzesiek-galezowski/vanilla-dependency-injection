using SimpleInjector;

namespace DiFrameworkCons.CompileTimeVsRuntime.CircularDependencies.SimpleInjector;

public static class _3_ResolutionOfUnregisteredTypes
{
  /// <summary>
  /// In case the cycle is not referenced by any registered type,
  /// the ResolveUnregisteredConcreteTypes option leads to verification
  /// passing despite there being a cycle.
  ///
  /// To be fair, resolving unregistered types isn't a recommended approach
  /// for SimpleInjector and the container's author warns against it.
  /// See <url>https://simpleinjector.org/ructd/</url>
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