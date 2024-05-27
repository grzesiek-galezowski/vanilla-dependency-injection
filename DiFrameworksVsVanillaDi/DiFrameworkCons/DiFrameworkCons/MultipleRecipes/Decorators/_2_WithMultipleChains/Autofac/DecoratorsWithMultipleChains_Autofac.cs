namespace DiFrameworkCons.MultipleRecipes.Decorators._2_WithMultipleChains.Autofac;

public static class DecoratorsWithMultipleChains_Autofac
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
  public static void ShouldComposeVariousDecoratorConfigurationsUsingAutofac()
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

  /// <summary>
  /// This autofac version uses the built-in decorator feature - classic syntax.
  ///
  /// The decorators are created manually which means that the advantages
  /// from autowiring are more limited because every constructor needs to be invoked.
  /// </summary>
  [Test]
  public static void ShouldComposeVariousDecoratorConfigurationsUsingAutofacDecoratorsFeature1()
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