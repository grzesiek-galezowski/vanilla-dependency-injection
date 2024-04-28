namespace DiFrameworkCons.MultipleConstructors;

public class ObjectWithTwoConstructors
{
  public readonly ConstructorArgument Arg;

  public ObjectWithTwoConstructors(Constructor1Argument arg)
  {
    Arg = arg;
  }
  public ObjectWithTwoConstructors(Constructor2Argument arg)
  {
    Arg = arg;
  }
}

public interface ConstructorArgument;

public record Constructor1Argument : ConstructorArgument;
public record Constructor2Argument : ConstructorArgument;