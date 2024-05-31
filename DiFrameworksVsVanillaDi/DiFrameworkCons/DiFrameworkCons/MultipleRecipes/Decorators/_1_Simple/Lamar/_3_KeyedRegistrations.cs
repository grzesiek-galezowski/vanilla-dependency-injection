using Lamar;

namespace DiFrameworkCons.MultipleRecipes.Decorators._1_Simple.Lamar;

public static class _3_KeyedRegistrations
{
  /// <summary>
  /// Both MsDi-style keyed registrations and built-in Lamar keyed registrations
  /// can also be used.
  /// </summary>
  [Test]
  public static void ShouldAssembleDecoratorsUsingKeyedRegistrations()
  {
    using var container = new Container(builder =>
    {
      builder.AddKeyedTransient<IAnswer, Answer>("1");
      builder.AddKeyedTransient<IAnswer>("2", (x, key) =>
        ActivatorUtilities.CreateInstance<TracedAnswer>(
          x, x.GetRequiredKeyedService<IAnswer>("1")));
      builder.AddKeyedTransient<IAnswer>("3", (x, key) =>
        ActivatorUtilities.CreateInstance<SynchronizedAnswer>(
          x, x.GetRequiredKeyedService<IAnswer>("2"), 1));
    });

    var answer = container.GetRequiredKeyedService<IAnswer>("3");
    answer.Should().BeOfType<SynchronizedAnswer>();
    answer.NestedAnswer.Should().BeOfType<TracedAnswer>();
    answer.NestedAnswer.NestedAnswer.Should().BeOfType<Answer>();
    ((SynchronizedAnswer)answer).X.Should().Be(1);
  }
}