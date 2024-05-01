using System.Collections.Generic;
using Castle.DynamicProxy;

namespace DiFrameworkPros._3_Interception;

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

public class CallLogger(List<string> messages) : IInterceptor
{
  public void Intercept(IInvocation invocation)
  {
    var message = "Called " + invocation.Method.Name;
    Console.WriteLine(message);
    messages.Add(message);
    invocation.Proceed();
  }
}