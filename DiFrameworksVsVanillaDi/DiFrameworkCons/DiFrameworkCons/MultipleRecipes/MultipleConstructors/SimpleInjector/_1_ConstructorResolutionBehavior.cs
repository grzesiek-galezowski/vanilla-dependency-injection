using System.Collections.Generic;
using DiFrameworkCons.SimpleInjectorExtensions;
using SimpleInjector;
using SimpleInjector.Advanced;

namespace DiFrameworkCons.MultipleRecipes.MultipleConstructors.SimpleInjector;

/// <summary>
/// Autofac's creator discourages the use of multiple constructors in a class,
/// because they introduce "ambiguity". While not onboard with his reasoning,
/// I tend to steer away from multiple constructors as much as possible myself.
///
/// Anyway, the closest thing to a "recommended approach" is to use
/// the <see cref="IConstructorResolutionBehavior"/> interface.
///
/// The tradeoff is that this behavior is only configurable globally
/// on the container level.
/// </summary>
internal class _1_ConstructorResolutionBehavior
{
  [Test]
  public void ShouldPickTheCorrectConstructorUsingResolutionBehavior()
  {
    //GIVEN
    using var container = new Container();
    container.Options.ConstructorResolutionBehavior = new PerTypeConstructorBehavior(
      new Dictionary<Type, List<Type>>
      {
        [typeof(ObjectWithTwoConstructors)] = [typeof(Constructor2Argument)]
      },
      container.Options.ConstructorResolutionBehavior);

    container.RegisterSingleton<Constructor1Argument>();
    container.RegisterSingleton<Constructor2Argument>();
    container.RegisterSingleton<ObjectWithTwoConstructors>();

    //WHEN
    var resolvedInstance = container.GetInstance<ObjectWithTwoConstructors>();

    //THEN
    resolvedInstance.Arg.Should().BeOfType<Constructor2Argument>();
  }
}