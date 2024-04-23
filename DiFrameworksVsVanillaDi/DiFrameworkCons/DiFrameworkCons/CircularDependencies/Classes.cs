namespace DiFrameworkCons.CircularDependencies;

public record One(Two Two);
public record Two(Three Three);
public record Three(One One);