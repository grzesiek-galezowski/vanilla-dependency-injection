namespace DiFrameworkCons;

public class Decorators2
{
  [Test]
  public void ShouldComposeVariousDecoratorConfigurationsWithVanillaDi()
  {
    //GIVEN
    var chain1 = new A(new B(new C1(new D())));
    var chain2 = new A(new B(new C2(new D())));
    //WHEN

    //THEN
    Assert.IsInstanceOf<B>(chain1.Next);
    Assert.IsInstanceOf<C1>(chain1.Next.Next);
    Assert.IsInstanceOf<D>(chain1.Next.Next!.Next);
    Assert.IsNull(chain1.Next.Next!.Next!.Next);

    Assert.IsInstanceOf<B>(chain2.Next);
    Assert.IsInstanceOf<C2>(chain2.Next.Next);
    Assert.IsInstanceOf<D>(chain2.Next.Next!.Next);
    Assert.IsNull(chain2.Next.Next!.Next!.Next);
  }

  [Test] //no decorators version. Can it be simplified using decorators?
  public void ShouldComposeVariousDecoratorConfigurationsUsingAutofac()
  {
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

    using var container = builder.Build();
    var chain1 = container.ResolveNamed<A>("chain1");
    var chain2 = container.ResolveNamed<A>("chain2");

    //THEN
    Assert.IsInstanceOf<B>(chain1.Next);
    Assert.IsInstanceOf<C1>(chain1.Next.Next);
    Assert.IsInstanceOf<D>(chain1.Next.Next!.Next);
    Assert.IsNull(chain1.Next.Next!.Next!.Next);

    Assert.IsInstanceOf<B>(chain2.Next);
    Assert.IsInstanceOf<C2>(chain2.Next.Next);
    Assert.IsInstanceOf<D>(chain2.Next.Next!.Next);
    Assert.IsNull(chain2.Next.Next!.Next!.Next);
  }

  [Test]
  public void ShouldComposeVariousDecoratorConfigurationsWithMsDi()
  {
    //GIVEN
    var services = new ServiceCollection();
    services.AddKeyedSingleton("chain1", (c, _) =>
      ActivatorUtilities.CreateInstance<A>(c,
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
    Assert.IsInstanceOf<B>(chain1.Next);
    Assert.IsInstanceOf<C1>(chain1.Next.Next);
    Assert.IsInstanceOf<D>(chain1.Next.Next!.Next);

    Assert.IsInstanceOf<B>(chain2.Next);
    Assert.IsInstanceOf<C2>(chain2.Next.Next);
    Assert.IsInstanceOf<D>(chain2.Next.Next!.Next);
    Assert.IsNull(chain2.Next.Next!.Next!.Next);
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
