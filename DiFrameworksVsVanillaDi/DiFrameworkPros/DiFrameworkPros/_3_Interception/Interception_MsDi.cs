using System.Collections.Generic;
using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;

namespace DiFrameworkPros._3_Interception;

internal class Interception_MsDi
{
  /// <summary>
  /// MsDi can only intercept specified types?
  /// </summary>
  [Test]
  public void ShouldEnableInterceptionUsingLamar()
  {
    //GIVEN
    var proxyGenerator = new ProxyGenerator();
    var serviceCollection = new ServiceCollection();

    serviceCollection.AddSingleton<CallLogger>();
      serviceCollection.AddSingleton<List<string>>();
      serviceCollection.AddTransient<Dependency>();
      serviceCollection.AddTransient(x => 
          proxyGenerator.CreateInterfaceProxyWithTargetInterface<IDependency>(
            x.GetRequiredService<Dependency>(),
            x.GetRequiredService<CallLogger>()));

    using var container = serviceCollection.BuildServiceProvider(new ServiceProviderOptions()
    {
      ValidateOnBuild = true,
      ValidateScopes = true
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