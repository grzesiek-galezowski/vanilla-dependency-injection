namespace DiFrameworkCons.TODO.Events;

public class MyObserver
{
  public void Notify(int value)
  {
    LastReceived = value;
  }

  public int LastReceived { get; set; }
}

public interface IMyDependency
{
  void DoSomething();
  int InstanceId { get; }
}

public class MyDependency : IMyDependency
{
  private static int _lastInstanceId = 0;
  public int InstanceId { get; }

  public MyDependency()
  {
    InstanceId = _lastInstanceId++;
  }

  public event Action<int>? SomeKindOfEvent;

  public void DoSomething()
  {
    SomeKindOfEvent?.Invoke(InstanceId);
  }
}