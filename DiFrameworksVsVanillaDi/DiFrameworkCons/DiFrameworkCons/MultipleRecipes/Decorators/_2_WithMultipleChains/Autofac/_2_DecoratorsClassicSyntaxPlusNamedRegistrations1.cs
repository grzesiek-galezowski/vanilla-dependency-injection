namespace DiFrameworkCons.MultipleRecipes.Decorators._2_WithMultipleChains.Autofac;

public static class _2_DecoratorsClassicSyntaxPlusNamedRegistrations1
{
  /// <summary>
  /// This autofac version uses the built-in decorator feature - classic syntax.
  ///
  /// In this version, the decorators are created manually, which means that
  /// the advantages from automatic resolution are more limited because every
  /// constructor needs to be invoked.
  /// </summary>
  [Test]
  public static void ShouldComposeVariousDecoratorConfigurationsUsingAutofacDecoratorsFeature()
  {
    //GIVEN
    var builder = new ContainerBuilder();

    builder.RegisterType<D>()
      .InstancePerDependency()
      .Named<IComponent>("D");
    builder.RegisterDecorator<IComponent>(
        (_, inner) => new C1(inner), fromKey: "D")
      .Named<IComponent>("C1");
    builder.RegisterDecorator<IComponent>(
        (_, inner) => new C2(inner), fromKey: "D")
      .Named<IComponent>("C2");
    builder.RegisterDecorator<IComponent>(
        (_, inner) => new B(inner), fromKey: "C1")
      .Named<IComponent>("B1");
    builder.RegisterDecorator<IComponent>(
        (_, inner) => new B(inner), fromKey: "C2")
      .Named<IComponent>("B2");
    builder.RegisterDecorator<IComponent>(
        (_, inner) => new A(inner), fromKey: "B1")
      .Named<IComponent>("A1");
    builder.RegisterDecorator<IComponent>(
        (_, inner) => new A(inner), fromKey: "B2")
      .Named<IComponent>("A2");

    //WHEN
    using var container = builder.Build();
    var chain1 = container.ResolveNamed<IComponent>("A1");
    var chain2 = container.ResolveNamed<IComponent>("A2");

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