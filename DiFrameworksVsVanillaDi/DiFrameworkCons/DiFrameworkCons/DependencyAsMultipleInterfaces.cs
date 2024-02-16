namespace DiFrameworkCons;

//todo add description
public class DependencyAsMultipleInterfaces
{
  [Test]
  public void ShouldUseOneInstanceForDifferentInterfacesUsingVanillaDi()
  {
    //GIVEN
    var cache = new Cache();

    //WHEN
    var cacheUser = new UserOfReaderAndWriter(cache, cache);

    //THEN
    Assert.AreSame(cacheUser.ReadCache, cacheUser.WriteCache);
    Assert.AreEqual(cacheUser.ReadCache.Number, cacheUser.WriteCache.Number);
  }

  [Test]
  public void ShouldRegisterSingleInstancesWhenRegisteringSingleTypeAsTwoInterfacesUsingAutofac()
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
    Assert.AreSame(cacheUser.ReadCache, cacheUser.WriteCache);
    Assert.AreEqual(cacheUser.ReadCache.Number, cacheUser.WriteCache.Number);
  }

  [Test]
  public void ShouldRegisterSingleInstancesWhenRegisteringSingleTypeAsTwoInterfacesUsingMsDi()
  {
    //GIVEN
    var builder = new ServiceCollection();
    builder.AddSingleton<UserOfReaderAndWriter>();
    builder.AddSingleton<Cache>();
    builder.AddSingleton<IReadCache>(c => c.GetRequiredService<Cache>());
    builder.AddSingleton<IWriteCache>(c => c.GetRequiredService<Cache>());

    using var container = builder.BuildServiceProvider();
    //WHEN
    var cacheUser = container.GetRequiredService<UserOfReaderAndWriter>();

    //THEN
    Assert.AreSame(cacheUser.ReadCache, cacheUser.WriteCache);
    Assert.AreEqual(cacheUser.ReadCache.Number, cacheUser.WriteCache.Number);
  }
}

public record UserOfReaderAndWriter(IReadCache ReadCache, IWriteCache WriteCache);

public interface IWriteCache
{
  public int Number { get; }
}

public interface IReadCache
{
  public int Number { get; }
}

public record Cache : IWriteCache, IReadCache
{
  public Cache()
  {
    Number = _num++;
  }

  private static int _num = 1;
  public int Number { get; } = _num;
};