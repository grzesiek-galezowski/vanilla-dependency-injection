namespace DiFrameworkCons.DependencyAsMultipleInterfaces;

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