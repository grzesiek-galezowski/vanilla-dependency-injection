using System;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using NSubstitute.Proxies.CastleDynamicProxy;
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
      .RegisterType<Lol2>().As<ILol2>()
      .EnableInterfaceInterceptors()
      .InterceptedBy(typeof(CallLogger));
    containerBuilder.RegisterType<CallLogger>();

    using var container = containerBuilder.Build();

    var lol1 = container.Resolve<ILol2>();
    var lol2 = container.Resolve<ILol2>();
    lol1.DoSomething();
    lol2.DoSomething();
  }

  private static readonly ProxyGenerator ProxyGenerator = new ProxyGenerator();

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
    Lol2 CreateLol2() => ProxyGenerator.CreateClassProxyWithTarget(new Lol2(), new CallLogger());

    var lol1 = CreateLol2();
    var lol2 = CreateLol2();

    lol1.DoSomething();
    lol2.DoSomething();
  }
}

public interface ILol2
{
  void DoSomething();
}

public class Lol2 : ILol2
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
