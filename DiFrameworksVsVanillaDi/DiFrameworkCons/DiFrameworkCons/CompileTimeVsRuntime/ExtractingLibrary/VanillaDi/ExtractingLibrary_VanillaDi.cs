using System.Text.RegularExpressions;
using Castle.Core.Logging;

namespace DiFrameworkCons.CompileTimeVsRuntime.ExtractingLibrary.VanillaDi;

internal class ExtractingLibrary_VanillaDi
{
  [Test]
  public void ShouldAllowEasyExtractionOfLibraryFromCode()
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

