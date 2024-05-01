using System.Collections.Generic;
using Castle.DynamicProxy;

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
    var messages = new List<string>();
    var dependency1 = CreateDependency();
    var dependency2 = CreateDependency();

    dependency1.DoSomething();
    dependency2.DoSomething();

    messages.Count.Should().Be(2);
    return;

    IDependency CreateDependency()
    {
      return ProxyGenerator.CreateInterfaceProxyWithTarget<IDependency>(
        new Dependency(),
        new CallLogger(messages));
    }
  }
}
