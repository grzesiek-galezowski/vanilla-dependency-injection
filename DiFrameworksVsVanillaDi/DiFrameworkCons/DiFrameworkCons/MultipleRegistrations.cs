using FluentAssertions;

namespace DiFrameworkCons;

//todo add descriptions
class MultipleRegistrations
{
  [Test]
  public void ShouldResolveLastRegisteredImplementationFromContainer()
  {
    //GIVEN
    var builder = new ContainerBuilder();
    builder.RegisterType<Constructor1Argument>().As<ConstructorArgument>().SingleInstance();
    builder.RegisterType<Constructor2Argument>().As<ConstructorArgument>().SingleInstance();
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
      Assert.Fail("should not be called");
      return null;
    }).As<ConstructorArgument>().SingleInstance();
    builder.RegisterType<Constructor1Argument>().As<ConstructorArgument>().InstancePerDependency();
    builder.RegisterType<ObjectWithConstructorArgument>().SingleInstance();
    using var container = builder.Build();

    //WHEN
    var resolvedInstance = container.Resolve<ObjectWithConstructorArgument>();

    //THEN
    resolvedInstance.Arg.Should().BeOfType<Constructor1Argument>();
  }



  public record ObjectWithConstructorArgument(ConstructorArgument Arg);

  public interface ConstructorArgument { }
  public record Constructor1Argument : ConstructorArgument;
  public record Constructor2Argument : ConstructorArgument;
}