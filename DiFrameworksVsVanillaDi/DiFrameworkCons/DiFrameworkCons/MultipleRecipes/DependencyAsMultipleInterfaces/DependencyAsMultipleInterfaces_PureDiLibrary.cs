using Pure.DI;

namespace DiFrameworkCons.MultipleRecipes.DependencyAsMultipleInterfaces;

public class DependencyAsMultipleInterfaces_PureDiLibrary
{
  [Test]
  public void ShouldUseOneInstanceForDifferentInterfaces()
  {
    //GIVEN
    var composition = new Composition9();

    //WHEN
    var cacheUser = composition.Root;

    //THEN
    cacheUser.WriteCache.Should().BeSameAs(cacheUser.ReadCache);
    cacheUser.WriteCache.Number.Should().Be(cacheUser.ReadCache.Number);
  }
}

public partial class Composition9
{
  public void Setup()
  {
    DI.Setup(nameof(Composition9))
      .Bind<Cache>().As(Lifetime.Singleton).To<Cache>()
      .Bind<IReadCache>().As(Lifetime.Singleton).To(
        context =>
        {
          //tempting to extract a helper method
          //but this breaks code generation...
          context.Inject<Cache>(out var cache);
          return cache;
        })
      .Bind<IWriteCache>().As(Lifetime.Singleton).To(
        context =>
        {
          context.Inject<Cache>(out var cache);
          return cache;
        })
      .Bind<UserOfReaderAndWriter>().To<UserOfReaderAndWriter>()
      .Root<UserOfReaderAndWriter>("Root");
  }
}

