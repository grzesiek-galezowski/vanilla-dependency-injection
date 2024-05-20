using Autofac.Core;
using Autofac.Features.AttributeFilters;

namespace DiFrameworkCons.MultipleRecipes.MultipleSameInterfaceArguments;

public static class MultipleSameInterfaceArguments_Autofac
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

  /// <summary>
  /// "Note that some relationships are based on types that are in Autofac.
  /// Using those relationship types do tie you to at least having a reference to Autofac, even
  /// if you choose to use a different DI framework for the actual resolution of services." 
  /// </summary>
  [Test]
  public static void ContainerCompositionWithAttributes()
  {
    var containerBuilder = new ContainerBuilder();
    containerBuilder.RegisterType<LocalDataStorage>().Keyed<IDataStorage>(Storages.Local);
    containerBuilder.RegisterType<RemoteDataStorage>().Keyed<IDataStorage>(Storages.Remote);
    containerBuilder.RegisterType<ArchiveServiceAttributed>().WithAttributeFiltering();

    using var container = containerBuilder.Build();
    var archiveService = container.Resolve<ArchiveServiceAttributed>();
    archiveService.LocalStorage.Should().BeOfType<LocalDataStorage>();
    archiveService.RemoteStorage.Should().BeOfType<RemoteDataStorage>();
  }

  [Test]
  //also: keyed (below) and indexed (and named parameters for constants)
  public static void ContainerCompositionWithNamed()
  {
    var containerBuilder = new ContainerBuilder();
    containerBuilder.RegisterType<LocalDataStorage>().Named<IDataStorage>("local");
    containerBuilder.RegisterType<RemoteDataStorage>().Named<IDataStorage>("remote");
    containerBuilder.Register(c =>
      new ArchiveService(
        c.ResolveNamed<IDataStorage>("local"),
        c.ResolveNamed<IDataStorage>("remote")));

    using var container = containerBuilder.Build();
    var archiveService = container.Resolve<ArchiveService>();
    archiveService.LocalStorage.Should().BeOfType<LocalDataStorage>();
    archiveService.RemoteStorage.Should().BeOfType<RemoteDataStorage>();
  }

  [Test]
  public static void ContainerCompositionThroughNamedParameters()
  {
    //only for up-front known values and constants - cannot resolve from container each time
    //prone to name change(?)
    var containerBuilder = new ContainerBuilder();
    containerBuilder.RegisterType<ArchiveService>()
      .WithParameter("LocalStorage", new LocalDataStorage())
      .WithParameter("RemoteStorage", new RemoteDataStorage());

    using var container = containerBuilder.Build();
    var archiveService = container.Resolve<ArchiveService>();

    archiveService.LocalStorage.Should().BeOfType<LocalDataStorage>();
    archiveService.RemoteStorage.Should().BeOfType<RemoteDataStorage>();
  }
}