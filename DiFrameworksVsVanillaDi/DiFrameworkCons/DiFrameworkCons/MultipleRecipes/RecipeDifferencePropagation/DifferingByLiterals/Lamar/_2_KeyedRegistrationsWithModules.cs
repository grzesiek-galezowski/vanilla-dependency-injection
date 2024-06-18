using Lamar;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DiFrameworkCons.MultipleRecipes.RecipeDifferencePropagation.DifferingByLiterals.Lamar;

public static class _2_KeyedRegistrationsWithModules
{
  /// <summary>
  /// While Lamar supports modules (called registries),
  /// it does not seem to support
  /// </summary>
  [Test]
  public static void ShouldResolveTwoSimilarObjectGraphsWithDifferentLeavesFromContainerModules()
  {
    using var container = new Container(builder =>
    {
      builder.AddSingleton(x =>
        ActivatorUtilities.CreateInstance<World>(
          x,
          x.GetRequiredKeyedService<Character>("first"),
          x.GetRequiredKeyedService<Character>("second")));
      builder.IncludeRegistry<FirstCharacterRegistry>();
      builder.IncludeRegistry<SecondCharacterRegistry>();
    });

    //WHEN
    var world = container.GetRequiredService<World>();

    //THEN
    world.Enemy.Should().NotBeSameAs(world.Hero);
    world.Enemy.Armor.Should().NotBeSameAs(world.Hero.Armor);
    world.Enemy.Armor.Helmet.Should().NotBeSameAs(world.Hero.Armor.Helmet);
    world.Enemy.Armor.BreastPlate.Should().NotBeSameAs(world.Hero.Armor.BreastPlate);
    world.Enemy.Armor.BreastPlate.Defense.Should().NotBe(world.Hero.Armor.BreastPlate.Defense);
    world.Enemy.Sword.Should().NotBeSameAs(world.Hero.Sword);

    world.Hero.Sword.Attack.Should().Be(4);
    world.Hero.Armor.BreastPlate.Defense.Should().Be(2);
    world.Enemy.Sword.Attack.Should().Be(6);
    world.Enemy.Armor.BreastPlate.Defense.Should().Be(4);
  }
}


// While Lamar supports modules and even passing module instances,
// it seems to not support two instances of the same registry,
// hence the subtypes:

file class FirstCharacterRegistry() : CharacterWithLiteralsRegistry("first", 2, 4);
file class SecondCharacterRegistry() : CharacterWithLiteralsRegistry("second", 4, 6);

file class CharacterWithLiteralsRegistry : ServiceRegistry
{
  protected CharacterWithLiteralsRegistry(
    string prefix,
    int breastPlateDefense,
    int swordAttack)
  {
    this.AddKeyedSingleton(prefix,
      (x, key) =>
        ActivatorUtilities.CreateInstance<Character>(x,
          x.GetRequiredKeyedService<Armor>(key),
          x.GetRequiredKeyedService<Sword>(key)));
    this.AddKeyedSingleton(prefix, (x, key) =>
      ActivatorUtilities.CreateInstance<Armor>(x,
        x.GetRequiredKeyedService<BreastPlate>(key)));
    this.TryAddTransient<Helmet>();
    this.AddKeyedSingleton(prefix,
      (x, key) => ActivatorUtilities.CreateInstance<BreastPlate>(x, breastPlateDefense));
    this.AddKeyedSingleton(prefix,
      (x, key) => ActivatorUtilities.CreateInstance<Sword>(x, swordAttack));
  }
}
