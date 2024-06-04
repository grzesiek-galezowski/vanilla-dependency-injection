using SimpleInjector;

namespace DiFrameworkCons.MultipleRecipes.RecipeDifferencePropagation.DifferingByLiterals.SimpleInjector;

class _2_ActivatorUtilitiesAllTheWay
{
  [Test]
  public static void ShouldResolveTwoSimilarObjectGraphsWithDifferentLeavesUsingActivatorUtilities()
  {
    //GIVEN
    using var container = new Container();
    container
      .Register(() =>
        ActivatorUtilities.CreateInstance<World>(container,
          CreateCharacter(container, breastPlateDefense: 2, swordAttack: 4),
          CreateCharacter(container, breastPlateDefense: 4, swordAttack: 6)),
        Lifestyle.Singleton);

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