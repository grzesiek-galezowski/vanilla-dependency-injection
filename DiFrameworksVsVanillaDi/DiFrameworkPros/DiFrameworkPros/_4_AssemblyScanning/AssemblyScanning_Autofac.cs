using System.Reflection;
using Autofac;
using Autofac.Core.Registration;

namespace DiFrameworkPros._4_AssemblyScanning;

public static class AssemblyScanning_Autofac
{
  /// <summary>
  /// DI containers have unique ability to register types from assemblies
  /// using a "convention over configuration" approach.
  ///
  /// Autofac has assembly scanning built in
  /// </summary>
  [Test]
  public static void ShouldBeAbleToResolveBasedOnConvention()
  {
    var builder = new ContainerBuilder();

    builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
      .Where(t => t.Name.EndsWith("Repository"))
      .AsImplementedInterfaces().SingleInstance();

    using var container = builder.Build();

    var i1 = container.Resolve<Interface1>();
    var i2 = container.Resolve<Interface2>();

    i1.Should().BeOfType<MyRepository>();
    i2.Should().BeOfType<MyRepository>();
    i1.Should().BeSameAs(i2);

    FluentActions.Invoking(container.Resolve<MyRepository2>)
      .Should().Throw<ComponentNotRegisteredException>(); //not following convention
  }
}