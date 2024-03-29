namespace DiFrameworkCons;

//todo add descriptions
class MultipleObjectOfSameTypeConfiguredDifferentlyAndNamingPropagation2
{
  [Test]
  public void ShouldResolveTwoSimilarObjectGraphsWithDifferentLeavesFromVanillaDi()
  {
    var world = new World(
      new Character(
        new Armor(
          new Helmet(),
          new ChainMail()),
        new LongSword()),
      new Character(
        new Armor(
          new Helmet(),
          new BreastPlate()),
        new ShortSword()));

    Assert.AreNotSame(world.Hero, world.Enemy);
    Assert.AreNotSame(world.Hero.Armor, world.Enemy.Armor);
    Assert.AreNotSame(world.Hero.Armor.Helmet, world.Enemy.Armor.Helmet);

    Assert.IsInstanceOf<ChainMail>(world.Hero.Armor.BodyArmor);
    Assert.IsInstanceOf<BreastPlate>(world.Enemy.Armor.BodyArmor);
    Assert.IsInstanceOf<LongSword>(world.Hero.Weapon);
    Assert.IsInstanceOf<ShortSword>(world.Enemy.Weapon);
  }

  [Test]
  public void ShouldResolveTwoSimilarObjectGraphsWithDifferentLeavesFromVanillaDiDried()
  {
    //GIVEN
    var world = new World(
      Soldier(new ChainMail(), new LongSword()),
      Soldier(new BreastPlate(), new ShortSword()));

    //THEN
    Assert.AreNotSame(world.Hero, world.Enemy);
    Assert.AreNotSame(world.Hero.Armor, world.Enemy.Armor);
    Assert.AreNotSame(world.Hero.Armor.Helmet, world.Enemy.Armor.Helmet);

    Assert.IsInstanceOf<ChainMail>(world.Hero.Armor.BodyArmor);
    Assert.IsInstanceOf<BreastPlate>(world.Enemy.Armor.BodyArmor);
    Assert.IsInstanceOf<LongSword>(world.Hero.Weapon);
    Assert.IsInstanceOf<ShortSword>(world.Enemy.Weapon);
  }

  [Test]
  public void ShouldResolveTwoSimilarObjectGraphsWithDifferentLeavesFromAutofacContainer()
  {
    //GIVEN
    var builder = new ContainerBuilder();
    builder.RegisterType<World>()
      .WithParameter(
        (info, _) => info.Position == 1,
        (_, context) => context.ResolveNamed<Character>("secondCharacter"))
      .SingleInstance();

    builder.RegisterType<Character>()
      .WithParameter(
        (info, _) => info.Position == 1,
        (_, context) => context.Resolve<LongSword>()
      )
      .SingleInstance();

    builder.RegisterType<Character>()
      .WithParameter(
        (info, _) => info.Position == 0,
        (_, context) => context.ResolveNamed<Armor>("secondArmor")
      )
      .WithParameter(
        (info, _) => info.Position == 1,
        (_, context) => context.Resolve<ShortSword>()
      )
      .Named<Character>("secondCharacter")
      .SingleInstance();

    builder.RegisterType<Armor>()
      .WithParameter(
        (info, _) => info.Position == 1,
        (_, context) => context.Resolve<ChainMail>())
      .SingleInstance();

    builder.RegisterType<Armor>()
      .WithParameter(
        (info, _) => info.Position == 1,
        (_, context) => context.Resolve<BreastPlate>())
      .Named<Armor>("secondArmor")
      .SingleInstance();

    builder.RegisterType<Helmet>().InstancePerDependency();

    builder.RegisterType<ChainMail>().InstancePerDependency();
    builder.RegisterType<BreastPlate>().InstancePerDependency();
    builder.RegisterType<LongSword>().InstancePerDependency();
    builder.RegisterType<ShortSword>().InstancePerDependency();
    using var container = builder.Build();

    //WHEN
    var world = container.Resolve<World>();

    //THEN
    Assert.AreNotSame(world.Hero, world.Enemy);
    Assert.AreNotSame(world.Hero.Armor, world.Enemy.Armor);
    Assert.AreNotSame(world.Hero.Armor.Helmet, world.Enemy.Armor.Helmet);

    Assert.IsInstanceOf<ChainMail>(world.Hero.Armor.BodyArmor);
    Assert.IsInstanceOf<BreastPlate>(world.Enemy.Armor.BodyArmor);
    Assert.IsInstanceOf<LongSword>(world.Hero.Weapon);
    Assert.IsInstanceOf<ShortSword>(world.Enemy.Weapon);
  }

  [Test]
  public void ShouldResolveTwoSimilarObjectGraphsWithDifferentLeavesFromMsDi()
  {
    //GIVEN
    var builder = new ServiceCollection();
    builder.AddSingleton(c => ActivatorUtilities.CreateInstance<World>(
      c,
      c.GetRequiredService<Character>(),
      c.GetRequiredKeyedService<Character>("secondCharacter")));
    builder.AddSingleton(
      ctx => ActivatorUtilities.CreateInstance<Character>(
        ctx,
        ctx.GetRequiredService<LongSword>()));
    builder.AddKeyedSingleton(
      "secondCharacter",
      (ctx, o) => ActivatorUtilities.CreateInstance<Character>(
        ctx,
        ctx.GetRequiredKeyedService<Armor>("secondArmor"),
        ctx.GetRequiredService<ShortSword>()));
    builder.AddSingleton(ctx => ActivatorUtilities.CreateInstance<Armor>(
      ctx,
      ctx.GetRequiredService<ChainMail>()));
    builder.AddKeyedSingleton(
      "secondArmor",
      (ctx, o) => ActivatorUtilities.CreateInstance<Armor>(
        ctx, ctx.GetRequiredService<BreastPlate>()));
    builder.AddTransient<Helmet>();
    builder.AddSingleton<ChainMail>();
    builder.AddSingleton<BreastPlate>();
    builder.AddSingleton<LongSword>();
    builder.AddSingleton<ShortSword>();
    using var container = builder.BuildServiceProvider();

    //WHEN
    var world = container.GetRequiredService<World>();

    //THEN
    Assert.AreNotSame(world.Hero, world.Enemy);
    Assert.AreNotSame(world.Hero.Armor, world.Enemy.Armor);
    Assert.AreNotSame(world.Hero.Armor.Helmet, world.Enemy.Armor.Helmet);

    Assert.IsInstanceOf<ChainMail>(world.Hero.Armor.BodyArmor);
    Assert.IsInstanceOf<BreastPlate>(world.Enemy.Armor.BodyArmor);
    Assert.IsInstanceOf<LongSword>(world.Hero.Weapon);
    Assert.IsInstanceOf<ShortSword>(world.Enemy.Weapon);
  }

  //[Test]
  //public void ShouldResolveTwoSimilarObjectGraphsWithDifferentLeavesFromContainerModules()
  //{
  //  //GIVEN
  //  var firstCategory = Guid.NewGuid().ToString();
  //  var secondCategory = Guid.NewGuid().ToString();
  //
  //  var builder = new ContainerBuilder();
  //  builder.RegisterType<World>()
  //    .As<World>()
  //    .WithParameter(
  //      (info, _) => info.Position == 0,
  //      (_, context) => context.ResolveNamed<Character>($"{firstCategory}Character"))
  //    .WithParameter(
  //      (info, _) => info.Position == 1,
  //      (_, context) => context.ResolveNamed<Character>($"{secondCategory}Character"))
  //    .SingleInstance();
  //
  //  builder.RegisterModule(new SoldierModule(4, 2, firstCategory));
  //  builder.RegisterModule(new SoldierModule(6, 4, secondCategory));
  //
  //  using var container = builder.Build();
  //
  //  //WHEN
  //  var world = container.Resolve<World>();
  //
  //  //THEN
  //  Assert.AreNotSame(world.Hero, world.Enemy);
  //  Assert.AreNotSame(world.Hero.Armor, world.Enemy.Armor);
  //  Assert.AreNotSame(world.Hero.Armor.Helmet, world.Enemy.Armor.Helmet);
  //  Assert.AreNotSame(world.Hero.Armor.BodyArmor, world.Enemy.Armor.BodyArmor);
  //  Assert.AreNotSame(world.Hero.Armor.BodyArmor.Defense, world.Enemy.Armor.BodyArmor.Defense);
  //  Assert.AreNotSame(world.Hero.Weapon, world.Enemy.Weapon);
  //
  //  Assert.AreEqual(4, world.Hero.Weapon.Attack);
  //  Assert.AreEqual(2, world.Hero.Armor.BodyArmor.Defense);
  //  Assert.AreEqual(6, world.Enemy.Weapon.Attack);
  //  Assert.AreEqual(4, world.Enemy.Armor.BodyArmor.Defense);
  //}

  private static Character Soldier(BodyArmor bodyArmor, HandWeapon weapon)
  {
    return new Character(
      new Armor(
        new Helmet(),
        bodyArmor),
      weapon);
  }

  internal interface BodyArmor
  {
  }
  public record World(Character Hero, Character Enemy);
  public record Character(Armor Armor, HandWeapon Weapon);
  public record Armor(Helmet Helmet, BodyArmor BodyArmor);
  public record BreastPlate : BodyArmor;
  public record ChainMail: BodyArmor;
  public record Helmet;
  public interface HandWeapon
  {
  }
  public record ShortSword : HandWeapon;
  public record LongSword : HandWeapon;

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
          (_, context) => context.ResolveNamed<ShortSword>($"{_category}Sword")
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

      builder.RegisterType<ShortSword>()
        .Named<ShortSword>($"{_category}Sword")
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
