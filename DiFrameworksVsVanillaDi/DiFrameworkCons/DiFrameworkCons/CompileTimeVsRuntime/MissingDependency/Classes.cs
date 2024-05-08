namespace DiFrameworkCons.CompileTimeVsRuntime.MissingDependency;

public interface ITwo;
public record One(ITwo Two);
public record Two : ITwo;