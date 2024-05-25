using Lamar;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DiFrameworkCons.MultipleRecipes.RecipeDifferencePropagation.DifferingByLiterals;

public static class WorldWithLiterals_Lamar
{
  /// <summary>
  /// Surprisingly, the version of this example from MsDi file
  /// (one that combined keyed and keyless registrations of the same type)
  /// did not work with Lamar. I had to turn the registrations for a type
  /// to keyed to make it work.
  /// </summary>
  [Test]
  public static void ShouldResolveTwoSimilarObjectGraphsWithDifferentLeaves()
  {
    //GIVEN
    using var container = new Container(builder =>
    {
      const string firstKey = "first";
      const string secondKey = "second";
      builder.AddSingleton(x =>
        ActivatorUtilities.CreateInstance<World>(
          x,
          x.GetRequiredKeyedService<Character>(firstKey),
          x.GetRequiredKeyedService<Character>(secondKey)
        ));
      builder.AddKeyedSingleton(firstKey,
        (x, key) =>
          ActivatorUtilities.CreateInstance<Character>(x,
            x.GetRequiredKeyedService<Armor>(key),
            x.GetRequiredKeyedService<Sword>(key)));

      builder.AddKeyedSingleton(secondKey,
        (x, key) =>
          ActivatorUtilities.CreateInstance<Character>(x,
            x.GetRequiredKeyedService<Armor>(key),
            x.GetRequiredKeyedService<Sword>(key)));

      builder.AddKeyedSingleton(firstKey, (x, key) =>
        ActivatorUtilities.CreateInstance<Armor>(x,
          x.GetRequiredKeyedService<BreastPlate>(key)));
      builder.AddKeyedSingleton(secondKey, (x, key) =>
        ActivatorUtilities.CreateInstance<Armor>(x,
          x.GetRequiredKeyedService<BreastPlate>(key)));
      builder.AddTransient<Helmet>();
      builder.AddKeyedSingleton(firstKey,
        (x, key) => ActivatorUtilities.CreateInstance<BreastPlate>(x, 2));
      builder.AddKeyedSingleton(secondKey,
        (x, key) => ActivatorUtilities.CreateInstance<BreastPlate>(x, 4));
      builder.AddKeyedSingleton(firstKey, (x, key) => ActivatorUtilities.CreateInstance<Sword>(x, 4));
      builder.AddKeyedSingleton(secondKey, (x, key) => ActivatorUtilities.CreateInstance<Sword>(x, 6));
    });

    //WHEN
    var world = container.GetRequiredService<World>();

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

  [Test]
  public static void ShouldResolveTwoSimilarObjectGraphsWithDifferentLeavesFromContainerModules()
  {
    using var container = new Container(builder =>
    {
      builder.AddSingleton(x =>
        ActivatorUtilities.CreateInstance<World>(
          x,
          x.GetRequiredKeyedService<Character>("first"),
          x.GetRequiredKeyedService<Character>("second")));
      builder.AddCharacter("first", 2, 4);
      builder.AddCharacter("second", 4, 6);
    });

    //WHEN
    var world = container.GetRequiredService<World>();

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

  [Test]
  public static void ShouldResolveTwoSimilarObjectGraphsWithDifferentLeavesUsingActivatorUtilities()
  {
    //GIVEN
    using var container = new Container(builder =>
    {
      builder
        .AddSingleton(x =>
          ActivatorUtilities.CreateInstance<World>(x,
            CreateCharacter(x, breastPlateDefense: 2, swordAttack: 4),
            CreateCharacter(x, breastPlateDefense: 4, swordAttack: 6)));
    });

    //WHEN
    var world = container.GetRequiredService<World>();

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

  /// <summary>
  /// Lamar also allows adding arguments to registrations,
  /// similar to Autofac.
  ///
  /// This version could also be converted to "modules".
  /// </summary>
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
        .Ctor<Sword>().IsNamedInstance(firstKey)
        .Singleton().Named(firstKey);

      builder.ForConcreteType<Character>()
        .Configure
        .Ctor<Armor>().IsNamedInstance(secondKey)
        .Ctor<Sword>().IsNamedInstance(secondKey)
        .Singleton().Named(secondKey);

      builder.ForConcreteType<Armor>()
        .Configure
        .Ctor<BreastPlate>().IsNamedInstance(firstKey)
        .Singleton().Named(firstKey);

      builder.ForConcreteType<Armor>()
        .Configure
        .Ctor<BreastPlate>().IsNamedInstance(secondKey)
        .Singleton().Named(secondKey);

      builder.For<Helmet>().Use<Helmet>().Transient();
      builder.ForConcreteType<BreastPlate>()
        .Configure
        .Ctor<int>().Is(2)
        .Singleton().Named(firstKey);
      builder.ForConcreteType<BreastPlate>()
        .Configure
        .Ctor<int>().Is(4)
        .Singleton().Named(secondKey);

      builder.ForConcreteType<Sword>()
        .Configure
        .Ctor<int>().Is(4)
        .Singleton().Named(firstKey);
      builder.ForConcreteType<Sword>()
        .Configure
        .Ctor<int>().Is(6)
        .Singleton().Named(secondKey);
    });

    //WHEN
    var world = container.GetRequiredService<World>();

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

  private static Character CreateCharacter(IServiceProvider x, int breastPlateDefense, int swordAttack)
  {
    return ActivatorUtilities.CreateInstance<Character>(x,
      ActivatorUtilities.CreateInstance<Armor>(x,
        ActivatorUtilities.CreateInstance<Helmet>(x),
        ActivatorUtilities.CreateInstance<BreastPlate>(x, breastPlateDefense)),
      ActivatorUtilities.CreateInstance<Sword>(x, swordAttack));
  }
}

file static class CharacterWithLiteralsExtensions
{
  public static void AddCharacter(
    this IServiceCollection builder,
    string prefix,
    int breastPlateDefense,
    int swordAttack)
  {
    builder.AddKeyedSingleton(prefix,
      (x, key) =>
        ActivatorUtilities.CreateInstance<Character>(x,
          x.GetRequiredKeyedService<Armor>(key),
          x.GetRequiredKeyedService<Sword>(key)));
    builder.AddKeyedSingleton(prefix, (x, key) =>
      ActivatorUtilities.CreateInstance<Armor>(x,
        x.GetRequiredKeyedService<BreastPlate>(key)));
    builder.TryAddTransient<Helmet>();
    builder.AddKeyedSingleton(prefix,
      (x, key) => ActivatorUtilities.CreateInstance<BreastPlate>(x, breastPlateDefense));
    builder.AddKeyedSingleton(prefix,
      (x, key) => ActivatorUtilities.CreateInstance<Sword>(x, swordAttack));
  }

}