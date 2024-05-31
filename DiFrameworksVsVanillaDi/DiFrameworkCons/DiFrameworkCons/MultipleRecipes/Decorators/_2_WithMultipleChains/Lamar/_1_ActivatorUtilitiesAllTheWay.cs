using Lamar;

namespace DiFrameworkCons.MultipleRecipes.Decorators._2_WithMultipleChains.Lamar;

public static class _1_ActivatorUtilitiesAllTheWay
{
  /// <summary>
  /// Lamar supports MsDi interfaces, so we can do exactly the same thing
  /// as with MsDi.
  /// </summary>
  [Test]
  public static void ShouldComposeVariousDecoratorConfigurationsWithMsDi()
  {
    //GIVEN
    using var container = new Container(registry =>
    {
      registry.AddKeyedSingleton("chain1",
        (c, _) => ActivatorUtilities.CreateInstance<A>(c,
          ActivatorUtilities.CreateInstance<B>(c,
            ActivatorUtilities.CreateInstance<C1>(c,
              ActivatorUtilities.CreateInstance<D>(c)))));

      registry.AddKeyedSingleton("chain2",
        (c, _) => ActivatorUtilities.CreateInstance<A>(c,
          ActivatorUtilities.CreateInstance<B>(c,
            ActivatorUtilities.CreateInstance<C2>(c,
              ActivatorUtilities.CreateInstance<D>(c)))));
    });

    //WHEN
    var chain1 = container.GetRequiredKeyedService<A>("chain1");
    var chain2 = container.GetRequiredKeyedService<A>("chain2");

    //THEN
    chain1.Next.Should().BeOfType<B>();
    chain1.Next.Next.Should().BeOfType<C1>();
    chain1.Next.Next!.Next.Should().BeOfType<D>();

    chain2.Next.Should().BeOfType<B>();
    chain2.Next.Next.Should().BeOfType<C2>();
    chain2.Next.Next!.Next.Should().BeOfType<D>();
    chain2.Next.Next!.Next!.Next.Should().BeNull();
  }
}