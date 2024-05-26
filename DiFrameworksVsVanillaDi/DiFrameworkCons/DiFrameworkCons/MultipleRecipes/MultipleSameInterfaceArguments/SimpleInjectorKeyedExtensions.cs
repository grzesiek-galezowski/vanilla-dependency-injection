using SimpleInjector;

namespace DiFrameworkCons.MultipleRecipes.MultipleSameInterfaceArguments;

public static class SimpleInjectorKeyedExtensions
{
  public static T GetNamedService<T>(this Container container, string key) where T : class
  {
    return container.GetRequiredService<SimpleInjectorKeyedFactory<T>>().CreateNew(key);
  }

  public static void NamedRegistrations<T>(this Container container, Action<SimpleInjectorKeyedFactory<T>> action) where T : class
  {
    var factory = new SimpleInjectorKeyedFactory<T>(container);
    action.Invoke(factory);
    container.RegisterInstance(factory);
  }
}