namespace DiFrameworkCons;

/// <summary>
/// Even more challenging for a DI container is a situation where
/// we want to have multiple chains of decorators with slight differences.
/// </summary>
public class DecoratorsWithMultipleChains
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

    //WHEN

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
  /// This autofac version doesn't use the built-in decorator feature.
  ///
  /// The code here looks to me as much more complex than in case of manual
  /// composition. Of course, we may decide to use lambda registrations and still
  /// compose each chain manually, giving up the advantages of container
  /// for this subgraph while retaining some of its cons.
  /// </summary>
  [Test]
  public void ShouldComposeVariousDecoratorConfigurationsUsingAutofac()
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
  public void ShouldComposeVariousDecoratorConfigurationsUsingAutofacDecoratorsFeature1()
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
  public void ShouldComposeVariousDecoratorChainsUsingAutofacDecoratorsFeature2()
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

  /// <summary>
  /// MsDi allows something that's pretty close to the manual code
  /// with many of the benefits of the container (like auto wiring)
  /// by using the ActivatorUtilities class.
  ///
  /// Note though that this approach is based on lambda registrations
  /// and thus most of this code is not covered by container validation.
  /// </summary>
  [Test]
  public void ShouldComposeVariousDecoratorConfigurationsWithMsDi()
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

  public interface IComponent
  {
    IComponent? Next { get; }
  }

  private record A(IComponent Next) : IComponent;
  private record B(IComponent Next) : IComponent;
  private record C1(IComponent Next) : IComponent;
  private record C2(IComponent Next) : IComponent;
  private record D() : IComponent
  {
    public IComponent? Next { get; }
  }
}
