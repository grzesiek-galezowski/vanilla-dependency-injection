using Lamar;
using Lamar.IoC;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace DiFrameworkPros._4_AssemblyScanning;

public class AssemblyScanning_Lamar
{
  /// <summary>
  /// Lamar has its own convention over configuration approach.
  /// However, I was unable to get it to work for internal types
  /// and found it too tedious for registering a singleton with
  /// multiple interfaces.
  /// (see https://jasperfx.github.io/lamar/documentation/ioc/registration/auto-registration-and-conventions/)
  ///
  /// So instead, this example takes advantage of the fact that Lamar container
  /// implements the IServiceCollection interface and uses the Scrutor
  /// library.
  /// </summary>
  [Test]
  public void ShouldBeAbleToResolveBasedOnConventionLamar()
  {
    var container = new Container(x =>
    {
      x.Scan(scan => scan
        .FromCallingAssembly()
        .AddClasses(classes => classes.Where(c => c.Name.EndsWith("Repository")))
        .UsingRegistrationStrategy(RegistrationStrategy.Skip)
        .AsSelfWithInterfaces()
        .WithSingletonLifetime());
    });

    var i1 = container.GetRequiredService<Interface1>();
    var i2 = container.GetRequiredService<Interface2>();

    i1.Should().BeOfType<MyRepository>();
    i2.Should().BeOfType<MyRepository>();
    i1.Should().BeSameAs(i2);

    FluentActions.Invoking(container.GetRequiredService<MyRepository2>)
      .Should().ThrowExactly<LamarMissingRegistrationException>(); //not following convention
  }
}
