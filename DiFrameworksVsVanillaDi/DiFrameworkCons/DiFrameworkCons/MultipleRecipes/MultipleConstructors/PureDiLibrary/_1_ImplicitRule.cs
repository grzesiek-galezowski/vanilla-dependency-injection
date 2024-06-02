using Pure.DI;

namespace DiFrameworkCons.MultipleRecipes.MultipleConstructors.PureDiLibrary;

public static class _1_ImplicitRule
{
  [Test]
  public static void ShouldResolveUsingFirstConstructor()
  {
    //GIVEN
    var composition = new Composition11();

    //WHEN
    var instance = composition.Root;

    //THEN
    instance.Arg.Should().BeOfType<Constructor1Argument>();
  }
}

partial class Composition11
{
  public void Setup()
  {
    DI.Setup(nameof(Composition11))
      .Root<ObjectWithTwoConstructors>("Root");
  }
}