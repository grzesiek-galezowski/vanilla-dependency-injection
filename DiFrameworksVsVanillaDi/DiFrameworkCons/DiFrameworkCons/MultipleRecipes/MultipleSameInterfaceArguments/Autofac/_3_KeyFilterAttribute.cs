using Autofac.Features.AttributeFilters;

namespace DiFrameworkCons.MultipleRecipes.MultipleSameInterfaceArguments.Autofac;

public static class _3_KeyFilterAttribute
{
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

  public record ArchiveServiceAttributed(
    [KeyFilter(Storages.Local)] IDataStorage LocalStorage,
    [KeyFilter(Storages.Remote)] IDataStorage RemoteStorage);
}