namespace DiFrameworkCons.MultipleRecipes.Decorators._2_WithMultipleChains.Autofac;

public static class _3_DecoratorsClassicSyntaxPlusNamedRegistrations2
{
  /// <summary>
  /// This autofac version uses the built-in decorator feature - classic syntax.
  ///
  /// The decorators are registered and resolved from container.
  /// More autowiring advantages at the cost of more LoC
  /// </summary>
  [Test]
  public static void ShouldComposeVariousDecoratorChainsUsingAutofacDecoratorsFeature2()
  {
    //GIVEN
    var builder = new ContainerBuilder();

    builder.RegisterType<D>().InstancePerDependency().Named<IComponent>("D");
    builder.RegisterType<C1>().InstancePerDependency();
    builder.RegisterType<C2>().InstancePerDependency();
    builder.RegisterType<B>().InstancePerDependency();
    builder.RegisterType<A>().InstancePerDependency();
    builder.RegisterDecorator<IComponent>(
        (c, inner) => c.Resolve<C1>(new TypedParameter(typeof(IComponent), inner)),
        fromKey: "D")
      .Named<IComponent>("C1");
    builder.RegisterDecorator<IComponent>(
        (c, inner) => c.Resolve<C2>(new TypedParameter(typeof(IComponent), inner)),
        fromKey: "D")
      .Named<IComponent>("C2");
    builder.RegisterDecorator<IComponent>(
        (c, inner) => c.Resolve<B>(new TypedParameter(typeof(IComponent), inner)),
        fromKey: "C1")
      .Named<IComponent>("B1");
    builder.RegisterDecorator<IComponent>(
        (c, inner) => c.Resolve<B>(new TypedParameter(typeof(IComponent), inner)),
        fromKey: "C2")
      .Named<IComponent>("B2");
    builder.RegisterDecorator<IComponent>(
        (c, inner) => c.Resolve<A>(new TypedParameter(typeof(IComponent), inner)),
        fromKey: "B1")
      .Named<IComponent>("chain1");
    builder.RegisterDecorator<IComponent>(
        (c, inner) => c.Resolve<A>(new TypedParameter(typeof(IComponent), inner)),
        fromKey: "B2")
      .Named<IComponent>("chain2");

    //WHEN
    using var container = builder.Build();
    var chain1 = container.ResolveNamed<IComponent>("chain1");
    var chain2 = container.ResolveNamed<IComponent>("chain2");

    //THEN
    chain1.Next.Should().BeOfType<B>();
    chain1.Next!.Next.Should().BeOfType<C1>();
    chain1.Next.Next!.Next.Should().BeOfType<D>();
    chain1.Next.Next!.Next!.Next.Should().BeNull();

    chain2.Next.Should().BeOfType<B>();
    chain2.Next!.Next.Should().BeOfType<C2>();
    chain2.Next.Next!.Next.Should().BeOfType<D>();
    chain2.Next.Next!.Next!.Next.Should().BeNull();
  }
}