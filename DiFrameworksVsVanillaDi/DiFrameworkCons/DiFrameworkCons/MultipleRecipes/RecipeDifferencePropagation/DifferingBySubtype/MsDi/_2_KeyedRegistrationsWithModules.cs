using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DiFrameworkCons.MultipleRecipes.RecipeDifferencePropagation.DifferingBySubtype.MsDi;

public static class _2_KeyedRegistrationsWithModules
{
  [Test]
  public static void ShouldResolveTwoSimilarObjectGraphsWithDifferentLeavesFromContainerModules()
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