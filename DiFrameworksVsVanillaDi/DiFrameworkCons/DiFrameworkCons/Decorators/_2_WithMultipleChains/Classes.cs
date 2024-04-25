namespace DiFrameworkCons.Decorators._2_WithMultipleChains;

public interface IComponent
{
  IComponent? Next { get; }
}

public record A(IComponent Next) : IComponent;
public record B(IComponent Next) : IComponent;
public record C1(IComponent Next) : IComponent;
public record C2(IComponent Next) : IComponent;
public record D : IComponent
{
  public IComponent? Next { get; }
}