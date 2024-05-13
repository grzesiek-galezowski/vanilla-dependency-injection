using Pure.DI;

namespace DiFrameworkCons.MultipleRecipes.Decorators._2_WithMultipleChains;

public class DecoratorsWithMultipleChains_PureDiLibrary
{
  [Test]
  public static void ShouldComposeVariousDecoratorConfigurationsWithKeyedDependencies()
  {
    //GIVEN
    var composition = new Composition8();

    //WHEN
    var chain1 = composition.Chain1;
    var chain2 = composition.Chain2;

    //THEN
    chain1.Next.Should().BeOfType<B>();
    chain1.Next.Next.Should().BeOfType<C1>();
    chain1.Next.Next!.Next.Should().BeOfType<D>();
    chain1.Next.Next!.Next!.Next.Should().BeNull();
    
    chain2.Next.Should().BeOfType<B>();
    chain2.Next.Next.Should().BeOfType<C2>();
    chain2.Next.Next!.Next.Should().BeOfType<D>();
    chain2.Next.Next!.Next!.Next.Should().BeNull();
  }
}

partial class Composition8
{
  public void Setup()
  {
    DI.Setup(nameof(Composition8))
      .Bind<D>().As(Lifetime.Transient).To<D>()
      .Bind<C1>().As(Lifetime.Transient).To(context =>
      {
        context.Inject<D>(out var d);
        return new C1(d);
      })
      .Bind<C2>().As(Lifetime.Transient).To(context =>
      {
        context.Inject<D>(out var d);
        return new C2(d);
      })
      .Bind<B>("chain1B").As(Lifetime.Transient).To(context =>
        {
          context.Inject<C1>(out var c1);
          return new B(c1);
        })
      .Bind<B>("chain2B").As(Lifetime.Transient).To(context =>
        {
          context.Inject<C2>(out var c2);
          return new B(c2);
        })
      .Bind<A>("chain1").As(Lifetime.Singleton)
      .To(context =>
      {
        context.Inject<B>("chain1B", out var b);
        return new A(b);
      })
      .Bind<A>("chain2").As(Lifetime.Singleton)
      .To(context =>
      {
        context.Inject<B>("chain2B", out var b);
        return new A(b);
      })
      .Root<A>("Chain1", tag: "chain1")
      .Root<A>("Chain2", tag: "chain2");
  }
}
