namespace DiFrameworkCons.MultipleRecipes.Decorators._1_Simple.VanillaDi;

public class _1_VanillaCode
{
  /// <summary>
  /// In Vanilla DI, this is pretty straightforward. It's really no different
  /// from any other composition - just create objects and pass one object
  /// as an argument of another's constructor.
  /// </summary>
  [Test]
  public void ShouldAssembleDecoratorsUsingVanillaDi()
  {
    var answer = new SynchronizedAnswer(new TracedAnswer(new Answer()), 1);

    answer.Should().BeOfType<SynchronizedAnswer>();
    answer.NestedAnswer.Should().BeOfType<TracedAnswer>();
    answer.NestedAnswer.NestedAnswer.Should().BeOfType<Answer>();
    answer.X.Should().Be(1);
  }

}