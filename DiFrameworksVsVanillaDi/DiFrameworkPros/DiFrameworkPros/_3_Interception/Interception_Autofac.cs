using System.Collections.Generic;
using Autofac;
using Autofac.Extras.DynamicProxy;

namespace DiFrameworkPros._3_Interception;

public static class Interception_Autofac
{
  /// <summary>
  /// Some DI Containers allow easy call interception.
  /// This can be somewhat useful in adding auto decorators to existing code
  /// or stuff like opening/closing a transaction around another call.
  /// </summary>
  [Test]
  public static void ShouldEnableInterceptionUsingAutofac()
  {
    var containerBuilder = new ContainerBuilder();
    containerBuilder
      .RegisterType<Dependency>().As<IDependency>()
      .EnableInterfaceInterceptors()
      .InterceptedBy(typeof(CallLogger));
    containerBuilder.RegisterType<CallLogger>();
    containerBuilder.RegisterType<List<string>>().SingleInstance();

    using var container = containerBuilder.Build();

    var dependency1 = container.Resolve<IDependency>();
    var dependency2 = container.Resolve<IDependency>();
    dependency1.DoSomething();
    dependency2.DoSomething();

    container.Resolve<List<string>>().Count.Should().Be(2);
  }
}