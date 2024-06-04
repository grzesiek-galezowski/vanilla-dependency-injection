namespace DiFrameworkCons.MultipleRecipes.RecipeDifferencePropagation.DifferingByLiterals.VanillaDi;

//todo add descriptions
class _2_VanillaCodeWithExtractedFunction
{
  [Test]
  public void ShouldResolveTwoSimilarObjectGraphsWithDifferentLeavesFromVanillaDiDried()
  {
    //GIVEN
    var world = new World(
        Soldier(4, 2),
        Soldier(6, 4));

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

  private static Character Soldier(int x1, int x2)
  {
    return new Character(
        new Armor(
            new Helmet(),
            new BreastPlate(x2)),
        new Sword(x1));
  }
}