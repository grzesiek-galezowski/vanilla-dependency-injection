using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Scrutor;

namespace DiFrameworkPros._4_AssemblyScanning;

public static class AssemblyScanning_MsDi
{
  /// <summary>
  /// MsDi doesn't have assembly scanning by default (as of 2023),
  /// but this capability can be added using e.g. the Scrutor library.
  /// </summary>
  [Test]
  public static void ShouldBeAbleToResolveBasedOnConventionUsingMsDiAndScrutor()
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

    i1.Should().BeOfType<MyRepository>();
    i2.Should().BeOfType<MyRepository>();
    i1.Should().BeSameAs(i2);

    FluentActions.Invoking(container.GetRequiredService<MyRepository2>)
      .Should().Throw<InvalidOperationException>(); //not following convention
  }
}