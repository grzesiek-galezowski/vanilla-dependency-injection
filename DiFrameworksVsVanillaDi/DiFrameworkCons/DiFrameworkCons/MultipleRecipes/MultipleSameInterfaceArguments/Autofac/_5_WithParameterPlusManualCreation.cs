using Autofac.Core;
using Autofac.Features.AttributeFilters;

namespace DiFrameworkCons.MultipleRecipes.MultipleSameInterfaceArguments.Autofac;

public static class _5_WithParameterPlusManualCreation
{
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