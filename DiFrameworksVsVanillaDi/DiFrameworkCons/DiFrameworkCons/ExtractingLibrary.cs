using System.Text.RegularExpressions;
using Castle.Core.Logging;

namespace DiFrameworkCons;

//todo add more to this example
internal class ExtractingLibrary
{
  [Test]
  public void ShouldAllowEasyExtractionOfLibrary()
  {
    //GIVEN
    //show how to extract library returning project conversion
    ILogger logger = new ConsoleLogger();
    var assetPreprocessor = new AssetPreprocessor(
      new ProjectConversion(
        new JsonFileReader("file.json"),
        new ProjectSectionFormat(),
        new HeaderFormat(new Regex("^.+$")),
        logger)
    );

    //WHEN

    //THEN
  }
}

internal class ProjectConversion
{
  public ProjectConversion(JsonFileReader jsonFileReader, ProjectSectionFormat projectSectionFormat, HeaderFormat headerFormat, ILogger logger)
  {
  }
}

internal class HeaderFormat
{
  public HeaderFormat(Regex regex)
  {
  }
}

internal class ProjectSectionFormat
{
}

internal class JsonFileReader
{
  public JsonFileReader(string fileName)
  {
  }
}

internal class AssetPreprocessor
{
  public AssetPreprocessor(ProjectConversion projectConversion)
  {
    
  }
}