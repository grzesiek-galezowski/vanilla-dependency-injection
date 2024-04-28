namespace DiFrameworkCons.TODO;

//todo add descriptions
public class MultipleRegistrations
{
  [Test]
  public void ShouldResolveLastRegisteredImplementationFromContainer()
  {
    //GIVEN
    var builder = new ContainerBuilder();
    builder.RegisterType<Constructor1Argument>().As<IConstructorArgument>().SingleInstance();
    builder.RegisterType<Constructor2Argument>().As<IConstructorArgument>().SingleInstance();
    builder.RegisterType<ObjectWithConstructorArgument>().SingleInstance();
    using var container = builder.Build();

    //WHEN
    var resolvedInstance = container.Resolve<ObjectWithConstructorArgument>();

    //THEN
    resolvedInstance.Arg.Should().BeOfType<Constructor2Argument>();
  }


  [Test]
  public void ShouldResolveLastRegisteredImplementationFromContainerForTheSameTypeAndDifferentLifestyles()
  {
    //GIVEN
    var builder = new ContainerBuilder();
    builder.Register<Constructor1Argument>(context =>
    {
      Execute.Assertion.FailWith("should not be called");
      return null!;
    }).As<IConstructorArgument>().SingleInstance();
    builder.RegisterType<Constructor1Argument>().As<IConstructorArgument>().InstancePerDependency();
    builder.RegisterType<ObjectWithConstructorArgument>().SingleInstance();
    using var container = builder.Build();

    //WHEN
    var resolvedInstance = container.Resolve<ObjectWithConstructorArgument>();

    //THEN
    resolvedInstance.Arg.Should().BeOfType<Constructor1Argument>();
  }



  public record ObjectWithConstructorArgument(IConstructorArgument Arg);

  public interface IConstructorArgument;
  public record Constructor1Argument : IConstructorArgument;
  public record Constructor2Argument : IConstructorArgument;
}