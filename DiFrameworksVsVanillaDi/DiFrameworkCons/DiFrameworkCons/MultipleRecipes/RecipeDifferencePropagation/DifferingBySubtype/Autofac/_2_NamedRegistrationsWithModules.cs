namespace DiFrameworkCons.MultipleRecipes.RecipeDifferencePropagation.DifferingBySubtype.Autofac;

public static class _2_NamedRegistrationsWithModules
{
  [Test]
  public static void ShouldResolveTwoSimilarObjectGraphsWithDifferentLeavesFromContainerModulesUsingAutofac()
  {
    //GIVEN
    var firstCategory = "hero";
    var secondCategory = "enemy";

    var builder = new ContainerBuilder();
    builder.RegisterType<World>()
      .As<World>()
      .WithParameter(
        (info, _) => info.Position == 0,
        (_, context) => context.ResolveNamed<Character>(firstCategory))
      .WithParameter(
        (info, _) => info.Position == 1,
        (_, context) => context.ResolveNamed<Character>(secondCategory))
      .SingleInstance();

    builder.RegisterModule(new SoldierAutofacModule<LongSword, ChainMail>(firstCategory));
    builder.RegisterModule(new SoldierAutofacModule<ShortSword, BreastPlate>(secondCategory));

    using var container = builder.Build();

    //WHEN
    var world = container.Resolve<World>();

    //THEN
    world.Enemy.Should().NotBeSameAs(world.Hero);
    world.Enemy.Armor.Should().NotBeSameAs(world.Hero.Armor);
    world.Enemy.Armor.Helmet.Should().NotBeSameAs(world.Hero.Armor.Helmet);

    world.Hero.Armor.BodyArmor.Should().BeOfType<ChainMail>();
    world.Enemy.Armor.BodyArmor.Should().BeOfType<BreastPlate>();
    world.Hero.Weapon.Should().BeOfType<LongSword>();
    world.Enemy.Weapon.Should().BeOfType<ShortSword>();
  }

  public class SoldierAutofacModule<THandWeapon, TBodyArmor> : Module
    where THandWeapon : IHandWeapon
    where TBodyArmor : IBodyArmor
  {
    private readonly string _category;

    public SoldierAutofacModule(string category)
    {
      _category = category;
    }

    protected override void Load(ContainerBuilder builder)
    {
      builder.RegisterType<Character>()
        .WithParameter(
          (info, _) => info.Position == 0,
          (_, context) => context.ResolveNamed<Armor>(_category)
        )
        .WithParameter(
          (info, _) => info.Position == 1,
          (_, context) => context.ResolveNamed<THandWeapon>(_category)
        )
        .Named<Character>(_category)
        .SingleInstance();

      builder.RegisterType<Armor>()
        .WithParameter(
          (info, _) => info.Position == 0,
          (_, context) => context.ResolveNamed<Helmet>(_category))
        .WithParameter(
          (info, _) => info.Position == 1,
          (_, context) => context.ResolveNamed<TBodyArmor>(_category))
        .Named<Armor>(_category)
        .SingleInstance();

      builder.RegisterType<THandWeapon>()
        .Named<THandWeapon>(_category)
        .SingleInstance();

      builder.RegisterType<Helmet>()
        .Named<Helmet>(_category)
        .SingleInstance();

      builder.RegisterType<TBodyArmor>()
        .Named<TBodyArmor>(_category)
        .SingleInstance();
    }
  }
}