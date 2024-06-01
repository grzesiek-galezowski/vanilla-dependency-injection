using Lamar;

namespace DiFrameworkCons.MultipleRecipes.MultipleConstructors.Lamar;

public static class _2_SelectConstructor
{
  /// <summary>
  /// We can also pick the constructor at the price of using
  /// a lambda.
  /// </summary>
  [Test]
  public static void ShouldResolveUsingFirstConstructorUsingManualSelection()
  {
    //GIVEN
    using var container = new Container(builder =>
    {
      builder.AddSingleton<Constructor1Argument>();
      builder.AddSingleton<Constructor2Argument>();
      builder.AddSingleton<ObjectWithTwoConstructors>();

      builder.ForConcreteType<ObjectWithTwoConstructors>().Configure
        .SelectConstructor(() => new ObjectWithTwoConstructors(
          (null as Constructor1Argument)!));
    });

    //WHEN
    var resolvedInstance = container.GetRequiredService<ObjectWithTwoConstructors>();

    //THEN
    resolvedInstance.Arg.Should().BeOfType<Constructor1Argument>();
  }
}