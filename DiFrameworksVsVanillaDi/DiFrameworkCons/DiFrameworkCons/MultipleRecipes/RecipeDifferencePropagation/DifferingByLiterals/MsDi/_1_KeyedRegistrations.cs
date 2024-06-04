namespace DiFrameworkCons.MultipleRecipes.RecipeDifferencePropagation.DifferingByLiterals.MsDi;

public static class _1_KeyedRegistrations
{
  [Test]
  public static void ShouldResolveTwoSimilarObjectGraphsWithDifferentLeaves()
  {
    //GIVEN
    var builder = new ServiceCollection();
    var secondCharacterKey = "second";
    builder.AddSingleton(x =>
      ActivatorUtilities.CreateInstance<World>(
        x,
        x.GetRequiredService<Character>(),
        x.GetRequiredKeyedService<Character>(secondCharacterKey)
      ));
    builder.AddSingleton<Character>();
    builder.AddKeyedSingleton(secondCharacterKey,
      (x, key) =>
        ActivatorUtilities.CreateInstance<Character>(x,
          x.GetRequiredKeyedService<Armor>(key),
          x.GetRequiredKeyedService<Sword>(key)));
    builder.AddSingleton<Armor>();
    builder.AddKeyedSingleton(secondCharacterKey, (x, key) =>
      ActivatorUtilities.CreateInstance<Armor>(x,
        x.GetRequiredKeyedService<BreastPlate>(key)));
    builder.AddTransient<Helmet>();
    builder.AddSingleton(
      x => ActivatorUtilities.CreateInstance<BreastPlate>(x, 2));
    builder.AddKeyedSingleton(secondCharacterKey,
      (x, key) => ActivatorUtilities.CreateInstance<BreastPlate>(x, 4));
    builder.AddSingleton(x => ActivatorUtilities.CreateInstance<Sword>(x, 4));
    builder.AddKeyedSingleton(secondCharacterKey, (x, key) => ActivatorUtilities.CreateInstance<Sword>(x, 6));

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