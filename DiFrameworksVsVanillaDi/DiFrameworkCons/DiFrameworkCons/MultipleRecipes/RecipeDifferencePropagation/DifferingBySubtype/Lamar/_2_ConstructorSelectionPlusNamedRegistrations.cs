using Lamar;

namespace DiFrameworkCons.MultipleRecipes.RecipeDifferencePropagation.DifferingBySubtype.Lamar;

public static class _2_ConstructorSelectionPlusNamedRegistrations
{
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
}