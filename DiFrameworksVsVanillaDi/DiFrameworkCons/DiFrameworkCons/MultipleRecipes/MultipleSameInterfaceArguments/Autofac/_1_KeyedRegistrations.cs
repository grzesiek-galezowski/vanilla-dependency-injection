using Autofac.Core;

namespace DiFrameworkCons.MultipleRecipes.MultipleSameInterfaceArguments.Autofac;

public static class _1_KeyedRegistrations
{
  [Test]
  public static void ContainerCompositionThroughResolveKeyedComponents()
  {
    var containerBuilder = new ContainerBuilder();
    containerBuilder.RegisterType<LocalDataStorage>().Keyed<IDataStorage>(Storages.Local);
    containerBuilder.RegisterType<RemoteDataStorage>().Keyed<IDataStorage>(Storages.Remote);
    containerBuilder.RegisterType<ArchiveService>();

    using var container = containerBuilder.Build();
    var archiveService = container.Resolve<ArchiveService>(
      new ResolvedParameter(
        (info, context) => info.Name == "LocalStorage",
        (info, context) => context.ResolveKeyed<IDataStorage>(Storages.Local)),
      new ResolvedParameter(
        (info, context) => info.Name == "RemoteStorage",
        (info, context) => context.ResolveKeyed<IDataStorage>(Storages.Remote))
    );
    archiveService.LocalStorage.Should().BeOfType<LocalDataStorage>();
    archiveService.RemoteStorage.Should().BeOfType<RemoteDataStorage>();
  }
}