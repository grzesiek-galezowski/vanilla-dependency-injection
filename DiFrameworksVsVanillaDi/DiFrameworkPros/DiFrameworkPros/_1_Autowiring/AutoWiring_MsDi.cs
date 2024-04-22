using Microsoft.Extensions.DependencyInjection;

namespace DiFrameworkPros._1_Autowiring;

public static class AutoWiring_MsDi
{
  [Test]
  public static void ShouldAutoWireBasicDependenciesUsingMsDi()
  {
    var builder = new ServiceCollection();
    builder.AddSingleton<Person>();
    builder.AddSingleton<Kitchen>();
    builder.AddSingleton<Knife>();
    builder.AddTransient<Logger>();
    builder.AddSingleton<LoggingChannel>();

    using var container = builder.BuildServiceProvider();

    var person = container.GetRequiredService<Person>();
  }
}