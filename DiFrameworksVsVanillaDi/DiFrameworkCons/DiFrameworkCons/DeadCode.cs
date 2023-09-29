using Autofac;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace DiFrameworkCons;

/// <summary>
/// Sometimes, you may have some dependencies that you simply don't use
/// or don't use anymore. 
/// </summary>
public class DeadCode
{
  /// <summary>
  /// The container doesn't know which registrations are gonna be used
  /// so there's no way of detecting "dead" dependencies that are not used or passed anywhere
  /// </summary>
  [Test]
  public void ContainerContainsSomeDeadCodeWithAutofac()
  {
    //GIVEN
    var builder = new ContainerBuilder();
    builder.RegisterType<Dependency>().SingleInstance();
    builder.RegisterType<DependencyConsumer>().SingleInstance();
    //dead code
    builder.RegisterType<DeadCode>().InstancePerDependency();
    using var container = builder.Build();

    //WHEN
    var resolvedInstance = container.Resolve<DependencyConsumer>();

    //THEN
    Assert.NotNull(resolvedInstance);
  }

  [Test]
  public void ContainerContainsSomeDeadCodeWithMsDi()
  {
    //GIVEN
    var builder = new ServiceCollection();
    builder.AddSingleton<Dependency>();
    builder.AddSingleton<DependencyConsumer>();
    //dead code
    builder.AddSingleton<DeadCode>();
    using var container = builder.BuildServiceProvider();

    //WHEN
    var resolvedInstance = container.GetRequiredService<DependencyConsumer>();

    //THEN
    Assert.NotNull(resolvedInstance);
  }

  /// <summary>
  /// "Dead" dependencies can be clearly visible in the imperative code when
  /// doing Vanilla DI - below, the `deadCode` will be marked by the IDE as unused.
  /// </summary>
  [Test]
  public void VanillaDiContainsDeadCode()
  {
    //GIVEN
    var consumer = new DependencyConsumer(new Dependency());
    var deadCode = new DeadCode();

    //WHEN

    //THEN
    Assert.NotNull(consumer);
  }
  //bug new example: two registrations of the same type - with different lifestyles
}


public record DependencyConsumer(Dependency Dependency);
public record Dependency;