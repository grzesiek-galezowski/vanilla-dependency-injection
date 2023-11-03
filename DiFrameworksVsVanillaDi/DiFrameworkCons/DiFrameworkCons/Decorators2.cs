using Autofac.Core;
using Scrutor;

namespace DiFrameworkCons;

public class Decorators2
{
  [Test]
  public void ShouldComposeVariousDecoratorConfigurationsWithVanillaDi()
  {
    //GIVEN
    var chain1 = new A(new B(new C1(new D())));
    var chain2 = new A(new B(new C2(new D())));
    //WHEN

    //THEN
    Assert.IsInstanceOf<B>(chain1.Next);
    Assert.IsInstanceOf<C1>(chain1.Next.Next);
    Assert.IsInstanceOf<D>(chain1.Next.Next!.Next);
    Assert.IsNull(chain1.Next.Next!.Next!.Next);

    Assert.IsInstanceOf<B>(chain2.Next);
    Assert.IsInstanceOf<C2>(chain2.Next.Next);
    Assert.IsInstanceOf<D>(chain2.Next.Next!.Next);
    Assert.IsNull(chain2.Next.Next!.Next!.Next);
  }


  public interface IComponent
  {
    IComponent? Next { get; }
  }

  private record A(IComponent Next) : IComponent;
  private record B(IComponent Next) : IComponent;
  private record C1(IComponent Next) : IComponent;
  private record C2(IComponent Next) : IComponent;
  private record D() : IComponent
  {
    public IComponent? Next { get; }
  }
}