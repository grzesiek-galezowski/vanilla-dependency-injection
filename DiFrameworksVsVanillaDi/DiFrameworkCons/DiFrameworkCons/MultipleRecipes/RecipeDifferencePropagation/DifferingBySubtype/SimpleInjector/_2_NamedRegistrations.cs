using DiFrameworkCons.SimpleInjectorExtensions;
using SimpleInjector;

namespace DiFrameworkCons.MultipleRecipes.RecipeDifferencePropagation.DifferingBySubtype.SimpleInjector;

public static class _2_NamedRegistrations
{
  [Test]
  public static void ShouldResolveTwoSimilarObjectGraphsWithDifferentLeaves()
  {
    //GIVEN
    using var container = new Container();
    var firstCharacterKey = "first";
    var secondCharacterKey = "second";
    container.RegisterSingleton(() =>
      ActivatorUtilities.CreateInstance<World>(
        container,
        container.GetNamedService<Character>(firstCharacterKey),
        container.GetNamedService<Character>(secondCharacterKey)
      ));
    container.NamedRegistrations<Character>(c =>
    {
      c.RegisterSingleton(firstCharacterKey, name =>
        ActivatorUtilities.CreateInstance<Character>(container,
          container.GetNamedService<Armor>(name),
          container.GetNamedService<IHandWeapon>(name))
      );
      c.RegisterSingleton(secondCharacterKey, name =>
        ActivatorUtilities.CreateInstance<Character>(container,
          container.GetNamedService<Armor>(name),
          container.GetNamedService<IHandWeapon>(name))
      );
    });
    container.NamedRegistrations<Armor>(c =>
    {
      c.RegisterSingleton(firstCharacterKey, name =>
        ActivatorUtilities.CreateInstance<Armor>(container,
          container.GetNamedService<IBodyArmor>(name)));
      c.RegisterSingleton(secondCharacterKey, name =>
        ActivatorUtilities.CreateInstance<Armor>(container,
          container.GetNamedService<IBodyArmor>(name)));
    });
    container.Register<Helmet>();
    container.NamedRegistrations<IBodyArmor>(c =>
    {
      c.RegisterSingleton<ChainMail>(firstCharacterKey);
      c.RegisterSingleton<BreastPlate>(secondCharacterKey);
    });
    container.NamedRegistrations<IHandWeapon>(c =>
    {
      c.RegisterSingleton<LongSword>(firstCharacterKey);
      c.RegisterSingleton<ShortSword>(secondCharacterKey);
    });

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