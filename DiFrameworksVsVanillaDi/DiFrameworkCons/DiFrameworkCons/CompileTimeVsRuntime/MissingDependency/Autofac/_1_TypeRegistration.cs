using Autofac.Core;

namespace DiFrameworkCons.CompileTimeVsRuntime.MissingDependency.Autofac;

public static class _1_TypeRegistration
{
  [Test]
  //see https://autofac.readthedocs.io/en/latest/faq/container-analysis.html
  public static void ShouldShowFailureWhenMissingDependencyIsDiscoveredWithAutofac()
  {
    //GIVEN
    var containerBuilder = new ContainerBuilder();
    containerBuilder.RegisterType<One>().InstancePerDependency();
    using var container = containerBuilder.Build();
    //WHEN
    //THEN
    Invoking(() =>
      {
        var one = container.Resolve<One>();
      }).Should().Throw<DependencyResolutionException>()
      .Which.ToString().Should().Contain(
        "None of the constructors found on type 'DiFrameworkCons.CompileTimeVsRuntime.MissingDependency.One' " +
        "can be invoked with the available services and parameters:\r\n" +
        "Cannot resolve parameter 'DiFrameworkCons.CompileTimeVsRuntime.MissingDependency.ITwo Two' " +
        "of constructor " +
        "'Void .ctor(DiFrameworkCons.CompileTimeVsRuntime.MissingDependency.ITwo)'.");
  }
}