namespace DiFrameworkCons.MultipleRecipes.MultipleConstructors.VanillaDi;

//todo add descriptions
//BUG: add SimpleInjector version
class _1_VanillaCode
{
  [Test]
  public void ShouldResolveUsingFirstConstructorUsingVanillaDi()
  {
    //GIVEN

    //WHEN
    var instance = new ObjectWithTwoConstructors(new Constructor1Argument());

    //THEN
    instance.Arg.Should().BeOfType<Constructor1Argument>();
  }

  //BUG: the third option is to use a ActivatorUtilitiesConstructorAttribute
}