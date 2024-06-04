using Lamar;

namespace DiFrameworkCons.MultipleRecipes.RecipeDifferencePropagation.DifferingByLiterals.Lamar;

public static class _1_KeyedRegistrations
{
  /// <summary>
  /// Surprisingly, the version of this example from MsDi file
  /// (one that combined keyed and keyless registrations of the same type)
  /// did not work with Lamar. I had to turn the registrations for a type
  /// to keyed to make it work.
  /// </summary>
  [Test]
  public static void ShouldResolveTwoSimilarObjectGraphsWithDifferentLeaves()
  {
    //GIVEN
    using var container = new Container(builder =>
    {
      const string firstKey = "first";
      const string secondKey = "second";
      builder.AddSingleton(x =>
        ActivatorUtilities.CreateInstance<World>(
          x,
          x.GetRequiredKeyedService<Character>(firstKey),
          x.GetRequiredKeyedService<Character>(secondKey)
        ));
      builder.AddKeyedSingleton(firstKey,
        (x, key) =>
          ActivatorUtilities.CreateInstance<Character>(x,
            x.GetRequiredKeyedService<Armor>(key),
            x.GetRequiredKeyedService<Sword>(key)));

      builder.AddKeyedSingleton(secondKey,
        (x, key) =>
          ActivatorUtilities.CreateInstance<Character>(x,
            x.GetRequiredKeyedService<Armor>(key),
            x.GetRequiredKeyedService<Sword>(key)));

      builder.AddKeyedSingleton(firstKey, (x, key) =>
        ActivatorUtilities.CreateInstance<Armor>(x,
          x.GetRequiredKeyedService<BreastPlate>(key)));
      builder.AddKeyedSingleton(secondKey, (x, key) =>
        ActivatorUtilities.CreateInstance<Armor>(x,
          x.GetRequiredKeyedService<BreastPlate>(key)));
      builder.AddTransient<Helmet>();
      builder.AddKeyedSingleton(firstKey,
        (x, key) => ActivatorUtilities.CreateInstance<BreastPlate>(x, 2));
      builder.AddKeyedSingleton(secondKey,
        (x, key) => ActivatorUtilities.CreateInstance<BreastPlate>(x, 4));
      builder.AddKeyedSingleton(firstKey, (x, key) => ActivatorUtilities.CreateInstance<Sword>(x, 4));
      builder.AddKeyedSingleton(secondKey, (x, key) => ActivatorUtilities.CreateInstance<Sword>(x, 6));
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