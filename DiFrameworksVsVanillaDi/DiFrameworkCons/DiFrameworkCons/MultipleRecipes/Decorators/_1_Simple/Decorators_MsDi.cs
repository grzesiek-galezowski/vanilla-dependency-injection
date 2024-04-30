namespace DiFrameworkCons.MultipleRecipes.Decorators._1_Simple;

public static class Decorators_MsDi
{
  /// <summary>
  /// MsDi doesn't support decorators (we can always fall back to lambda registration of course).
  ///
  /// Two ways around these limitations are: 1) MediatR, 2) Scrutor (shown below)
  /// </summary>
  [Test]
  public static void ShouldAssembleDecoratorsUsingMsDiWithScrutor()
  {
    var builder = new ServiceCollection();

    builder.AddTransient<IAnswer, Answer>();

    //Decorators are applied in the order they are registered
    builder.Decorate<IAnswer, TracedAnswer>();
    //With MsDi, there's currently no way around falling back lambda
    //(see https://github.com/dotnet/extensions/issues/2937)
    builder.Decorate<IAnswer>((a, ctx) =>
      ActivatorUtilities.CreateInstance<SynchronizedAnswer>(ctx, a, 1));

    using var container = builder.BuildServiceProvider();
    var answer = container.GetRequiredService<IAnswer>();
    answer.Should().BeOfType<SynchronizedAnswer>();
    answer.NestedAnswer.Should().BeOfType<TracedAnswer>();
    answer.NestedAnswer.NestedAnswer.Should().BeOfType<Answer>();
    ((SynchronizedAnswer)answer).X.Should().Be(1);
  }
}