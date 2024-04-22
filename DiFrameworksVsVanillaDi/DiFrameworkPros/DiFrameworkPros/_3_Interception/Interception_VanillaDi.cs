using Castle.DynamicProxy;
using NUnit.Framework;

namespace DiFrameworkPros._3_Interception;

public class Interception_VanillaDi
{
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
    var dependency1 = CreateDependency();
    var dependency2 = CreateDependency();

    dependency1.DoSomething();
    dependency2.DoSomething();
    return;

    static Dependency CreateDependency()
      => ProxyGenerator.CreateClassProxyWithTarget(new Dependency(), new CallLogger());
  }
}
