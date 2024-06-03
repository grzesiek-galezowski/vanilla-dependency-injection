namespace DiFrameworkCons.MultipleRecipes.MultipleLifestylesOfInstancesTheSameClass;

//todo add descriptions
internal class MultipleLifestylesOfInstancesTheSameClass_VanillaDi
{
  [Test]
  public void ShouldComposeWithTwoLifestylesOfThrottledOutbox()
  {
    //GIVEN
    var o = new ThrottledOutbox();
    var p1 = new OnDemandProcess(o);
    var p2 = new ScheduledProcess(o);
    var p3 = new EmergencyProcess(
      new ThrottledOutbox()
    );

    //THEN
    p1.ThrottledOutbox.Should().BeSameAs(p2.ThrottledOutbox);
    p3.ThrottledOutbox.Should().NotBeSameAs(p2.ThrottledOutbox);
    p3.ThrottledOutbox.Should().NotBeSameAs(p1.ThrottledOutbox);
  }
}

