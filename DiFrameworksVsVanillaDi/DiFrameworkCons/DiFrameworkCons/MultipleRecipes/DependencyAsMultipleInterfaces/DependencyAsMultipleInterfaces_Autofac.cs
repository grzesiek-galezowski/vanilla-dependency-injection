namespace DiFrameworkCons.MultipleRecipes.DependencyAsMultipleInterfaces;

public static class DependencyAsMultipleInterfaces_Autofac
{
  /// <summary>
  /// With Autofac, this is also relatively easy - we can use either the .As()
  /// multiple times or just use the .AsImplementedInterfaces().
  /// Autofac doesn't "lose" with Vanilla DI because registering as
  /// multiple interfaces is very straightforward.
  /// </summary>
  [Test]
  public static void ShouldRegisterSingleInstancesWhenRegisteringSingleTypeAsTwoInterfacesUsingAutofac()
  {
    //GIVEN
    var containerBuilder = new ContainerBuilder();
    containerBuilder.RegisterType<UserOfReaderAndWriter>().SingleInstance();
    containerBuilder.RegisterType<Cache>()
      .SingleInstance()
      .AsImplementedInterfaces();

    using var container = containerBuilder.Build();
    //WHEN
    var cacheUser = container.Resolve<UserOfReaderAndWriter>();

    //THEN
    cacheUser.WriteCache.Should().BeSameAs(cacheUser.ReadCache);
    cacheUser.WriteCache.Number.Should().Be(cacheUser.ReadCache.Number);
  }
}