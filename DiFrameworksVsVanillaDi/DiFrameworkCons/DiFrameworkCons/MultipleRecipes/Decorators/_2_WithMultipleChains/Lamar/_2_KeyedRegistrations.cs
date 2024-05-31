using Lamar;

namespace DiFrameworkCons.MultipleRecipes.Decorators._2_WithMultipleChains.Lamar;

public static class _2_KeyedRegistrations
{
  /// <summary>
  /// An alternative is to use keyed registrations.
  /// Note that starting from the difference in the two chains, the requirement
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
      services.AddKeyedTransient(chain1Key,
        (x, key) => ActivatorUtilities.CreateInstance<B>(
          x, x.GetRequiredService<C1>()));
      services.AddKeyedTransient(chain2Key,
        (x, key) => ActivatorUtilities.CreateInstance<B>(
          x, x.GetRequiredService<C2>()));
      services.AddKeyedSingleton(chain1Key,
        (x, key) => ActivatorUtilities.CreateInstance<A>(
          x, x.GetRequiredKeyedService<B>(key)));
      services.AddKeyedSingleton(chain2Key,
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