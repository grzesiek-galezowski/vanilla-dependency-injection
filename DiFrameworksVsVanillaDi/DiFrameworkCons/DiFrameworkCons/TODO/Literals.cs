namespace DiFrameworkCons.TODO;

//todo add descriptions, change this to the example from the live stream
public class Literals
{
  [Test]
  public void ShouldResolveObjectWithLiterals()
  {
    //GIVEN
    var builder = new ContainerBuilder();
    builder.RegisterType<Dependency>().SingleInstance();
    builder.RegisterType<DependencyConsumer>()
        .WithParameter("X", 2).SingleInstance();
    using var container = builder.Build();

    //WHEN
    var resolvedInstance = container.Resolve<DependencyConsumer>();

    //THEN
    resolvedInstance.X.Should().Be(2);
  }

  [Test]
  public void ShouldResolveObjectWithLiteralsUsingVanillaDi()
  {
    //GIVEN
    var consumer = new DependencyConsumer(new Dependency(), 2);

    //WHEN

    //THEN
    consumer.Should().NotBeNull();
  }

  public record DependencyConsumer(Dependency Dependency, int X);
  public record Dependency;
}