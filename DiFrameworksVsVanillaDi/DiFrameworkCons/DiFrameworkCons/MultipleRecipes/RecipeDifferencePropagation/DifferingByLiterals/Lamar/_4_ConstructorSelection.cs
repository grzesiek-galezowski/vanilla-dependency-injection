using Lamar;

namespace DiFrameworkCons.MultipleRecipes.RecipeDifferencePropagation.DifferingByLiterals.Lamar;

public static class _4_ConstructorSelection
{
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
}