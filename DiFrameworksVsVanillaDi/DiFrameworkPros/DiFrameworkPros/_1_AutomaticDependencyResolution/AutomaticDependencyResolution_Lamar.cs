using Lamar;
using Microsoft.Extensions.DependencyInjection;

namespace DiFrameworkPros._1_AutomaticDependencyResolution;

public static class AutomaticDependencyResolution_Lamar
{
  /// <summary>
  /// Lamar can automatically find the required types and create
  /// their instances as long as those types are public and
  /// as long as we're happy with the __transient__ lifestyle.
  /// So far, I could not find the way to configure the default lifestyle.
  /// </summary>
  [Test]
  public static void ShouldAutomaticallyResolvePublicTransientDependenciesUsingLamar()
  {
    using var container = new Container(_ => { });
    
    var person1 = container.GetRequiredService<Person>();
    var person2 = container.GetRequiredService<Person>();

    person1.Should().NotBeSameAs(person2);
  }

  /// <summary>
  /// Every non-default creation we need to register explicitly
  /// </summary>
  [Test]
  public static void ShouldAllowConfiguringNonStandardCreationUsingLamar()
  {
    using var container = new Container(registry =>
    {
      registry.AddSingleton<Person>();
      registry.AddSingleton<Kitchen>();
      registry.AddSingleton<Knife>();
      registry.AddSingleton<LoggingChannel>();
    });

    var person1 = container.GetRequiredService<Person>();
    var person2 = container.GetRequiredService<Person>();

    person1.Should().BeSameAs(person2);
  }
}