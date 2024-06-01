namespace DiFrameworkCons.MultipleRecipes.MultipleConstructors.Autofac;

public static class _1_UsingTheUsingConstructorFeature
{
  [Test]
  public static void ShouldResolveObjectUsingSelectedConstructor()
  {
    //GIVEN
    var builder = new ContainerBuilder();
    builder.RegisterType<Constructor1Argument>().SingleInstance();
    builder.RegisterType<Constructor2Argument>().SingleInstance(); // redundant
    builder.RegisterType<ObjectWithTwoConstructors>()
      .UsingConstructor(typeof(Constructor1Argument)).SingleInstance();
    using var container = builder.Build();

    //WHEN
    var resolvedInstance = container.Resolve<ObjectWithTwoConstructors>();

    //THEN
    resolvedInstance.Arg.Should().BeOfType<Constructor1Argument>();
  }
}