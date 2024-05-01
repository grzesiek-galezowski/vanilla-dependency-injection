using System.Collections.Generic;
using Castle.DynamicProxy;
using Pure.DI;

namespace DiFrameworkPros._3_Interception;

internal class Interception_PureDiLibrary
{
  [Test]
  public void ShouldEnableInterceptionUsingPureDiLibrary()
  {
    var composition = new Composition3();
    var dependency1 = composition.Root;
    var dependency2 = composition.Root;
    dependency1.DoSomething();
    dependency2.DoSomething();

    dependency1.Should().NotBeSameAs(dependency2);
    composition.Messages.Count.Should().Be(2);
  }
}

partial class Composition3
{
  private static readonly ProxyGenerator ProxyGenerator = new();
  public readonly List<string> Messages = [];

  public void Setup()
  {
    // OnDependencyInjection = On
    // OnDependencyInjectionContractTypeNameRegularExpression = IDependency
    DI.Setup(nameof(Composition3))
      .RootBind<IDependency>("Root").As(Lifetime.Transient).To<Dependency>();
  }

  private partial T OnDependencyInjection<T>(
    in T value,
    object? tag,
    Lifetime lifetime)
  {
    if (typeof(T).IsValueType)
    {
      return value;
    }

    return (T)ProxyGenerator.CreateInterfaceProxyWithTargetInterface(
      typeof(T),
      value,
      new CallLogger(Messages));
  }
}