namespace DiFrameworkCons.MultipleRecipes.MultipleSameInterfaceArguments.MsDi;

public static class _1_KeyedRegistrations
{
  [Test]
  public static void ContainerCompositionThroughKeyedComponents()
  {
    var containerBuilder = new ServiceCollection();
    containerBuilder.AddKeyedTransient<IDataStorage, LocalDataStorage>(Storages.Local);
    containerBuilder.AddKeyedTransient<IDataStorage, RemoteDataStorage>(Storages.Remote);
    containerBuilder.AddTransient(x =>
      ActivatorUtilities.CreateInstance<ArchiveService>(x,
        x.GetRequiredKeyedService<IDataStorage>(Storages.Local),
        x.GetRequiredKeyedService<IDataStorage>(Storages.Remote)));

    using var container = containerBuilder.BuildServiceProvider();
    var archiveService = container.GetRequiredService<ArchiveService>();

    archiveService.LocalStorage.Should().BeOfType<LocalDataStorage>();
    archiveService.RemoteStorage.Should().BeOfType<RemoteDataStorage>();
  }
}