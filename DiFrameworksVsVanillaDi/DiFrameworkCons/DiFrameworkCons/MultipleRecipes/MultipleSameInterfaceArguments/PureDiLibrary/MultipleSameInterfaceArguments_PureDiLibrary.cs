using Pure.DI;

namespace DiFrameworkCons.MultipleRecipes.MultipleSameInterfaceArguments;

public class MultipleSameInterfaceArguments_PureDiLibrary
{
  [Test]
  public void ContainerCompositionThroughTaggedComponents()
  {
    var composition = new Composition14();
    var archiveService = composition.ArchiveService;

    archiveService.LocalStorage.Should().BeOfType<LocalDataStorage>();
    archiveService.RemoteStorage.Should().BeOfType<RemoteDataStorage>();
  }
}

partial class Composition14
{
  public void Setup()
  {
    DI.Setup(nameof(Composition14))
      .RootBind<ArchiveService>("ArchiveService").To(context =>
      {
        context.Inject<IDataStorage>("local", out var local);
        context.Inject<IDataStorage>("remote", out var remote);
        return new ArchiveService(local, remote);
      })
      .Bind<IDataStorage>("local").As(Lifetime.Singleton).To<LocalDataStorage>()
      .Bind<IDataStorage>("remote").As(Lifetime.Singleton).To<RemoteDataStorage>();
  }
}