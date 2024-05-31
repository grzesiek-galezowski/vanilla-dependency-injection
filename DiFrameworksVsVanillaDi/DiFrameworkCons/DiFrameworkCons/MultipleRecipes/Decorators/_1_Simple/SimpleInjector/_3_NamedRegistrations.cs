using SimpleInjector;

namespace DiFrameworkCons.MultipleRecipes.Decorators._1_Simple.SimpleInjector;

public static class _3_NamedRegistrations
{
  /// <summary>
  /// If you implement a named registration feature by yourself,
  /// you can use it as well.
  /// </summary>
  [Test]
  public static void ShouldAssembleDecoratorsUsingKeyedRegistrations()
  {
    var container = new Container();
    container.NamedRegistrations<IAnswer>(c =>
    {
      c.Register("answer",
        _ => ActivatorUtilities.CreateInstance<Answer>(container));
      c.Register("traced",
        _ => ActivatorUtilities.CreateInstance<TracedAnswer>(
          container,
          container.GetNamedService<IAnswer>("answer")));
    });
    container.Register<IAnswer>(
      () => ActivatorUtilities.CreateInstance<SynchronizedAnswer>(
        container,
        container.GetNamedService<IAnswer>("traced"), 1));


    var answer = container.GetInstance<IAnswer>();
    answer.Should().BeOfType<SynchronizedAnswer>();
    answer.NestedAnswer.Should().BeOfType<TracedAnswer>();
    answer.NestedAnswer.NestedAnswer.Should().BeOfType<Answer>();
    ((SynchronizedAnswer)answer).X.Should().Be(1);
  }
}