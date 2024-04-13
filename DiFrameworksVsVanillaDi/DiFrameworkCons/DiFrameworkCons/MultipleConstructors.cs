namespace DiFrameworkCons;

//todo add descriptions
class MultipleConstructors
{
  [Test]
  public void ShouldResolveUsingFirstConstructorFromContainer()
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

  [Test]
  public void ShouldResolveUsingFirstConstructorUsingVanillaDi()
  {
    //GIVEN

    //WHEN
    var resolvedInstance = new ObjectWithTwoConstructors(new Constructor1Argument());

    //THEN
    resolvedInstance.Arg.Should().BeOfType<Constructor1Argument>();
  }

  public class ObjectWithTwoConstructors
  {
    public readonly ConstructorArgument Arg;

    public ObjectWithTwoConstructors(Constructor1Argument arg)
    {
      Arg = arg;
    }
    public ObjectWithTwoConstructors(Constructor2Argument arg)
    {
      Arg = arg;
    }
  }

  public interface ConstructorArgument;

  public record Constructor1Argument : ConstructorArgument;
  public record Constructor2Argument : ConstructorArgument;
}