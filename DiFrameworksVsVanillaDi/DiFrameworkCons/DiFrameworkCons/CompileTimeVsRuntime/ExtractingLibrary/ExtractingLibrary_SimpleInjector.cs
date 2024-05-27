using System.Text.RegularExpressions;
using Castle.Core.Logging;
using SimpleInjector;

namespace DiFrameworkCons.CompileTimeVsRuntime.ExtractingLibrary;

public static class ExtractingLibrary_SimpleInjector
{
  [Test]
  public static void ShouldAllowEasyExtractionOfLibraryFromCode()
  {
    //GIVEN
    using var container = new Container();
    container.Register<AssetPreprocessor>();
    container.Register<ProjectConversion>();
    container.Register(() => new JsonFileReader("file.json"));
    container.Register<ProjectSectionFormat>();
    container.Register(() => new HeaderFormat(new Regex("^.+$")));

    // by default, SimpleInjector prevents from registering
    // a class with multiple constructor parameters.
    // https://docs.simpleinjector.org/en/latest/extensibility.html#overriding-constructor-resolution-behavior
    // shows how to override it but it's much more work than a lambda
    // and besides, SimpleInjector verification is based on resolution,
    // so we should be fine here.
    container.Register<ILogger>(() => new ConsoleLogger());

    //WHEN
    container.Verify();

    //THEN
    container.GetRequiredService<AssetPreprocessor>().Should().NotBeNull();

    // SimpleInjector can describe visualize object graph to a degree,
    // giving some hints on how to construct it manually.
    // It seems to not like lambdas. No wonder.
    container.GetRegistration<ProjectConversion>()
      !.VisualizeObjectGraph(new VisualizationOptions()
      {
        IncludeLifestyleInformation = true,
        UseFullyQualifiedTypeNames = false
      }).Should().Be(
        @"ProjectConversion( // Transient
    JsonFileReader(), // Transient
    ProjectSectionFormat(), // Transient
    HeaderFormat(), // Transient
    ILogger()) // Transient");
  }
}