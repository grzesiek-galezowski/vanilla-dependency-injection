using Lamar;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DiFrameworkCons.MultipleRecipes.RecipeDifferencePropagation.DifferingBySubtype;

public static class WorldWithPolymorphism_Lamar
{
  [Test]
  public static void ShouldResolveTwoSimilarObjectGraphsWithDifferentLeavesFromActivatorUtilities()
  {
    //GIVEN
    using var container = new Container(builder =>
    {
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

  [Test]
  public static void ShouldResolveTwoSimilarObjectGraphsWithDifferentLeavesUsingConstructorSelection()
  {
    //GIVEN
    var container = new Container(builder =>
    {
      var firstKey = "first";
      var secondKey = "second";
      builder.ForConcreteType<World>()
        .Configure
        .Ctor<Character>("Hero").IsNamedInstance(firstKey)
        .Ctor<Character>("Enemy").IsNamedInstance(secondKey)
        .Singleton();

      builder.ForConcreteType<Character>()
        .Configure
        .Ctor<Armor>().IsNamedInstance(firstKey)
        .Ctor<IHandWeapon>().IsNamedInstance(firstKey)
        .Singleton().Named(firstKey);

      builder.ForConcreteType<Character>()
        .Configure
        .Ctor<Armor>().IsNamedInstance(secondKey)
        .Ctor<IHandWeapon>().IsNamedInstance(secondKey)
        .Singleton().Named(secondKey);

      builder.ForConcreteType<Armor>()
        .Configure
        .Ctor<IBodyArmor>().IsNamedInstance(firstKey)
        .Singleton().Named(firstKey);

      builder.ForConcreteType<Armor>()
        .Configure
        .Ctor<IBodyArmor>().IsNamedInstance(secondKey)
        .Singleton().Named(secondKey);

      builder.For<Helmet>().Use<Helmet>().Transient();

      builder.For<IBodyArmor>().Use<ChainMail>()
        .Singleton().Named(firstKey);
      builder.For<IBodyArmor>().Use<BreastPlate>()
        .Singleton().Named(secondKey);

      builder.For<IHandWeapon>().Use<LongSword>()
        .Singleton().Named(firstKey);
      builder.For<IHandWeapon>().Use<ShortSword>()
        .Singleton().Named(secondKey);
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