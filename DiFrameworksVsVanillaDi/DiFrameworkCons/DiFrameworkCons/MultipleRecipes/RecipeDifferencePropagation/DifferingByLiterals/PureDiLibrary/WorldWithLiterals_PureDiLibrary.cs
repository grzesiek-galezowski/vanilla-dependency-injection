using Pure.DI;

namespace DiFrameworkCons.MultipleRecipes.RecipeDifferencePropagation.DifferingByLiterals.PureDiLibrary;

static class WorldWithLiterals_PureDiLibrary
{
  [Test]
  public static void ShouldResolveTwoSimilarObjectGraphsWithDifferentLeavesFromVanillaDi()
  {
    //GIVEN
    var composition = new Composition15();

    //WHEN
    var world = composition.World;

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

partial class Composition15
{
  public void Setup()
  {
    DI.Setup(nameof(Composition15))
      .RootBind<World>("World").As(Lifetime.Singleton).To(context =>
      {
        context.Inject<Character>("first", out var hero);
        context.Inject<Character>("second", out var enemy);
        return new World(hero, enemy);
      })
      .Bind<Character>("first").As(Lifetime.Singleton).To(context =>
      {
        context.Inject<Armor>("first", out var armor);
        context.Inject<Sword>("first", out var sword);
        return new Character(armor, sword);
      })
      .Bind<Character>("second").As(Lifetime.Singleton).To(context =>
      {
        context.Inject<Armor>("second", out var armor);
        context.Inject<Sword>("second", out var sword);
        return new Character(armor, sword);
      })
      .Bind<Helmet>().As(Lifetime.Transient).To<Helmet>()
      .Bind<Armor>("first").As(Lifetime.Singleton).To(context =>
      {
        context.Inject<Helmet>(out var helmet);
        context.Inject<BreastPlate>("first", out var breastPlate);
        return new Armor(helmet, breastPlate);
      })
      .Bind<Armor>("second").As(Lifetime.Singleton).To(context =>
      {
        context.Inject<Helmet>(out var helmet);
        context.Inject<BreastPlate>("second", out var breastPlate);
        return new Armor(helmet, breastPlate);
      })
      .Bind<BreastPlate>("first")
      .As(Lifetime.Singleton)
      .To(_ => new BreastPlate(2))
      .Bind<BreastPlate>("second")
      .As(Lifetime.Singleton)
      .To(_ => new BreastPlate(4))
      .Bind<Sword>("first")
      .As(Lifetime.Singleton)
      .To(_ => new Sword(4))
      .Bind<Sword>("second")
      .As(Lifetime.Singleton)
      .To(_ => new Sword(6));
  }
}