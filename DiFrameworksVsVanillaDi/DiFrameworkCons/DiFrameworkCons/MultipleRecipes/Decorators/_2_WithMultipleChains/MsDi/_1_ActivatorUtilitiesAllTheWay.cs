namespace DiFrameworkCons.MultipleRecipes.Decorators._2_WithMultipleChains.MsDi;

public static class _1_ActivatorUtilitiesAllTheWay
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
}