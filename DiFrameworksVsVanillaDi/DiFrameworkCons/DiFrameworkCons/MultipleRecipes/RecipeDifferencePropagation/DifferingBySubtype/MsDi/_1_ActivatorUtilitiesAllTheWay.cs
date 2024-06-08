namespace DiFrameworkCons.MultipleRecipes.RecipeDifferencePropagation.DifferingBySubtype.MsDi;

public static class _1_ActivatorUtilitiesAllTheWay
{
  [Test]
  public static void ShouldResolveTwoSimilarObjectGraphsWithDifferentLeavesFromActivatorUtilities()
  {
    //GIVEN
    var builder = new ServiceCollection();

    //Works only if created objects are transients
    //and does not allow reusing the nested instances created by ActivatorUtilities
    builder.AddSingleton(c => ActivatorUtilities.CreateInstance<World>(c,
      ActivatorUtilities.CreateInstance<Character>(c,
        ActivatorUtilities.CreateInstance<Armor>(c,
          ActivatorUtilities.CreateInstance<ChainMail>(c)),
        ActivatorUtilities.CreateInstance<LongSword>(c)),
      ActivatorUtilities.CreateInstance<Character>(c,
        ActivatorUtilities.CreateInstance<Armor>(c,
          ActivatorUtilities.CreateInstance<BreastPlate>(c)),
        ActivatorUtilities.CreateInstance<ShortSword>(c))));
    builder.AddTransient<Helmet>();

    using var container = builder.BuildServiceProvider();

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