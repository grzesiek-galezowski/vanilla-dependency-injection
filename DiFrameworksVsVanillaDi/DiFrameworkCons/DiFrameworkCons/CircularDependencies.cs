using Autofac;
using Autofac.Core;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace DiFrameworkCons;

public class CircularDependencies
{
  [Test]
  //9.3.3 Constructor/Constructor dependencies
  public void ShouldShowFailureWhenCircularDependencyIsDiscoveredWithAutofac()
  {
    //GIVEN
    var containerBuilder = new ContainerBuilder();
    containerBuilder.RegisterType<One>();
    containerBuilder.RegisterType<Two>();
    using var container = containerBuilder.Build();
    //WHEN
    //THEN
    //TODO uncomment to see the exception
    Assert.Throws<DependencyResolutionException>(() =>
    {
      var one = container.Resolve<One>();
    });
  }

  [Test]
  //9.3.3 Constructor/Constructor dependencies
  public void ShouldShowFailureWhenCircularDependencyIsDiscoveredWithMsDi()
  {
    //GIVEN
    var containerBuilder = new ServiceCollection();
    containerBuilder.AddTransient<One>();
    containerBuilder.AddTransient<Two>();
    using var container = containerBuilder.BuildServiceProvider(true);
    //WHEN
    //THEN
    //TODO uncomment to see the exception
    Assert.Throws<DependencyResolutionException>(() =>
    {
      var one = container.GetRequiredService<One>();
    });
  }

  public record One(Two Two);
  public record Two(One One);
}