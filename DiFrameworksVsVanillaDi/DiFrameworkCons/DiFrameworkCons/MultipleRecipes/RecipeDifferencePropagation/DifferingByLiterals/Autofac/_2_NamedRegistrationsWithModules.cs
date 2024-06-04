namespace DiFrameworkCons.MultipleRecipes.RecipeDifferencePropagation.DifferingByLiterals.Autofac;

public static class _2_NamedRegistrationsWithModules
{
  [Test]
  public static void ShouldResolveTwoSimilarObjectGraphsWithDifferentLeavesFromContainerModules()
  {
    //GIVEN
    var firstCategory = Guid.NewGuid().ToString();
    var secondCategory = Guid.NewGuid().ToString();

    var builder = new ContainerBuilder();
    builder.RegisterType<World>()
      .As<World>()
      .WithParameter(
        (info, _) => info.Position == 0,
        (_, context) => context.ResolveNamed<Character>($"{firstCategory}Character"))
      .WithParameter(
        (info, _) => info.Position == 1,
        (_, context) => context.ResolveNamed<Character>($"{secondCategory}Character"))
      .SingleInstance();

    builder.RegisterModule(new SoldierModule(4, 2, firstCategory));
    builder.RegisterModule(new SoldierModule(6, 4, secondCategory));

    using var container = builder.Build();

    //WHEN
    var world = container.Resolve<World>();

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

  public class SoldierModule : Module
  {
    private readonly int _swordAttack;
    private readonly int _breastplateDefense;
    private readonly string _category;

    public SoldierModule(int swordAttack, int breastplateDefense, string category)
    {
      _swordAttack = swordAttack;
      _breastplateDefense = breastplateDefense;
      _category = category;
    }

    protected override void Load(ContainerBuilder builder)
    {
      builder.RegisterType<Character>()
        .WithParameter(
          (info, _) => info.Position == 0,
          (_, context) => context.ResolveNamed<Armor>($"{_category}Armor")
        )
        .WithParameter(
          (info, _) => info.Position == 1,
          (_, context) => context.ResolveNamed<Sword>($"{_category}Sword")
        )
        .Named<Character>($"{_category}Character")
        .SingleInstance();

      builder.RegisterType<Armor>()
        .WithParameter(
          (info, _) => info.Position == 0,
          (_, context) => context.ResolveNamed<Helmet>($"{_category}Helmet"))
        .WithParameter(
          (info, _) => info.Position == 1,
          (_, context) => context.ResolveNamed<BreastPlate>($"{_category}BreastPlate"))
        .Named<Armor>($"{_category}Armor")
        .SingleInstance();

      builder.RegisterType<Sword>()
        .Named<Sword>($"{_category}Sword")
        .WithParameter("Attack", _swordAttack)
        .SingleInstance();
      builder.RegisterType<Helmet>()
        .Named<Helmet>($"{_category}Helmet")
        .SingleInstance();
      builder.RegisterType<BreastPlate>()
        .Named<BreastPlate>($"{_category}BreastPlate")
        .WithParameter("Defense", _breastplateDefense)
        .SingleInstance();

    }
  }
}