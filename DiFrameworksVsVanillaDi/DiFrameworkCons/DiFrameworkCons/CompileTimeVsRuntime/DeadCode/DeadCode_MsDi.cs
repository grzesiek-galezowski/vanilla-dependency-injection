namespace DiFrameworkCons.CompileTimeVsRuntime.DeadCode;

public static class DeadCode_MsDi
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
  public static void ContainerContainsSomeDeadCode()
  {
    //GIVEN
    var builder = new ServiceCollection();
    builder.AddSingleton<Dependency>();
    builder.AddSingleton<DependencyConsumer>();
    //dead code
    builder.AddSingleton<DeadCode_VanillaDi>();
    using var container = builder.BuildServiceProvider();

    //WHEN
    var resolvedInstance = container.GetRequiredService<DependencyConsumer>();

    //THEN
    resolvedInstance.Should().NotBeNull();
  }
}