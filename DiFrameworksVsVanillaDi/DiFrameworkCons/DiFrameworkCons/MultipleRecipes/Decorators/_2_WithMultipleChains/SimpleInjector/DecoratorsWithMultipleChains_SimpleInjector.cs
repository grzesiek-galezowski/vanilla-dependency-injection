using System.Collections.Generic;
using SimpleInjector;

namespace DiFrameworkCons.MultipleRecipes.Decorators._2_WithMultipleChains.SimpleInjector;

public static class DecoratorsWithMultipleChains_SimpleInjector
{
  /// <summary>
  /// There is a chance that this example can be implemented
  /// in a smarter way using the named registrations trick
  /// (https://docs.simpleinjector.org/en/latest/howto.html#resolve-instances-by-key)
  /// and conditional registrations, but I failed to find this way.
  /// </summary>
  [Test]
  public static void ShouldComposeVariousDecoratorConfigurations()
  {
    //GIVEN
    using var container = new Container();
    container.RegisterSingleton(
      () =>
        new Components
        {
          ["chain1"] = ActivatorUtilities.CreateInstance<A>(container,
            ActivatorUtilities.CreateInstance<B>(container,
              ActivatorUtilities.CreateInstance<C1>(container,
                ActivatorUtilities.CreateInstance<D>(container)))),
          ["chain2"] = ActivatorUtilities.CreateInstance<A>(container,
            ActivatorUtilities.CreateInstance<B>(container,
              ActivatorUtilities.CreateInstance<C2>(container,
                ActivatorUtilities.CreateInstance<D>(container))))
        }
    );

    //WHEN
    var chain1 = container.GetRequiredService<Components>().Resolve("chain1");
    var chain2 = container.GetRequiredService<Components>().Resolve("chain2");

    //THEN
    chain1.Next.Should().BeOfType<B>();
    chain1.Next.Next.Should().BeOfType<C1>();
    chain1.Next.Next!.Next.Should().BeOfType<D>();

    chain2.Next.Should().BeOfType<B>();
    chain2.Next.Next.Should().BeOfType<C2>();
    chain2.Next.Next!.Next.Should().BeOfType<D>();
    chain2.Next.Next!.Next!.Next.Should().BeNull();
  }

  private class Components
    : Dictionary<string, IComponent>
  {
    public IComponent Resolve(string name) => this[name];
  }
}