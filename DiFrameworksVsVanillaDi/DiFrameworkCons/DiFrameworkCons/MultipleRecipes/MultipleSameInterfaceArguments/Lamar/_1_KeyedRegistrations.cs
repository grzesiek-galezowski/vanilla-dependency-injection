using Lamar;

namespace DiFrameworkCons.MultipleRecipes.MultipleSameInterfaceArguments.Lamar;

public static class _1_KeyedRegistrations
{
  [Test]
  public static void ContainerCompositionThroughKeyedComponents()
  {
    using var container = new Container(containerBuilder =>
    {
      containerBuilder.AddKeyedTransient<IDataStorage, LocalDataStorage>(Storages.Local);
      containerBuilder.AddKeyedTransient<IDataStorage, RemoteDataStorage>(Storages.Remote);
      containerBuilder.AddTransient(x =>
        ActivatorUtilities.CreateInstance<ArchiveService>(x,
          x.GetRequiredKeyedService<IDataStorage>(Storages.Local),
          x.GetRequiredKeyedService<IDataStorage>(Storages.Remote)));
    });

    var archiveService = container.GetRequiredService<ArchiveService>();

    archiveService.LocalStorage.Should().BeOfType<LocalDataStorage>();
    archiveService.RemoteStorage.Should().BeOfType<RemoteDataStorage>();
  }
}