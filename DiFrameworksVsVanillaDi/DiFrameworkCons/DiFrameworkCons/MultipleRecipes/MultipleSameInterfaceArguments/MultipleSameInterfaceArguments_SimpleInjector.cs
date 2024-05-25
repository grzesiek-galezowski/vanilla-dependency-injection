using SimpleInjector;

namespace DiFrameworkCons.MultipleRecipes.MultipleSameInterfaceArguments;

public static class MultipleSameInterfaceArguments_SimpleInjector
{
  [Test]
  public static void ContainerCompositionThroughConditionallyRegisteredComponents()
  {
    using var container = new Container();
    container.RegisterConditional<IDataStorage>(
      Lifestyle.Singleton.CreateRegistration<LocalDataStorage>(container),
      context => context.Consumer.Target.Parameter?.Position == 0);
    container.RegisterConditional<IDataStorage>(
      Lifestyle.Singleton.CreateRegistration<RemoteDataStorage>(container),
      context => context.Consumer.Target.Parameter?.Position == 1);
    container.RegisterSingleton<ArchiveService>();

    var archiveService = container.GetRequiredService<ArchiveService>();

    archiveService.LocalStorage.Should().BeOfType<LocalDataStorage>();
    archiveService.RemoteStorage.Should().BeOfType<RemoteDataStorage>();
  }

  [Test]
  public static void ContainerCompositionThroughResolvingSpecificTypes()
  {
    using var container = new Container();

    container.Register<LocalDataStorage>();
    container.Register<RemoteDataStorage>();
    container.RegisterSingleton(() =>
      new ArchiveService(
        container.GetRequiredService<LocalDataStorage>(),
        container.GetRequiredService<RemoteDataStorage>()));

    var archiveService = container.GetRequiredService<ArchiveService>();

    archiveService.LocalStorage.Should().BeOfType<LocalDataStorage>();
    archiveService.RemoteStorage.Should().BeOfType<RemoteDataStorage>();
  }


  [Test]
  public static void ContainerCompositionThroughKeyedComponents()
  {
    using var container = new Container();
    var factory = new SimpleInjectorKeyedFactory<IDataStorage>(container);
    factory.Register<LocalDataStorage>("local");
    factory.Register<RemoteDataStorage>("remote");
    container.RegisterInstance(factory);
    container.RegisterSingleton(() =>
      new ArchiveService(
        factory.CreateNew("local"),
        factory.CreateNew("remote")));

    var archiveService = container.GetRequiredService<ArchiveService>();

    archiveService.LocalStorage.Should().BeOfType<LocalDataStorage>();
    archiveService.RemoteStorage.Should().BeOfType<RemoteDataStorage>();
  }
}