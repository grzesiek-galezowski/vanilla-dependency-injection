using Castle.DynamicProxy;

public interface IDependency
{
  void DoSomething();
}

public class Dependency : IDependency
{
  public void DoSomething()
  {

  }
}

public class CallLogger : IInterceptor
{
  public void Intercept(IInvocation invocation)
  {
    Console.WriteLine("Called " + invocation.Method.Name);
    invocation.Proceed();
  }
}