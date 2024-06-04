namespace DiFrameworkCons.MultipleRecipes.RecipeDifferencePropagation.DifferingByLiterals.MsDi;

public static class _3_ActivatorUtilitiesAllTheWay
{
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

