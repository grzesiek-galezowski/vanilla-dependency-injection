using Lamar;

namespace DiFrameworkCons.CompileTimeVsRuntime.CircularDependencies.Lamar;

public class _1_ContainerValidationOnBuild
{
  /// <summary>
  /// Lamar also detects circular dependencies during container build.
  /// </summary>
  [Test]
  public static void ShouldDetectCircularDependenciesWhenBuildingContainer()
  {
    Invoking(() =>
      {
        using var x = new Container(builder =>
        {
          builder.AddTransient<One>()
            .AddTransient<Two>()
            .AddTransient<Three>();
        });
      }).Should().ThrowExactly<InvalidOperationException>()
      .Which.ToString().Should().ContainAny([
          "Bi-directional dependencies detected to new One(Two)",
          "Bi-directional dependencies detected to new Two(Three)",
          "Bi-directional dependencies detected to new Three(One)"
        ]
      );
  }
}