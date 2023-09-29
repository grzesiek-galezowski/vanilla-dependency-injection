using System;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using NUnit.Framework;

namespace DiFrameworkPros;

public class Interception
{
  /// <summary>
  /// Some DI Containers allow easy call interception.
  /// This can be somewhat useful in adding auto decorators to existing code
  /// or stuff like opening/closing a transaction around another call.
  /// </summary>
  [Test]
  public void ShouldEnableInterceptionUsingAutofac()
  {
    var containerBuilder = new ContainerBuilder();
    containerBuilder
      .RegisterType<Dependency>().As<IDependency>()
      .EnableInterfaceInterceptors()
      .InterceptedBy(typeof(CallLogger));
    containerBuilder.RegisterType<CallLogger>();

    using var container = containerBuilder.Build();

    var dependency1 = container.Resolve<IDependency>();
    var dependency2 = container.Resolve<IDependency>();
    dependency1.DoSomething();
    dependency2.DoSomething();
  }

  private static readonly ProxyGenerator ProxyGenerator = new();

  /// <summary>
  /// With manual DI, I'd rather just use handmade proxies, but if you want
  /// to use dynamic proxies, just wrap object creation in a function, apply
  /// dynamic proxy inside the function and use that function everywhere you
  /// need an instance or your interface. It's probably less powerful but most
  /// of the time it does the trick.
  /// </summary>
  [Test]
  public void ShouldEnableInterception()
  {
    Dependency CreateDependency()
      => ProxyGenerator.CreateClassProxyWithTarget(new Dependency(), new CallLogger());

    var dependency1 = CreateDependency();
    var dependency2 = CreateDependency();

    dependency1.DoSomething();
    dependency2.DoSomething();
  }

  public interface IDependency
  {
    void DoSomething();
  }

  public class Dependency : IDependency
  {
    public void DoSomething()
    {

    }
  }

  public class CallLogger : IInterceptor
  {
    public void Intercept(IInvocation invocation)
    {
      Console.WriteLine("Called " + invocation.Method.Name);
    }
  }
}
