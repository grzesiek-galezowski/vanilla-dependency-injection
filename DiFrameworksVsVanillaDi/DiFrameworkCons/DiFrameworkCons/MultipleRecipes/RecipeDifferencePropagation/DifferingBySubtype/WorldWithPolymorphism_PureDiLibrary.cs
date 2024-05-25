using Pure.DI;

namespace DiFrameworkCons.MultipleRecipes.RecipeDifferencePropagation.DifferingBySubtype;

static class WorldWithPolymorphism_PureDiLibrary
{
  [Test]
  public static void ShouldResolveTwoSimilarObjectGraphsWithDifferentLeavesFromVanillaDi()
  {
    //GIVEN
    var composition = new Composition16();

    //WHEN
    var world = composition.World;

    world.Enemy.Should().NotBeSameAs(world.Hero);
    world.Enemy.Armor.Should().NotBeSameAs(world.Hero.Armor);
    world.Enemy.Armor.Helmet.Should().NotBeSameAs(world.Hero.Armor.Helmet);

    world.Hero.Armor.BodyArmor.Should().BeOfType<ChainMail>();
    world.Enemy.Armor.BodyArmor.Should().BeOfType<BreastPlate>();
    world.Hero.Weapon.Should().BeOfType<LongSword>();
    world.Enemy.Weapon.Should().BeOfType<ShortSword>();
  }
}

partial class Composition16
{
  public void Setup()
  {
    DI.Setup(nameof(Composition16))
      .RootBind<World>("World").As(Lifetime.Singleton).To(context =>
      {
        context.Inject<Character>("first", out var hero);
        context.Inject<Character>("second", out var enemy);
        return new World(hero, enemy);
      })
      .Bind<Character>("first").As(Lifetime.Singleton).To(context =>
      {
        context.Inject<Armor>("first", out var armor);
        context.Inject<IHandWeapon>("first", out var weapon);
        return new Character(armor, weapon);
      })
      .Bind<Character>("second").As(Lifetime.Singleton).To(context =>
      {
        context.Inject<Armor>("second", out var armor);
        context.Inject<IHandWeapon>("second", out var weapon);
        return new Character(armor, weapon);
      })
      .Bind<Helmet>().As(Lifetime.Transient).To<Helmet>()
      .Bind<Armor>("first").As(Lifetime.Singleton).To(context =>
      {
        context.Inject<Helmet>(out var helmet);
        context.Inject<IBodyArmor>("first", out var bodyArmor);
        return new Armor(helmet, bodyArmor);
      })
      .Bind<Armor>("second").As(Lifetime.Singleton).To(context =>
      {
        context.Inject<Helmet>(out var helmet);
        context.Inject<IBodyArmor>("second", out var bodyArmor);
        return new Armor(helmet, bodyArmor);
      })
      .Bind<IBodyArmor>("first")
      .As(Lifetime.Singleton)
      .To<ChainMail>()
      .Bind<IBodyArmor>("second")
      .As(Lifetime.Singleton)
      .To<BreastPlate>()
      .Bind<IHandWeapon>("first")
      .As(Lifetime.Singleton)
      .To<LongSword>()
      .Bind<IHandWeapon>("second")
      .As(Lifetime.Singleton)
      .To<ShortSword>();
  }
}