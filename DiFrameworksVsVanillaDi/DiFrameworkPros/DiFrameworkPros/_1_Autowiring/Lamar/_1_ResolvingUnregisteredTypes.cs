using Lamar;
using Microsoft.Extensions.DependencyInjection;

namespace DiFrameworkPros._1_Autowiring.Lamar;

public static class _1_ResolvingUnregisteredTypes
{
  /// <summary>
  /// Lamar can automatically find the required types and create
  /// their instances as long as those types are public and
  /// as long as we're happy with the __transient__ lifestyle.
  /// So far, I could not find the way to configure the default lifestyle.
  /// </summary>
  [Test]
  public static void ShouldAutomaticallyResolvePublicTransientDependencies()
  {
    using var container = new Container(_ => { });

    var person1 = container.GetRequiredService<Person>();
    var person2 = container.GetRequiredService<Person>();

    person1.Should().NotBeSameAs(person2);
  }
}