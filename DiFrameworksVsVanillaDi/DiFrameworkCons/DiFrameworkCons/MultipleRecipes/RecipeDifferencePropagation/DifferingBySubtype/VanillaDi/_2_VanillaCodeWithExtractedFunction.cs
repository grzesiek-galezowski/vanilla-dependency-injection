namespace DiFrameworkCons.MultipleRecipes.RecipeDifferencePropagation.DifferingBySubtype.VanillaDi;

//todo add descriptions
class _2_VanillaCodeWithExtractedFunction
{
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

  private static Character Soldier(IBodyArmor bodyArmor, IHandWeapon weapon)
  {
    return new Character(
      new Armor(
        new Helmet(),
        bodyArmor),
      weapon);
  }
}