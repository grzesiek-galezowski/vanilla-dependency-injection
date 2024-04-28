namespace DiFrameworkCons.MultipleConstructors;

//todo add descriptions
class MultipleConstructors_VanillaDi
{
  [Test]
  public void ShouldResolveUsingFirstConstructorUsingVanillaDi()
  {
    //GIVEN

    //WHEN
    var resolvedInstance = new ObjectWithTwoConstructors(new Constructor1Argument());

    //THEN
    resolvedInstance.Arg.Should().BeOfType<Constructor1Argument>();
  }
}