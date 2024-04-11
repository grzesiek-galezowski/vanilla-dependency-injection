using Autofac.Core;
using FluentAssertions;
using NUnit.Framework.Legacy;

namespace DiFrameworkCons;

//todo add descriptions
public class MissingDependency
{
  [Test]
  public void ShouldShowFailureWhenMissingDependencyIsDiscoveredWithAutowiredAutofac()
  {
    //GIVEN
    var containerBuilder = new ContainerBuilder();
    containerBuilder.RegisterType<One>().InstancePerDependency();
    using var container = containerBuilder.Build();
    //WHEN
    //THEN
    var dependencyResolutionException = Assert.Throws<DependencyResolutionException>(() =>
    {
      var one = container.Resolve<One>();
    });

    dependencyResolutionException!.ToString().Should().Contain(
      "None of the constructors found on type 'DiFrameworkCons.MissingDependency+One' " +
      "can be invoked with the available services and parameters:\r\n" +
      "Cannot resolve parameter 'Two Two' of constructor 'Void .ctor(Two)'.");
  }

  [Test]
  //see https://autofac.readthedocs.io/en/latest/faq/container-analysis.html
  public void ShouldShowFailureWhenMissingDependencyIsDiscoveredWithAutowiredMsDiDuringContainerBuild()
  {
    //GIVEN
    var containerBuilder = new ServiceCollection();
    containerBuilder.AddTransient<One>();

    var dependencyResolutionException = Assert.Throws<AggregateException>(() =>
    {
      using var container = containerBuilder.BuildServiceProvider(new ServiceProviderOptions
      {
        ValidateOnBuild = true,
        ValidateScopes = true
      });
    });

    dependencyResolutionException!.ToString().Should().Contain(
      "Some services are not able to be constructed " +
      "(Error while validating the service descriptor " +
      "'ServiceType: DiFrameworkCons.MissingDependency+One " +
      "Lifetime: Transient ImplementationType: DiFrameworkCons.MissingDependency+One': " +
      "Unable to resolve service for type " +
      "'DiFrameworkCons.MissingDependency+Two' " +
      "while attempting to activate 'DiFrameworkCons.MissingDependency+One'.)");
  }

  [Test]
  public void ShouldShowFailureWhenMissingDependencyIsDiscoveredWithLambdaRegisteredMsDi()
  {
    //GIVEN
    var containerBuilder = new ServiceCollection();
    containerBuilder
      .AddTransient(c => new One(c.GetRequiredService<Two>()));
    using var container = containerBuilder.BuildServiceProvider(new ServiceProviderOptions
    {
      ValidateOnBuild = true
    });
    //WHEN
    //THEN
    var dependencyResolutionException = Assert.Throws<InvalidOperationException>(() =>
    {
      var one = container.GetRequiredService<One>();
    });

    StringAssert.Contains(
      "No service for type 'DiFrameworkCons.MissingDependency+Two' has been registered.",
      dependencyResolutionException!.ToString());
  }

  //bug add vanilla DI example

  public record One(Two Two);
  public record Two;
}