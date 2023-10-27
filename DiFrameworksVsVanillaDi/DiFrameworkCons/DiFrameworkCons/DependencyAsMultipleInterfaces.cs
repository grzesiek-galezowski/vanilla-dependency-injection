namespace DiFrameworkCons;

public class DependencyAsMultipleInterfaces
{
  [Test]
  public void ShouldRegisterSingleInstancesWhenRegisteringSingleTypeAsTwoInterfaces()
  {
    //GIVEN
    var containerBuilder = new ContainerBuilder();
    containerBuilder.RegisterType<Cache>()
      .SingleInstance()
      .AsImplementedInterfaces();

    using var container = containerBuilder.Build();
    //WHEN
    var readCache = container.Resolve<IReadCache>();
    var writeCache = container.Resolve<IWriteCache>();

    //THEN
    Assert.AreEqual(readCache.Number, writeCache.Number);
    Assert.AreSame(readCache, writeCache);
  }
}

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