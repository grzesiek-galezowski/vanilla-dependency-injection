using SimpleInjector;

namespace DiFrameworkCons.MultipleRecipes.MultipleSameInterfaceArguments.SimpleInjector;

public static class _2_ResolutionOfSpecificTypes
{
  [Test]
  public static void ContainerCompositionThroughResolvingSpecificTypes()
  {
    using var container = new Container();

    container.Register<LocalDataStorage>();
    container.Register<RemoteDataStorage>();
    container.RegisterSingleton(() =>
      new ArchiveService(
        container.GetRequiredService<LocalDataStorage>(),
        container.GetRequiredService<RemoteDataStorage>()));

    var archiveService = container.GetRequiredService<ArchiveService>();

    archiveService.LocalStorage.Should().BeOfType<LocalDataStorage>();
    archiveService.RemoteStorage.Should().BeOfType<RemoteDataStorage>();
  }
}