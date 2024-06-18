using System.Collections.Generic;
using Castle.DynamicProxy;
using DiFrameworkCons.SimpleInjectorExtensions;
using Microsoft.Extensions.DependencyInjection;
using SimpleInjector;

namespace DiFrameworkPros._3_Interception;

internal class Interception_SimpleInjector
{
  /// <summary>
  /// SimpleInjector can only intercept specified types.
  ///
  /// More native interception experience so far not included by default.
  /// 
  /// See https://docs.simpleinjector.org/en/latest/InterceptionExtensions.html
  /// 
  /// Also see https://stackoverflow.com/questions/74578676/intercept-all-instances-using-simple-injector
  /// 
  /// Some nuget packages might exist that add this capability to the SimpleInjector container.
  /// </summary>
  [Test]
  public void ShouldEnableInterceptionUsingLamar()
  {
    //GIVEN
    var proxyGenerator = new ProxyGenerator();
    using var container = new Container();
    container.Options.ConstructorResolutionBehavior = new PerTypeConstructorBehavior(
      new Dictionary<Type, List<Type>>
      {
        [typeof(List<string>)] = []
      },
      container.Options.ConstructorResolutionBehavior);

    container.RegisterSingleton<CallLogger>();
    container.RegisterSingleton<List<string>>();
    container.RegisterSingleton<Dependency>();
    container.RegisterSingleton(() =>
      proxyGenerator.CreateInterfaceProxyWithTargetInterface<IDependency>(
        container.GetRequiredService<Dependency>(),
        container.GetRequiredService<CallLogger>()));

    var dependency1 = container.GetRequiredService<IDependency>();
    var dependency2 = container.GetRequiredService<IDependency>();

    //WHEN
    dependency1.DoSomething();
    dependency2.DoSomething();

    //THEN
    container.GetRequiredService<List<string>>().Count.Should().Be(2);
  }
}