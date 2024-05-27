using DiFrameworkCons.CompileTimeVsRuntime.DeadCode.VanillaDi;

namespace DiFrameworkCons.CompileTimeVsRuntime.DeadCode.Autofac;

public static class DeadCode_Autofac
{
  /// <summary>
  /// The container doesn't know which registrations are going to be used
  /// so there's no way of detecting "dead" dependencies that are
  /// not used or passed anywhere.
  /// This may lead to a situation where we delete some objects because
  /// e.g. we remove a feature but some leftover dependencies stay because
  /// we don't even notice they were used only by that removed part of code. 
  /// </summary>
  [Test]
  public static void ContainerContainsSomeDeadCodeWithAutofac()
  {
    //GIVEN
    var builder = new ContainerBuilder();
    builder.RegisterType<Dependency>().SingleInstance();
    builder.RegisterType<DependencyConsumer>().SingleInstance();
    //dead code
    builder.RegisterType<DeadCode_VanillaDi>().InstancePerDependency();
    using var container = builder.Build();

    //WHEN
    var resolvedInstance = container.Resolve<DependencyConsumer>();

    //THEN
    resolvedInstance.Should().NotBeNull();
  }
}