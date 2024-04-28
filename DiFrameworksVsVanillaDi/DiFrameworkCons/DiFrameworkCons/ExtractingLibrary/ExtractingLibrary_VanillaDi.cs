using System.Text.RegularExpressions;
using Castle.Core.Logging;

namespace DiFrameworkCons.ExtractingLibrary;

internal class ExtractingLibrary_VanillaDi
{
  //todo: add description, add simple injector example
  [Test]
  public void ShouldAllowEasyExtractionOfLibraryFromCodeUsingVanillaDi()
  {
    //GIVEN
    //show how to extract library returning project conversion
    var assetPreprocessor = new AssetPreprocessor(
      new ProjectConversion(
        new JsonFileReader("file.json"),
        new ProjectSectionFormat(),
        new HeaderFormat(new Regex("^.+$")),
        new ConsoleLogger())
    );

    //WHEN

    //THEN
    assetPreprocessor.Should().NotBeNull();
  }
}

