using System.Collections.Generic;
using SimpleInjector;

namespace DiFrameworkCons.SimpleInjectorExtensions;

public class SimpleInjectorNamedFactory<T>(Container container)
  where T : class
{
  readonly Dictionary<string, InstanceProducer<T>> _producers =
    new(StringComparer.OrdinalIgnoreCase);

  public T CreateNew(string name) => _producers[name].GetInstance();

  public void Register<TImplementation>(string name)
    where TImplementation : class, T
  {
    Register<TImplementation>(name, Lifestyle.Transient);
  }

  public void RegisterSingleton<TImplementation>(string name)
    where TImplementation : class, T
  {
    Register<TImplementation>(name, Lifestyle.Singleton);
  }

  public void RegisterSingleton(string name, Func<string, T> instanceCreator)
  {
    Register(name, instanceCreator, Lifestyle.Singleton);
  }

  public void Register(string name, Func<string, T> instanceCreator)
  {
    Register(name, instanceCreator, Lifestyle.Transient);
  }

  private void Register<TImplementation>(string name, Lifestyle lifestyle) where TImplementation : class, T
  {
    _producers.Add(name, lifestyle.CreateProducer<T, TImplementation>(container));
  }

  private void Register(string name, Func<string, T> instanceCreator, Lifestyle lifestyle)
  {
    _producers.Add(name, lifestyle.CreateProducer(() => instanceCreator(name), container));
  }
}