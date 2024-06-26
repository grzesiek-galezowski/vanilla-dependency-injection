namespace DiFrameworkCons.MultipleRecipes.Decorators._1_Simple.MsDi;

public static class _1_Scrutor
{
  /// <summary>
  /// MsDi doesn't support decorators (we can always fall back to lambda registration of course).
  ///
  /// Two ways around these limitations are: 1) MediatR, 2) Scrutor (shown below)
  /// </summary>
  [Test]
  public static void ShouldAssembleDecoratorsWithScrutor()
  {
    var builder = new ServiceCollection();

    builder.AddTransient<IAnswer, Answer>();

    //Decorators are applied in the order they are registered
    builder.Decorate<IAnswer, TracedAnswer>();
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