using Lamar;

namespace DiFrameworkCons.MultipleRecipes.MultipleConstructors.Lamar;

public static class MultipleConstructors_Lamar
{
  /// <summary>
  /// Lamar by default picks the "greediest" constructor it can satisfy
  /// and if two constructors are equivalent, it just picks the first one.
  ///
  /// Here, we are "lucky" that it's the constructor we wanted.
  /// Changing the order in the constructors in the class breaks
  /// this test.
  /// </summary>
  [Test]
  public static void ShouldResolveUsingFirstConstructorUsingConvention()
  {
    //GIVEN
    using var container = new Container(builder =>
    {
      builder.AddSingleton<Constructor1Argument>();
      builder.AddSingleton<Constructor2Argument>();
      builder.AddSingleton<ObjectWithTwoConstructors>();
    });

    //WHEN
    var resolvedInstance = container.GetRequiredService<ObjectWithTwoConstructors>();

    //THEN
    resolvedInstance.Arg.Should().BeOfType<Constructor1Argument>();
  }

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