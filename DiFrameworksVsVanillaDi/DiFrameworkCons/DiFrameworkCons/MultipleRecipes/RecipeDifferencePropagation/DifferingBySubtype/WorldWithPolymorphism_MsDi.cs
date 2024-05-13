using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DiFrameworkCons.MultipleRecipes.RecipeDifferencePropagation.DifferingBySubtype;

public static class WorldWithPolymorphism_MsDi
{
  [Test]
  public static void ShouldResolveTwoSimilarObjectGraphsWithDifferentLeavesFromMsDiAndActivatorUtilities()
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

  [Test]
  public static void ShouldResolveTwoSimilarObjectGraphsWithDifferentLeavesFromContainerModulesUsingMsDi()
  {
    //GIVEN
    var builder = new ServiceCollection();
    builder.AddSingleton(c => ActivatorUtilities.CreateInstance<World>(
      c,
      c.GetRequiredKeyedService<Character>("hero"),
      c.GetRequiredKeyedService<Character>("enemy")));

    SoldierMsDiModule<LongSword, ChainMail>.RegisterIn(builder, "hero");
    SoldierMsDiModule<ShortSword, BreastPlate>.RegisterIn(builder, "enemy");

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

  public static class SoldierMsDiModule<THandWeapon, TBodyArmor>
    where THandWeapon : class, IHandWeapon
    where TBodyArmor : class, IBodyArmor
  {
    public static void RegisterIn(
      ServiceCollection builder, string category)
    {
      builder.AddKeyedSingleton(
        category,
        (ctx, o) => ActivatorUtilities.CreateInstance<Character>(
          ctx,
          ctx.GetRequiredKeyedService<Armor>(category),
          ctx.GetRequiredKeyedService<THandWeapon>(category)));
      builder.AddKeyedSingleton(
        category,
        (ctx, o) => ActivatorUtilities.CreateInstance<Armor>(
          ctx, ctx.GetRequiredKeyedService<TBodyArmor>(category)));

      builder.TryAddTransient<Helmet>();
      builder.AddKeyedSingleton<TBodyArmor>(category);
      builder.AddKeyedSingleton<THandWeapon>(category);
    }
  }
}