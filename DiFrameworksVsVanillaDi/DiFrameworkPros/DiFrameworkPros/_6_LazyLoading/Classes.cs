using System.Threading;

public interface IVerySlowDependency;

class VerySlowDependency : IVerySlowDependency
{
  public VerySlowDependency()
  {
    Thread.Sleep(TimeSpan.FromSeconds(10));
  }
}

public interface IControllerINeed;

public class ControllerINeed : IControllerINeed;

public interface IControllerIDoNotNeed;

class ControllerIDoNotNeedButItNeedsSlowDependency : IControllerIDoNotNeed
{
  public ControllerIDoNotNeedButItNeedsSlowDependency(IVerySlowDependency dependency)
  {
  }
}