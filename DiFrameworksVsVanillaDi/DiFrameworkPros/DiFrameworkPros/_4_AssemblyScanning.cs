using System;
using System.Reflection;
using Autofac;
using Autofac.Core.Registration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Scrutor;

namespace DiFrameworkPros;

public class AssemblyScanning
{
  /// <summary>
  /// DI containers have unique ability to register types from assemblies
  /// using a "convention over configuration" approach.
  /// </summary>
  [Test]
  public void ShouldBeAbleToResolveBasedOnConventionUsingAutofac()
  {
    var builder = new ContainerBuilder();

    builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
        .Where(t => t.Name.EndsWith("Repository"))
        .AsImplementedInterfaces().SingleInstance();

    using var container = builder.Build();

    var i1 = container.Resolve<Interface1>();
    var i2 = container.Resolve<Interface2>();

    Assert.IsInstanceOf<MyRepository>(i1);
    Assert.IsInstanceOf<MyRepository>(i2);
    Assert.AreEqual(i2, i1);

    Assert.Throws<ComponentNotRegisteredException>(
      () => container.Resolve<MyRepository2>()); //not following convention
  }

  [Test]
  public void ShouldBeAbleToResolveBasedOnConventionUsingMsDi()
  {
    var builder = new ServiceCollection();

    builder.Scan(scan => scan     
      .FromCallingAssembly()
      .AddClasses(classes => classes.Where(c => c.Name.EndsWith("Repository")))
      .UsingRegistrationStrategy(RegistrationStrategy.Skip)
      .AsSelfWithInterfaces()
      .WithSingletonLifetime());

    using var container = builder.BuildServiceProvider();

    var i1 = container.GetRequiredService<Interface1>();
    var i2 = container.GetRequiredService<Interface2>();

    Assert.IsInstanceOf<MyRepository>(i1);
    Assert.IsInstanceOf<MyRepository>(i2);
    Assert.AreEqual(i2, i1);

    Assert.Throws<InvalidOperationException>(
      () => container.GetRequiredService<MyRepository2>()); //not following convention
  }

  //Vanilla dependency injection doesn't have any alternative. The whole point of
  //Vanilla DI is to create the dependencies manually instead of using reflection.
}

public interface Interface1
{

}
public interface Interface2
{

}

public class MyRepository : Interface1, Interface2
{

}

public class MyRepository2 : Interface1, Interface2
{

}