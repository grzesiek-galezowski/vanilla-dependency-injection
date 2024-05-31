namespace DiFrameworkCons.MultipleRecipes.Decorators._1_Simple.MsDi;

public static class _3_KeyedRegistrations
{
  /// <summary>
  /// Since recently, one can also use keyed registrations
  /// </summary>
  [Test]
  public static void ShouldAssembleDecoratorsUsingKeyedRegistrations()
  {
    var builder = new ServiceCollection();

    builder.AddKeyedTransient<IAnswer, Answer>("1");
    builder.AddKeyedTransient<IAnswer>("2", (x, key) =>
      ActivatorUtilities.CreateInstance<TracedAnswer>(
        x, x.GetRequiredKeyedService<IAnswer>("1")));
    builder.AddKeyedTransient<IAnswer>("3", (x, key) =>
      ActivatorUtilities.CreateInstance<SynchronizedAnswer>(
        x, x.GetRequiredKeyedService<IAnswer>("2"), 1));

    using var container = builder.BuildServiceProvider();
    var answer = container.GetRequiredKeyedService<IAnswer>("3");
    answer.Should().BeOfType<SynchronizedAnswer>();
    answer.NestedAnswer.Should().BeOfType<TracedAnswer>();
    answer.NestedAnswer.NestedAnswer.Should().BeOfType<Answer>();
    ((SynchronizedAnswer)answer).X.Should().Be(1);
  }
}