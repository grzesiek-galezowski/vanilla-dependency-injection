using Microsoft.Extensions.DependencyInjection;

namespace DiFrameworkPros._1_AutomaticDependencyResolution;

public static class AutomaticDependencyResolution_MsDi
{
  [Test]
  public static void ShouldAutomaticallyResolveBasicDependenciesUsingMsDi()
  {
    var builder = new ServiceCollection();
    builder.AddSingleton<Person>();
    builder.AddSingleton<Kitchen>();
    builder.AddSingleton<Knife>();
    builder.AddTransient<Logger>();
    builder.AddSingleton<LoggingChannel>();

    using var container = builder.BuildServiceProvider();

    var person1 = container.GetRequiredService<Person>();
    var person2 = container.GetRequiredService<Person>();

    person1.Should().BeSameAs(person2);
  }
}