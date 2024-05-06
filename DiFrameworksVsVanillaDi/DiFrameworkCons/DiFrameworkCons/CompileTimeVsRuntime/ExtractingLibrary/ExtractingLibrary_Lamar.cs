using System.Text.RegularExpressions;
using Castle.Core.Logging;
using JasperFx.CodeGeneration;
using Lamar;

namespace DiFrameworkCons.CompileTimeVsRuntime.ExtractingLibrary;

public static class ExtractingLibrary_Lamar
{
  [Test]
  public static void ShouldAllowEasyExtractionOfLibraryFromCodeUsingLamar()
  {
    //GIVEN
    using var container = new Container(builder =>
    {
      builder.AddTransient<AssetPreprocessor>();
      builder.AddTransient<ProjectConversion>();
      builder.AddTransient<JsonFileReader>();
      builder.ForConcreteType<JsonFileReader>().Configure
        .Ctor<string>().Is("file.json");
      builder.AddTransient<ProjectSectionFormat>();
      builder.ForConcreteType<HeaderFormat>().Configure
        .Ctor<Regex>().Is(new Regex("^.+$"));
      builder.AddTransient<ILogger, ConsoleLogger>();
    });

    //THEN
    // Lamar can describe build plan for the code we intend to extract.
    // The code is not super-readable but can be of great help
    // in restoring the manual composition of to-be-extracted subgraph
    container.Model.For<ProjectConversion>().Default.DescribeBuildPlan().Should()
      .NotBeNull();

    container.GetRequiredService<AssetPreprocessor>().Should().NotBeNull();
  }
}