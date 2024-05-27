using System.Text.RegularExpressions;
using Castle.Core.Logging;

namespace DiFrameworkCons.CompileTimeVsRuntime.ExtractingLibrary;

public static class ExtractingLibrary_MsDi
{
  [Test]
  public static void ShouldAllowEasyExtractionOfLibraryFromCode()
  {
    //GIVEN
    var builder = new ServiceCollection();
    builder.AddTransient<AssetPreprocessor>();
    builder.AddTransient<ProjectConversion>();
    builder.AddTransient(ctx => ActivatorUtilities.CreateInstance<JsonFileReader>(ctx, "file.json"));
    builder.AddTransient<ProjectSectionFormat>();
    builder.AddTransient(ctx => ActivatorUtilities.CreateInstance<HeaderFormat>(ctx, new Regex("^.+$")));
    builder.AddTransient<ILogger, ConsoleLogger>();

    //WHEN
    using var container = builder.BuildServiceProvider();

    //THEN
    container.GetRequiredService<AssetPreprocessor>().Should().NotBeNull();
  }
}