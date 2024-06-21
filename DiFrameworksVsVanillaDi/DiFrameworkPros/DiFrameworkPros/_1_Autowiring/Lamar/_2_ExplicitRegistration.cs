using Lamar;
using Microsoft.Extensions.DependencyInjection;

namespace DiFrameworkPros._1_Autowiring.Lamar;

public static class _2_ExplicitRegistration
{
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