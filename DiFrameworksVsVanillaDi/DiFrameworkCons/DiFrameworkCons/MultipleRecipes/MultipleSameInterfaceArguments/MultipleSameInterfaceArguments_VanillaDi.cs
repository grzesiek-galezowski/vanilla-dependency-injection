//todo add descriptions
//todo object encapsulation (check if this is possible in a container)
//it is to some extent by using child scopes(?) but that makes these places dependent on the container

namespace DiFrameworkCons.MultipleRecipes.MultipleSameInterfaceArguments;

public class MultipleSameInterfaceArguments_VanillaDi
{
  [Test]
  public void HandMadeComposition()
  {
    var archiveService = new ArchiveService(
        new LocalDataStorage(),
        new RemoteDataStorage());

    Console.WriteLine(archiveService);
  }
}