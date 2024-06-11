namespace DiFrameworkCons.MultipleRecipes.MultipleConstructors.MsDi;

public static class _1_ActivatorUtilitiesAttribute
{
  /// <summary>
  /// We can further help the container pick the right constructor
  /// by marking it with the ActivatorUtilitiesConstructor attribute.
  /// </summary>
  [Test]
  public static void ShouldResolveUsingFirstConstructor()
  {
    //GIVEN
    var builder = new ServiceCollection();
    builder.AddSingleton<Constructor1Argument>();
    builder.AddSingleton<Constructor2Argument>();
    builder.AddSingleton(
      x => ActivatorUtilities.CreateInstance<ObjectWithTwoConstructorsForMsDi>(x));

    using var container = builder.BuildServiceProvider(new ServiceProviderOptions()
    {
      ValidateOnBuild = true,
      ValidateScopes = true
    });

    //WHEN
    var resolvedInstance = container.GetRequiredService<ObjectWithTwoConstructorsForMsDi>();

    //THEN
    resolvedInstance.Arg.Should().BeOfType<Constructor2Argument>();
  }
}

public class ObjectWithTwoConstructorsForMsDi : ObjectWithTwoConstructors
{
  public ObjectWithTwoConstructorsForMsDi(
    Constructor1Argument arg) : base(arg)
  {
  }

  /// <summary>
  /// Using the ActivatorUtilitiesConstructor couples the constructor with the
  /// Pure.DI library, but one can copy-paste the attribute to
  /// their codebase to avoid the coupling.
  /// </summary>
  /// <param name="arg"></param>
  [ActivatorUtilitiesConstructor]
  public ObjectWithTwoConstructorsForMsDi(Constructor2Argument arg) : base(arg)
  {
  }
}