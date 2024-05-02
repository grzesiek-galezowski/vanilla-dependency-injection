using System.Collections.Generic;
using Castle.DynamicProxy;
using Lamar;
using Microsoft.Extensions.DependencyInjection;

namespace DiFrameworkPros._3_Interception
{
  internal class Interception_Lamar
  {
    /// <summary>
    /// Lamar can only intercept specified types?
    /// </summary>
    [Test]
    public void ShouldEnableInterceptionUsingLamar()
    {
      //GIVEN
      var proxyGenerator = new ProxyGenerator();
      var container = new Container(x =>
      {
        x.AddSingleton<CallLogger>();
        x.AddSingleton<List<string>>();
        x.For<IDependency>().Add<Dependency>()
          .OnCreation((context, dependency) =>
            proxyGenerator.CreateInterfaceProxyWithTargetInterface<IDependency>(
              dependency,
              context.GetRequiredService<CallLogger>()));
      });
      var dependency1 = container.GetRequiredService<IDependency>();
      var dependency2 = container.GetRequiredService<IDependency>();

      //WHEN
      dependency1.DoSomething();
      dependency2.DoSomething();

      //THEN
      container.GetRequiredService<List<string>>().Count.Should().Be(2);
    }
  }
}
