namespace DiFrameworkCons.MultipleRecipes.DependencyAsMultipleInterfaces;

public class DependencyAsMultipleInterfaces_VanillaDi
{
  /// <summary>
  /// With Vanilla DI, this is business as usual - we can create an object,
  /// assign it to a variable and pass everywhere where an object of compatible
  /// type is expected.
  /// </summary>
  [Test]
  public void ShouldUseOneInstanceForDifferentInterfacesUsingVanillaDi()
  {
    //GIVEN
    var cache = new Cache();

    //WHEN
    var cacheUser = new UserOfReaderAndWriter(cache, cache);

    //THEN
    cacheUser.WriteCache.Should().BeSameAs(cacheUser.ReadCache);
    cacheUser.WriteCache.Number.Should().Be(cacheUser.ReadCache.Number);
  }
}

