namespace DiFrameworkCons.MultipleRecipes.MultipleSameInterfaceArguments.VanillaDi;

public class _1_VanillaCode
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