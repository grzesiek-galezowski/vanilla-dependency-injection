namespace DiFrameworkCons.MultipleRecipes.Decorators._2_WithMultipleChains.MsDi;

public static class DecoratorsWithMultipleChains_MsDi
{
  /// <summary>
  /// MsDi allows something that's pretty close to the manual code
  /// with many of the benefits of the container
  /// by using the ActivatorUtilities class.
  ///
  /// Note though that this approach is based on lambda registrations
  /// and thus most of this code is not covered by container validation.
  /// Also, it's quite fragile - the nesting is dynamic and there
  /// is no checking whether we are passing the right dependencies
  /// in the right places.
  /// </summary>
  [Test]
  public static void ShouldComposeVariousDecoratorConfigurationsWithActivatorUtilities()
  {
    //GIVEN
    var services = new ServiceCollection();
    services.AddKeyedSingleton("chain1",
      (c, _) => ActivatorUtilities.CreateInstance<A>(c,
        ActivatorUtilities.CreateInstance<B>(c,
          ActivatorUtilities.CreateInstance<C1>(c,
            ActivatorUtilities.CreateInstance<D>(c)))));

    services.AddKeyedSingleton("chain2",
      (c, _) => ActivatorUtilities.CreateInstance<A>(c,
        ActivatorUtilities.CreateInstance<B>(c,
          ActivatorUtilities.CreateInstance<C2>(c,
            ActivatorUtilities.CreateInstance<D>(c)))));

    //WHEN
    using var container = services.BuildServiceProvider();
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
    var services = new ServiceCollection();
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

    //WHEN
    using var container = services.BuildServiceProvider();
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