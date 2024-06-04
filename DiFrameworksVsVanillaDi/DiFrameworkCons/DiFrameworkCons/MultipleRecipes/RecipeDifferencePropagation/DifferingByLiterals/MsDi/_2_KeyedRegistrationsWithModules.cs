using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DiFrameworkCons.MultipleRecipes.RecipeDifferencePropagation.DifferingByLiterals.MsDi;

public static class _2_KeyedRegistrationsWithModules
{
  [Test]
  public static void ShouldResolveTwoSimilarObjectGraphsWithDifferentLeavesFromContainerModules()
  {
    var builder = new ServiceCollection();
    builder.AddSingleton(x =>
      ActivatorUtilities.CreateInstance<World>(
        x,
        x.GetRequiredKeyedService<Character>("first"),
        x.GetRequiredKeyedService<Character>("second")));
    CharacterWithLiteralsExtensions.AddCharacter(builder, "first", 2, 4);
    CharacterWithLiteralsExtensions.AddCharacter(builder, "second", 4, 6);

    using var container = builder.BuildServiceProvider();

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

file static class CharacterWithLiteralsExtensions
{
  public static void AddCharacter(
    this ServiceCollection builder,
    string prefix,
    int breastPlateDefense,
    int swordAttack)
  {
    builder.AddKeyedSingleton(prefix,
      (x, key) =>
        ActivatorUtilities.CreateInstance<Character>(x,
          x.GetRequiredKeyedService<Armor>(key),
          x.GetRequiredKeyedService<Sword>(key)));
    builder.AddKeyedSingleton(prefix, (x, key) =>
      ActivatorUtilities.CreateInstance<Armor>(x,
        x.GetRequiredKeyedService<BreastPlate>(key)));
    builder.TryAddTransient<Helmet>();
    builder.AddKeyedSingleton(prefix,
      (x, key) => ActivatorUtilities.CreateInstance<BreastPlate>(x, breastPlateDefense));
    builder.AddKeyedSingleton(prefix, (x, key) => ActivatorUtilities.CreateInstance<Sword>(x, swordAttack));
  }
}