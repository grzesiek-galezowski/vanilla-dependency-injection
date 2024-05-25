using System.Collections.Generic;
using SimpleInjector;

public class SimpleInjectorKeyedFactory<T>(Container container)
  where T : class
{
  readonly Dictionary<string, InstanceProducer<T>> _producers =
    new(StringComparer.OrdinalIgnoreCase);

  public T CreateNew(string name) => _producers[name].GetInstance();

  public void Register<TImplementation>(string name)
    where TImplementation : class, T
  {
    _producers.Add(name, Lifestyle.Transient
      .CreateProducer<T, TImplementation>(container));
  }
}
