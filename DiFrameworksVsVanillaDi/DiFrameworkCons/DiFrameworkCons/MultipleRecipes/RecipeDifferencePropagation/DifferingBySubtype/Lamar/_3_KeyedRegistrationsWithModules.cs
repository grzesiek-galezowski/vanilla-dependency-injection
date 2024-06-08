using Lamar;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DiFrameworkCons.MultipleRecipes.RecipeDifferencePropagation.DifferingBySubtype.Lamar;

public static class _3_KeyedRegistrationsWithModules
{
  [Test]
  public static void ShouldResolveTwoSimilarObjectGraphsWithDifferentLeavesFromContainerModules()
  {
    //GIVEN
    using var container = new Container(builder =>
    {
      builder.AddSingleton(c => ActivatorUtilities.CreateInstance<World>(
        c,
        c.GetRequiredKeyedService<Character>("hero"),
        c.GetRequiredKeyedService<Character>("enemy")));
      SoldierLamarModule<LongSword, ChainMail>.RegisterIn(builder, "hero");
      SoldierLamarModule<ShortSword, BreastPlate>.RegisterIn(builder, "enemy");
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

  private static class SoldierLamarModule<THandWeapon, TBodyArmor>
    where THandWeapon : class, IHandWeapon
    where TBodyArmor : class, IBodyArmor
  {
    public static void RegisterIn(
      IServiceCollection builder, string category)
    {
      builder.AddKeyedSingleton(
        category,
        (ctx, o) => ActivatorUtilities.CreateInstance<Character>(
          ctx,
          ctx.GetRequiredKeyedService<Armor>(o),
          ctx.GetRequiredKeyedService<THandWeapon>(o)));
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