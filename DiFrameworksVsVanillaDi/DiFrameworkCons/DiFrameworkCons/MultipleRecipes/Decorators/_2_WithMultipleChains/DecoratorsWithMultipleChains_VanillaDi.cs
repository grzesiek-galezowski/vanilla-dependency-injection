namespace DiFrameworkCons.MultipleRecipes.Decorators._2_WithMultipleChains;

public class DecoratorsWithMultipleChains_VanillaDi
{
  /// <summary>
  /// in Vanilla DI, composing a chain of decorators is as straightforward
  /// as composing any other object graph. No matter the order and combination
  /// of different decorators, it mostly takes the same effort to chain them.
  ///
  /// If a particular section of the decorator chains needs to be always the same,
  /// We can extract this section to a function and invoke in each case. 
  /// </summary>
  [Test]
  public void ShouldComposeVariousDecoratorConfigurationsWithVanillaDi()
  {
    //GIVEN
    var chain1 = new A(new B(new C1(new D())));
    var chain2 = new A(new B(new C2(new D())));

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

