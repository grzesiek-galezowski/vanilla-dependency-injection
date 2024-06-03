using SimpleInjector;

namespace DiFrameworkCons.SimpleInjectorExtensions;

public static class SimpleInjectorNamedExtensions
{
  public static T GetNamedService<T>(this Container container, string key) where T : class
  {
    return container.GetRequiredService<SimpleInjectorNamedFactory<T>>().CreateNew(key);
  }

  public static void NamedRegistrations<T>(this Container container, Action<SimpleInjectorNamedFactory<T>> action) where T : class
  {
    var factory = new SimpleInjectorNamedFactory<T>(container);
    action.Invoke(factory);
    container.RegisterInstance(factory);
  }
}