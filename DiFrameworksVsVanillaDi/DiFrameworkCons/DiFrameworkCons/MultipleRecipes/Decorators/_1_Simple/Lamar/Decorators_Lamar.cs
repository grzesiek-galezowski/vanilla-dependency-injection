using Lamar;

namespace DiFrameworkCons.MultipleRecipes.Decorators._1_Simple.Lamar;

public static class Decorators_Lamar
{
  /// <summary>
  /// Lamar supports decorators but not in a way
  /// that would allow passing an integer to the decorator's
  /// constructor. So I used Scrutor as with MsDi.
  /// </summary>
  [Test]
  public static void ShouldAssembleDecoratorsWithScrutor()
  {
    using var container = new Container(builder =>
    {
      builder.AddTransient<IAnswer, Answer>();

      //Decorators are applied in the order they are registered
      builder.Decorate<IAnswer, TracedAnswer>();
      builder.Decorate<IAnswer>((a, ctx) =>
        ActivatorUtilities.CreateInstance<SynchronizedAnswer>(ctx, a, 1));
    });

    var answer = container.GetRequiredService<IAnswer>();
    answer.Should().BeOfType<SynchronizedAnswer>();
    answer.NestedAnswer.Should().BeOfType<TracedAnswer>();
    answer.NestedAnswer.NestedAnswer.Should().BeOfType<Answer>();
    ((SynchronizedAnswer)answer).X.Should().Be(1);
  }
}