using SimpleInjector;

namespace DiFrameworkCons.MultipleRecipes.RecipeDifferencePropagation.DifferingByLiterals;

class WorldWithLiterals_SimpleInjector
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

  [Test]
  public static void ShouldResolveTwoSimilarObjectGraphsWithDifferentLeaves()
  {
    //GIVEN
    using var container = new Container();
    var firstCharacterName = "first";
    var secondCharacterName = "second";
    container.RegisterSingleton(() =>
      ActivatorUtilities.CreateInstance<World>(
        container,
        container.GetNamedService<Character>(firstCharacterName),
        container.GetNamedService<Character>(secondCharacterName)
        ));
    container.NamedRegistrations<Character>(c =>
    {
      c.RegisterSingleton(firstCharacterName, name =>
        ActivatorUtilities.CreateInstance<Character>(container,
          container.GetNamedService<Armor>(name),
          container.GetNamedService<Sword>(name))
      );
      c.RegisterSingleton(secondCharacterName, name =>
        ActivatorUtilities.CreateInstance<Character>(container,
          container.GetNamedService<Armor>(name),
          container.GetNamedService<Sword>(name))
      );
    });
    container.NamedRegistrations<Armor>(c =>
    {
      c.RegisterSingleton(firstCharacterName, name =>
        ActivatorUtilities.CreateInstance<Armor>(container,
          container.GetNamedService<BreastPlate>(name)));
      c.RegisterSingleton(secondCharacterName, name =>
        ActivatorUtilities.CreateInstance<Armor>(container,
          container.GetNamedService<BreastPlate>(name)));
    });
    container.Register<Helmet>();
    container.NamedRegistrations<BreastPlate>(c =>
    {
      c.RegisterSingleton(firstCharacterName, _ =>
        ActivatorUtilities.CreateInstance<BreastPlate>(container, 2));
      c.RegisterSingleton(secondCharacterName, _ =>
        ActivatorUtilities.CreateInstance<BreastPlate>(container, 4));
    });
    container.NamedRegistrations<Sword>(c =>
    {
      c.RegisterSingleton(firstCharacterName, _ =>
        ActivatorUtilities.CreateInstance<Sword>(container, 4));
      c.RegisterSingleton(secondCharacterName, _ =>
        ActivatorUtilities.CreateInstance<Sword>(container, 6));
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