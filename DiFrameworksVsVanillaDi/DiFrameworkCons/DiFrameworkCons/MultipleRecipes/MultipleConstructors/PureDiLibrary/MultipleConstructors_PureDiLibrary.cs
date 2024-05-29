using Pure.DI;

namespace DiFrameworkCons.MultipleRecipes.MultipleConstructors.PureDiLibrary;

/// <summary>
/// Automatic constructor selection
/// BUG: describe ordinal attribute
/// </summary>
class MultipleConstructors_PureDiLibrary
{
  [Test]
  public void ShouldResolveUsingFirstConstructor()
  {
    //GIVEN
    var composition = new Composition11();

    //WHEN
    var instance = composition.Root;

    //THEN
    instance.Arg.Should().BeOfType<Constructor1Argument>();
  }

  /// <summary>
  /// In order to select the constructor, we need to use a lambda,
  /// but aside of maintaining the constructor call,
  /// it's not a big issue with Pure.DI.
  /// </summary>
  [Test]
  public void ShouldResolveUsingSelectedConstructor()
  {
    //GIVEN
    var composition = new Composition12();

    //WHEN
    var instance = composition.Root;

    //THEN
    instance.Arg.Should().BeOfType<Constructor2Argument>();
  }

  [Test]
  public void ShouldResolveUsingConstructorMarkedWithOrdinalAttribute()
  {
    //GIVEN
    var composition = new Composition13();

    //WHEN
    var instance = composition.Root;

    //THEN
    instance.Arg.Should().BeOfType<Constructor2Argument>();
  }

  //BUG: the third option is to use a OrdinalAttribute
}

partial class Composition11
{
  public void Setup()
  {
    DI.Setup(nameof(Composition11))
      .Root<ObjectWithTwoConstructors>("Root");
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

public class ObjectWithTwoConstructorsForPureDi : ObjectWithTwoConstructors
{
  public ObjectWithTwoConstructorsForPureDi(
     Constructor1Argument arg) : base(arg)
  {
  }

  /// <summary>
  /// Using the OrdinalAttribute couples the constructor with the
  /// Pure.DI library, but one can copy-paste the attribute to
  /// their codebase to avoid the coupling.
  /// </summary>
  /// <param name="arg"></param>
  [Ordinal(0)]
  public ObjectWithTwoConstructorsForPureDi(Constructor2Argument arg) : base(arg)
  {
  }
}

partial class Composition13
{
  public void Setup()
  {
    DI.Setup(nameof(Composition13))
      .Root<ObjectWithTwoConstructorsForPureDi>("Root");
  }
}