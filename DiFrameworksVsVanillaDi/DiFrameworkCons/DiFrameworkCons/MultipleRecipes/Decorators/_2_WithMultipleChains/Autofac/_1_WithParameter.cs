namespace DiFrameworkCons.MultipleRecipes.Decorators._2_WithMultipleChains.Autofac;

public static class _1_WithParameter
{
  /// <summary>
  /// This autofac version doesn't use the built-in decorator feature.
  ///
  /// The code here looks to me as much more complex than in case of manual
  /// composition. Of course, we may decide to use lambda registrations and still
  /// compose each chain manually, giving up the advantages of container
  /// for this subgraph while retaining some of its cons.
  /// </summary>
  [Test]
  public static void ShouldComposeVariousDecoratorConfigurations()
  {
    //GIVEN
    var builder = new ContainerBuilder();

    builder.RegisterType<D>()
      .InstancePerDependency();
    builder.RegisterType<C1>()
      .InstancePerDependency()
      .WithParameter((info, _) => info.Name == "Next", (_, context) => context.Resolve<D>());
    builder.RegisterType<C2>()
      .InstancePerDependency()
      .WithParameter((info, _) => info.Name == "Next", (_, context) => context.Resolve<D>());
    builder.RegisterType<B>()
      .WithParameter((info, _) => info.Name == "Next", (_, context) => context.Resolve<C1>())
      .InstancePerDependency()
      .Named<B>("chain1");
    builder.RegisterType<B>()
      .WithParameter((info, _) => info.Name == "Next", (_, context) => context.Resolve<C2>())
      .InstancePerDependency()
      .Named<B>("chain2");
    builder.RegisterType<A>()
      .WithParameter((info, _) => info.Name == "Next", (_, context) => context.ResolveNamed<B>("chain1"))
      .InstancePerDependency()
      .Named<A>("chain1");
    builder.RegisterType<A>()
      .WithParameter((info, _) => info.Name == "Next", (_, context) => context.ResolveNamed<B>("chain2"))
      .InstancePerDependency()
      .Named<A>("chain2");

    //WHEN
    using var container = builder.Build();
    var chain1 = container.ResolveNamed<A>("chain1");
    var chain2 = container.ResolveNamed<A>("chain2");

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