using SimpleInjector;

namespace DiFrameworkCons.MultipleRecipes.MultipleSameInterfaceArguments.SimpleInjector;

public static class _1_ConditionalRegistrations
{
  [Test]
  public static void ContainerCompositionThroughConditionallyRegisteredComponents()
  {
    using var container = new Container();
    container.RegisterConditional<IDataStorage>(
      Lifestyle.Singleton.CreateRegistration<LocalDataStorage>(container),
      context => context.Consumer.Target.Parameter?.Position == 0);
    container.RegisterConditional<IDataStorage>(
      Lifestyle.Singleton.CreateRegistration<RemoteDataStorage>(container),
      context => context.Consumer.Target.Parameter?.Position == 1);
    container.RegisterSingleton<ArchiveService>();

    var archiveService = container.GetRequiredService<ArchiveService>();

    archiveService.LocalStorage.Should().BeOfType<LocalDataStorage>();
    archiveService.RemoteStorage.Should().BeOfType<RemoteDataStorage>();
  }
}