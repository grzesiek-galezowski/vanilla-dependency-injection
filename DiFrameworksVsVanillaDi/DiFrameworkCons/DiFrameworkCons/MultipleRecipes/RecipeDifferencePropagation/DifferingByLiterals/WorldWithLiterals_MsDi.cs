using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DiFrameworkCons.MultipleRecipes.RecipeDifferencePropagation.DifferingByLiterals;

public static class WorldWithLiterals_MsDi
{
  [Test]
  public static void ShouldResolveTwoSimilarObjectGraphsWithDifferentLeaves()
  {
    //GIVEN
    var builder = new ServiceCollection();
    builder.AddSingleton(x =>
      ActivatorUtilities.CreateInstance<World>(
        x,
        x.GetRequiredService<Character>(),
        x.GetRequiredKeyedService<Character>("secondCharacter")
        ));
    builder.AddSingleton<Character>();
    builder.AddKeyedSingleton("secondCharacter",
        (x, key) =>
          ActivatorUtilities.CreateInstance<Character>(x,
              x.GetRequiredKeyedService<Armor>("secondArmor"),
              x.GetRequiredKeyedService<Sword>("secondSword")));

    builder.AddSingleton<Armor>();
    builder.AddKeyedSingleton("secondArmor", (x, key) =>
        ActivatorUtilities.CreateInstance<Armor>(x,
            x.GetRequiredKeyedService<BreastPlate>("secondBreastPlate")));
    builder.AddTransient<Helmet>();
    builder.AddSingleton(
      x => ActivatorUtilities.CreateInstance<BreastPlate>(x, 2));
    builder.AddKeyedSingleton("secondBreastPlate",
      (x, key) => ActivatorUtilities.CreateInstance<BreastPlate>(x, 4));
    builder.AddSingleton(x => ActivatorUtilities.CreateInstance<Sword>(x, 4));
    builder.AddKeyedSingleton("secondSword", (x, key) => ActivatorUtilities.CreateInstance<Sword>(x, 6));

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

  [Test]
  public static void ShouldResolveTwoSimilarObjectGraphsWithDifferentLeavesFromContainerModules()
  {
    var builder = new ServiceCollection();
    builder.AddSingleton(x =>
      ActivatorUtilities.CreateInstance<World>(
        x,
        x.GetRequiredKeyedService<Character>("first"),
        x.GetRequiredKeyedService<Character>("second")));
    builder.AddCharacter("first", 2, 4);
    builder.AddCharacter("second", 4, 6);

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

  [Test]
  public static void ShouldResolveTwoSimilarObjectGraphsWithDifferentLeavesUsingActivatorUtilities()
  {
    //GIVEN
    var serviceCollection = new ServiceCollection();
    serviceCollection
      .AddSingleton(x =>
        ActivatorUtilities.CreateInstance<World>(x,
          CreateCharacter(x, breastPlateDefense: 2, swordAttack: 4),
          CreateCharacter(x, breastPlateDefense: 4, swordAttack: 6)));

    using var container = serviceCollection.BuildServiceProvider();

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

  private static Character CreateCharacter(IServiceProvider x, int breastPlateDefense, int swordAttack)
  {
    return ActivatorUtilities.CreateInstance<Character>(x,
      ActivatorUtilities.CreateInstance<Armor>(x,
        ActivatorUtilities.CreateInstance<Helmet>(x),
        ActivatorUtilities.CreateInstance<BreastPlate>(x, breastPlateDefense)),
      ActivatorUtilities.CreateInstance<Sword>(x, swordAttack));
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