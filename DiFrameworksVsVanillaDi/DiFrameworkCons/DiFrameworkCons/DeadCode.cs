namespace DiFrameworkCons;

/// <summary>
/// Sometimes, you may have some dependencies that you simply don't use
/// or don't use anymore. 
/// </summary>
public class DeadCode
{
  /// <summary>
  /// The container doesn't know which registrations are going to be used
  /// so there's no way of detecting "dead" dependencies that are not used or passed anywhere.
  /// This may lead to a situation where we delete some objects because e.g. we remove a feature
  /// but some leftover dependencies stay because we don't even notice they were used only by that
  /// removed part of code. 
  /// </summary>
  [Test]
  public void ContainerContainsSomeDeadCodeWithAutofac()
  {
    //GIVEN
    var builder = new ContainerBuilder();
    builder.RegisterType<Dependency>().SingleInstance();
    builder.RegisterType<DependencyConsumer>().SingleInstance();
    //dead code
    builder.RegisterType<DeadCode>().InstancePerDependency();
    using var container = builder.Build();

    //WHEN
    var resolvedInstance = container.Resolve<DependencyConsumer>();

    //THEN
    resolvedInstance.Should().NotBeNull();
  }

  [Test]
  public void ContainerContainsSomeDeadCodeWithMsDi()
  {
    //GIVEN
    var builder = new ServiceCollection();
    builder.AddSingleton<Dependency>();
    builder.AddSingleton<DependencyConsumer>();
    //dead code
    builder.AddSingleton<DeadCode>();
    using var container = builder.BuildServiceProvider();

    //WHEN
    var resolvedInstance = container.GetRequiredService<DependencyConsumer>();

    //THEN
    resolvedInstance.Should().NotBeNull();
  }

  /// <summary>
  /// "Dead" dependencies can be clearly visible in the imperative code when
  /// doing Vanilla DI - below, the `deadCode` will be marked by the IDE as unused.
  /// </summary>
  [Test]
  public void VanillaDiContainsDeadCode()
  {
    //GIVEN
    var consumer = new DependencyConsumer(new Dependency());
    var deadCode = new DeadCode();

    //WHEN

    //THEN
    consumer.Should().NotBeNull();
  }
}


public record DependencyConsumer(Dependency Dependency);
public record Dependency;