namespace DiFrameworkCons.MultipleConstructors;

public static class MultipleConstructors_Autofac
{
  [Test]
  public static void ShouldResolveUsingFirstConstructorFromAutofac()
  {
    //GIVEN
    var builder = new ContainerBuilder();
    builder.RegisterType<Constructor1Argument>().SingleInstance();
    builder.RegisterType<Constructor2Argument>().SingleInstance();
    builder.RegisterType<ObjectWithTwoConstructors>()
      .UsingConstructor(typeof(Constructor1Argument)).SingleInstance();
    using var container = builder.Build();

    //WHEN
    var resolvedInstance = container.Resolve<ObjectWithTwoConstructors>();

    //THEN
    resolvedInstance.Arg.Should().BeOfType<Constructor1Argument>();
  }
}