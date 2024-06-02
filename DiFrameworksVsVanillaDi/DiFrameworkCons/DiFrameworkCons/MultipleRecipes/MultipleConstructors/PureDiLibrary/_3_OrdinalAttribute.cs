using Pure.DI;

namespace DiFrameworkCons.MultipleRecipes.MultipleConstructors.PureDiLibrary;

/// <summary>
/// Automatic constructor selection
/// BUG: describe ordinal attribute
/// BUG: make attribute based example for the other containers
/// </summary>
class _3_OrdinalAttribute
{
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
}

partial class Composition13
{
  public void Setup()
  {
    DI.Setup(nameof(Composition13))
      .Root<ObjectWithTwoConstructorsForPureDi>("Root");
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