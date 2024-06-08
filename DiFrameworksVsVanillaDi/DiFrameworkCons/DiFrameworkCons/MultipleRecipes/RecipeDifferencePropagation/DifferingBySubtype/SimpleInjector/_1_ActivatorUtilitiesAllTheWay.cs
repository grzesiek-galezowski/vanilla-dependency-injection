using SimpleInjector;

namespace DiFrameworkCons.MultipleRecipes.RecipeDifferencePropagation.DifferingBySubtype.SimpleInjector;

public static class _1_ActivatorUtilitiesAllTheWay
{
  [Test]
  public static void ShouldResolveTwoSimilarObjectGraphsWithDifferentLeavesFromActivatorUtilities()
  {
    //GIVEN
    using var container = new Container();

    //Works only if created objects are transients
    //and does not allow reusing the nested instances created by ActivatorUtilities
    container.RegisterSingleton(() => ActivatorUtilities.CreateInstance<World>(container,
      ActivatorUtilities.CreateInstance<Character>(container,
        ActivatorUtilities.CreateInstance<Armor>(container,
          ActivatorUtilities.CreateInstance<ChainMail>(container)),
        ActivatorUtilities.CreateInstance<LongSword>(container)),
      ActivatorUtilities.CreateInstance<Character>(container,
        ActivatorUtilities.CreateInstance<Armor>(container,
          ActivatorUtilities.CreateInstance<BreastPlate>(container)),
        ActivatorUtilities.CreateInstance<ShortSword>(container))));
    container.Register<Helmet>();

    //WHEN
    var world = container.GetRequiredService<World>();

    //THEN
    world.Enemy.Should().NotBeSameAs(world.Hero);
    world.Enemy.Armor.Should().NotBeSameAs(world.Hero.Armor);
    world.Enemy.Armor.Helmet.Should().NotBeSameAs(world.Hero.Armor.Helmet);

    world.Hero.Armor.BodyArmor.Should().BeOfType<ChainMail>();
    world.Enemy.Armor.BodyArmor.Should().BeOfType<BreastPlate>();
    world.Hero.Weapon.Should().BeOfType<LongSword>();
    world.Enemy.Weapon.Should().BeOfType<ShortSword>();
  }
}