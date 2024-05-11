using Lamar;

namespace DiFrameworkCons.MultipleRecipes.Decorators._2_WithMultipleChains;

public static class DecoratorsWithMultipleChains_Lamar
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

  /// <summary>
  /// An alternative is to use keyed dependencies.
  /// Note that starting from the difference, the requirement
  /// to use keys propagates.
  /// </summary>
  [Test]
  public static void ShouldComposeVariousDecoratorConfigurationsWithKeyedDependencies()
  {
    //GIVEN
    var chain1Key = "chain1";
    var chain2Key = "chain2";
    var container = new Container(services =>
    {
      services.AddTransient<IComponent, D>();
      services.AddTransient<C1>();
      services.AddTransient<C2>();
      services.AddKeyedTransient<B>(chain1Key,
        (x, key) => ActivatorUtilities.CreateInstance<B>(
          x, x.GetRequiredService<C1>()));
      services.AddKeyedTransient<B>(chain2Key,
        (x, key) => ActivatorUtilities.CreateInstance<B>(
          x, x.GetRequiredService<C2>()));
      services.AddKeyedSingleton<A>(chain1Key,
        (x, key) => ActivatorUtilities.CreateInstance<A>(
          x, x.GetRequiredKeyedService<B>(key)));
      services.AddKeyedSingleton<A>(chain2Key,
        (x, key) => ActivatorUtilities.CreateInstance<A>(
          x, x.GetRequiredKeyedService<B>(key)));
    });

    //WHEN
    var chain1 = container.GetRequiredKeyedService<A>(chain1Key);
    var chain2 = container.GetRequiredKeyedService<A>(chain2Key);

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