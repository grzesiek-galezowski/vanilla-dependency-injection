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

    world.Enemy.Should().NotBeSameAs(world.Hero);
    world.Enemy.Armor.Should().NotBeSameAs(world.Hero.Armor);
    world.Enemy.Armor.Helmet.Should().NotBeSameAs(world.Hero.Armor.Helmet);

    world.Hero.Armor.BodyArmor.Should().BeOfType<ChainMail>();
    world.Enemy.Armor.BodyArmor.Should().BeOfType<BreastPlate>();
    world.Hero.Weapon.Should().BeOfType<LongSword>();
    world.Enemy.Weapon.Should().BeOfType<ShortSword>();
  }

  [Test]
  public void ShouldResolveTwoSimilarObjectGraphsWithDifferentLeavesFromVanillaDiDried()
  {
    //GIVEN
    var world = new World(
      Soldier(new ChainMail(), new LongSword()),
      Soldier(new BreastPlate(), new ShortSword()));

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
    world.Enemy.Should().NotBeSameAs(world.Hero);
    world.Enemy.Armor.Should().NotBeSameAs(world.Hero.Armor);
    world.Enemy.Armor.Helmet.Should().NotBeSameAs(world.Hero.Armor.Helmet);

    world.Hero.Armor.BodyArmor.Should().BeOfType<ChainMail>();
    world.Enemy.Armor.BodyArmor.Should().BeOfType<BreastPlate>();
    world.Hero.Weapon.Should().BeOfType<LongSword>();
    world.Enemy.Weapon.Should().BeOfType<ShortSword>();
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
    world.Enemy.Should().NotBeSameAs(world.Hero);
    world.Enemy.Armor.Should().NotBeSameAs(world.Hero.Armor);
    world.Enemy.Armor.Helmet.Should().NotBeSameAs(world.Hero.Armor.Helmet);

    world.Hero.Armor.BodyArmor.Should().BeOfType<ChainMail>();
    world.Enemy.Armor.BodyArmor.Should().BeOfType<BreastPlate>();
    world.Hero.Weapon.Should().BeOfType<LongSword>();
    world.Enemy.Weapon.Should().BeOfType<ShortSword>();
  }

  [Test]
  public void ShouldResolveTwoSimilarObjectGraphsWithDifferentLeavesFromContainerModulesUsingMsDi()
  {
    //GIVEN
    var builder = new ServiceCollection();
    builder.AddSingleton(c => ActivatorUtilities.CreateInstance<World>(
      c,
      c.GetRequiredKeyedService<Character>("hero"),
      c.GetRequiredKeyedService<Character>("enemy")));
    SoldierMsDiModule.RegisterIn<LongSword, ChainMail>(builder, "hero");
    SoldierMsDiModule.RegisterIn<ShortSword, BreastPlate>(builder, "enemy");

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
  public void ShouldResolveTwoSimilarObjectGraphsWithDifferentLeavesFromContainerModulesUsingAutofac()
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

  private static Character Soldier(BodyArmor bodyArmor, HandWeapon weapon)
  {
    return new Character(
      new Armor(
        new Helmet(),
        bodyArmor),
      weapon);
  }

  internal interface BodyArmor;
  public record World(Character Hero, Character Enemy);
  public record Character(Armor Armor, HandWeapon Weapon);
  public record Armor(Helmet Helmet, BodyArmor BodyArmor);
  public record BreastPlate : BodyArmor;
  public record ChainMail: BodyArmor;
  public record Helmet;
  public interface HandWeapon;
  public record ShortSword : HandWeapon;
  public record LongSword : HandWeapon;

  public class SoldierAutofacModule<THandWeapon, TBodyArmor> : Module
    where THandWeapon : HandWeapon
    where TBodyArmor : BodyArmor  
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
          (_, context) => context.ResolveNamed<Armor>($"{_category}Armor")
        )
        .WithParameter(
          (info, _) => info.Position == 1,
          (_, context) => context.ResolveNamed<THandWeapon>($"{_category}HandWeapon")
        )
        .Named<Character>($"{_category}Character")
        .SingleInstance();

      builder.RegisterType<Armor>()
        .WithParameter(
          (info, _) => info.Position == 0,
          (_, context) => context.ResolveNamed<Helmet>($"{_category}Helmet"))
        .WithParameter(
          (info, _) => info.Position == 1,
          (_, context) => context.ResolveNamed<TBodyArmor>($"{_category}BodyArmor"))
        .Named<Armor>($"{_category}Armor")
        .SingleInstance();

      builder.RegisterType<THandWeapon>()
        .Named<THandWeapon>($"{_category}HandWeapon")
        .SingleInstance();
      builder.RegisterType<Helmet>()
        .Named<Helmet>($"{_category}Helmet")
        .SingleInstance();
      builder.RegisterType<TBodyArmor>()
        .Named<TBodyArmor>($"{_category}BodyArmor")
        .SingleInstance();
    }
  }

  public static class SoldierMsDiModule
  {
    public static void RegisterIn<THandWeapon, TBodyArmor>(
      ServiceCollection builder, string category)
      where THandWeapon : class, HandWeapon
      where TBodyArmor : class, BodyArmor
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

      if (!builder.Contains(
            new ServiceDescriptor(
              typeof(Helmet),
              typeof(Helmet),
              ServiceLifetime.Transient)))
      {
        builder.AddTransient<Helmet>();
      }

      builder.AddKeyedSingleton<TBodyArmor>(category);
      builder.AddKeyedSingleton<THandWeapon>(category);
    }
  }
}