using System.Collections.Generic;

namespace DiFrameworkPros._2_LifetimeScopeManagement;

internal class DisposableDependency(Log log) : GenericDisposableDependency<object>(log);
internal class DisposableDependency2(Log log) : GenericDisposableDependency<int>(log);
internal class DisposableDependency3(Log log) : GenericDisposableDependency<string>(log);

internal class GenericDisposableDependency<T> : IDisposable
{
  private readonly Log _log;
  private readonly int _currentId;

  public GenericDisposableDependency(Log log)
  {
    _log = log;
    _currentId = log.NextId();
    _log.Created(_currentId);
  }

  public void Dispose()
  {
    _log.Disposed(_currentId);
  }
}

internal class Log
{
  public readonly List<string> Entries = [];
  private int _counter = 0;

  public void Write(string text)
  {
    TestContext.Progress.WriteLine(text);
    Entries.Add(text);
  }

  public int NextId()
  {
    return _counter++;
  }

  public void Created(int currentId)
  {
    Write("_____CREATED______" + currentId);
  }

  public void Disposed(int currentId)
  {
    Write("_____DISPOSED______" + currentId);
  }

  public void OpeningScope()
  {
    Write("opening scope");
  }

  public void ClosingScope()
  {
    Write("closing scope");
  }

  public void ClosedScope()
  {
    Write("closed scope");
  }
}