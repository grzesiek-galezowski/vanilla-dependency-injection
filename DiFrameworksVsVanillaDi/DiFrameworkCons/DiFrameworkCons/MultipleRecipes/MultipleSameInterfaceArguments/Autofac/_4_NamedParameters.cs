namespace DiFrameworkCons.MultipleRecipes.MultipleSameInterfaceArguments.Autofac;

public static class _4_NamedParameters
{
  [Test]
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
}