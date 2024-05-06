using Lamar;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DiFrameworkPros._5_DependencyReplacement;

public static class DependencyReplacement_Lamar
{
  /// <summary>
  /// The only way to replace the registrations I found is
  /// to use the .Configure() method.
  /// </summary>
  [Test]
  public static void ShouldBeAbleToOverrideArbitraryDependencyInContainerUsingLamar()
  {
    using var container = new Container(builder =>
    {
      builder.AddSingleton<ISomeLogic, SomeLogic>();
      builder.AddSingleton<ITroublesomeDependency, TroublesomeDependency>();
    });

    var troublesomeDependencyMock = Substitute.For<ITroublesomeDependency>();
    container.Configure(collection =>
    {
      collection.Replace(
        new ServiceDescriptor(
          typeof(ITroublesomeDependency),
          troublesomeDependencyMock));
    });

    container.GetRequiredService<ISomeLogic>().Execute();

    troublesomeDependencyMock.Received(1).DoSomething();
  }
}