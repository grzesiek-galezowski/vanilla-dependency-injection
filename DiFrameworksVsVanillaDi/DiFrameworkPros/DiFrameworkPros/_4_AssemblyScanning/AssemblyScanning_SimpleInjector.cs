using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SimpleInjector;

namespace DiFrameworkPros._4_AssemblyScanning;

public class AssemblyScanning_SimpleInjector
{
  /// <summary>
  /// SimpleInjector does not have assembly scanning API,
  /// but the authors suggest to just use Linq queries
  /// over reflection as a viable alternative
  /// (see: https://docs.simpleinjector.org/en/latest/advanced.html#batch-registration-auto-registration)
  /// </summary>
  [Test]
  public void ShouldBeAbleToResolveBasedOnConventionSimpleInjector()
  {
    using var container = new Container();

    var registrations =
      from type in Assembly.GetExecutingAssembly().GetTypes().ToList()
      where type.Name.EndsWith("Repository")
      from service in type.GetInterfaces()
      select new { service, implementation = type };

    foreach (var reg in registrations)
    {
      container.Register(reg.service, reg.implementation, Lifestyle.Singleton);
    }

    var i1 = container.GetRequiredService<Interface1>();
    var i2 = container.GetRequiredService<Interface2>();

    i1.Should().BeOfType<MyRepository>();
    i2.Should().BeOfType<MyRepository>();
    i1.Should().BeSameAs(i2);

    FluentActions.Invoking(container.GetRequiredService<MyRepository2>)
      .Should().ThrowExactly<InvalidOperationException>(); //not following the convention
  }
}
