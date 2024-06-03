using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SimpleInjector.Advanced;

namespace DiFrameworkCons.SimpleInjectorExtensions;

// Custom constructor resolution behavior
public class PerTypeConstructorBehavior(
  Dictionary<Type, List<Type>> mappings,
  IConstructorResolutionBehavior defaultBehavior)
  : IConstructorResolutionBehavior
{
  public ConstructorInfo? TryGetConstructor(
    Type implementationType, out string? errorMessage)
  {
    try
    {
      errorMessage = "OK";
      if (mappings.TryGetValue(implementationType, out var constructorTypes))
      {

        var constructorForTypes = implementationType.GetConstructors()
          .First(c => !c.GetParameters()
            .Select(p => p.ParameterType)
            .Except(constructorTypes).Any());
        return constructorForTypes;
      }
      else
      {
        return defaultBehavior.TryGetConstructor(implementationType, out errorMessage);
      }
    }
    catch (Exception ex)
    {
      errorMessage = ex.Message;
      return null;
    }
  }
}