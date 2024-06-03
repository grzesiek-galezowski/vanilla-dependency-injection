using DiFrameworkCons.SimpleInjectorExtensions;
using SimpleInjector;

namespace DiFrameworkCons.MultipleRecipes.Decorators._2_WithMultipleChains.SimpleInjector;

public static class _2_KeyedRegistrations
{
  /// <summary>
  /// An alternative is to use keyed registrations.
  /// Note that starting from the difference in the chains, the requirement
  /// to use keys propagates.
  /// </summary>
  [Test]
  public static void ShouldComposeVariousDecoratorConfigurationsWithKeyedDependencies()
  {
    //GIVEN
    var container = new Container();
    container.Register<IComponent, D>();

    container.NamedRegistrations<IComponent>(c =>
    {
      c.Register<C1>("c1");
      c.Register<C2>("c2");
      c.Register("chain1B",
        _ => ActivatorUtilities.CreateInstance<B>(
          container, container.GetNamedService<IComponent>("c1")));
      c.Register("chain2B",
        _ => ActivatorUtilities.CreateInstance<B>(
          container, container.GetNamedService<IComponent>("c2")));
      c.RegisterSingleton("chain1A",
        _ => ActivatorUtilities.CreateInstance<A>(
          container, container.GetNamedService<IComponent>("chain1B")));
      c.RegisterSingleton("chain2A",
        _ => ActivatorUtilities.CreateInstance<A>(
          container, container.GetNamedService<IComponent>("chain2B")));
    });

    //WHEN
    var chain1 = container.GetNamedService<IComponent>("chain1A");
    var chain2 = container.GetNamedService<IComponent>("chain2A");

    //THEN
    chain1.Next.Should().BeOfType<B>();
    chain1.Next!.Next.Should().BeOfType<C1>();
    chain1.Next.Next!.Next.Should().BeOfType<D>();

    chain2.Next.Should().BeOfType<B>();
    chain2.Next!.Next.Should().BeOfType<C2>();
    chain2.Next.Next!.Next.Should().BeOfType<D>();
    chain2.Next.Next!.Next!.Next.Should().BeNull();
  }
}