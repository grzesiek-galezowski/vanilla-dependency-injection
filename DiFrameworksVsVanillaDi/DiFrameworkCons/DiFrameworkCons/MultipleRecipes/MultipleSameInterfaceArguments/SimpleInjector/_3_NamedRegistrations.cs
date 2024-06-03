using DiFrameworkCons.SimpleInjectorExtensions;
using SimpleInjector;

namespace DiFrameworkCons.MultipleRecipes.MultipleSameInterfaceArguments.SimpleInjector;

public static class _3_NamedRegistrations
{
  [Test]
  public static void ContainerCompositionThroughNamedRegistrations()
  {
    using var container = new Container();
    container.NamedRegistrations<IDataStorage>(c =>
    {
      c.Register<LocalDataStorage>("local");
      c.Register<RemoteDataStorage>("remote");
    });
    container.RegisterSingleton(() =>
      new ArchiveService(
        container.GetNamedService<IDataStorage>("local"),
        container.GetNamedService<IDataStorage>("remote")));

    var archiveService = container.GetRequiredService<ArchiveService>();

    archiveService.LocalStorage.Should().BeOfType<LocalDataStorage>();
    archiveService.RemoteStorage.Should().BeOfType<RemoteDataStorage>();
  }
}