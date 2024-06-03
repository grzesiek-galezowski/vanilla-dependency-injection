using Autofac.Core;

namespace DiFrameworkCons.MultipleRecipes.MultipleSameInterfaceArguments.Autofac;

public static class _2_ResolvedKeyedParameters
{
  [Test]
  public static void ContainerCompositionThroughRegistrationOfResolvedParameters()
  {
    var containerBuilder = new ContainerBuilder();
    containerBuilder.RegisterType<LocalDataStorage>().Keyed<IDataStorage>(Storages.Local);
    containerBuilder.RegisterType<RemoteDataStorage>().Keyed<IDataStorage>(Storages.Remote);
    containerBuilder.RegisterType<ArchiveService>()
      .WithParameter(new ResolvedParameter(
        (info, context) => info.Name == "LocalStorage",
        (info, context) => context.ResolveKeyed<IDataStorage>(Storages.Local)))
      .WithParameter(new ResolvedParameter(
        (info, context) => info.Name == "RemoteStorage",
        (info, context) => context.ResolveKeyed<IDataStorage>(Storages.Remote)));

    using var container = containerBuilder.Build();
    var archiveService = container.Resolve<ArchiveService>();
    archiveService.LocalStorage.Should().BeOfType<LocalDataStorage>();
    archiveService.RemoteStorage.Should().BeOfType<RemoteDataStorage>();
  }
}