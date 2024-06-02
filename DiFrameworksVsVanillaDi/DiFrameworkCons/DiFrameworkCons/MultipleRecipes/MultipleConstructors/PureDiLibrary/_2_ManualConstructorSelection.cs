using Pure.DI;

namespace DiFrameworkCons.MultipleRecipes.MultipleConstructors.PureDiLibrary;

public static class _2_ManualConstructorSelection
{
  /// <summary>
  /// In order to select the constructor, we need to use a lambda,
  /// but aside of maintaining the constructor call,
  /// it's not a big issue with Pure.DI.
  /// </summary>
  [Test]
  public static void ShouldResolveUsingSelectedConstructor()
  {
    //GIVEN
    var composition = new Composition12();

    //WHEN
    var instance = composition.Root;

    //THEN
    instance.Arg.Should().BeOfType<Constructor2Argument>();
  }
}

partial class Composition12
{
  public void Setup()
  {
    DI.Setup(nameof(Composition12))
      .Bind<Constructor2Argument>().To<Constructor2Argument>()
      .RootBind<ObjectWithTwoConstructors>("Root").To(
        context =>
        {
          context.Inject<Constructor2Argument>(out var arg);
          return new ObjectWithTwoConstructors(arg);
        });
  }
}