using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Castle.Core.Logging;
using FluentAssertions;

namespace DiFrameworkCons;

//todo: add description, add simple injector example
/// <summary>
/// This example is supposed to show that it's easier to extract a library from
/// manually composed code - jest extract the subgraph into a new method,
/// extract some elements as arguments.
///
/// It's much easier to do this with the DI container. That's because, unless
/// the library is an extension to a particular container or asp.net core,
/// we don't want the library to be coupled to specific container because
/// users of other containers or users of Vanilla DI will be in trouble.
/// </summary>
internal class ExtractingLibrary
{
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

  [Test]
  public void ShouldAllowEasyExtractionOfLibraryFromCodeUsingAutofac()
  {
    //GIVEN
    var builder = new ContainerBuilder();
    builder.RegisterType<AssetPreprocessor>().InstancePerDependency();
    builder.RegisterType<ProjectConversion>().InstancePerDependency();
    builder.RegisterType<JsonFileReader>().InstancePerDependency()
      .WithParameter(new TypedParameter(typeof(string), "file.json"));
    builder.RegisterType<ProjectSectionFormat>().InstancePerDependency();
    builder.RegisterType<HeaderFormat>().InstancePerDependency()
      .WithParameter(new TypedParameter(typeof(Regex), new Regex("^.+$")));
    builder.RegisterType<ConsoleLogger>().As<ILogger>().InstancePerDependency();

    //WHEN
    using var container = builder.Build();

    //THEN
    container.Resolve<AssetPreprocessor>().Should().NotBeNull();
  }

  [Test]
  public void ShouldAllowEasyExtractionOfLibraryFromCodeUsingMsDi()
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

[SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Global")]
internal record ProjectConversion(
  JsonFileReader JsonFileReader,
  ProjectSectionFormat ProjectSectionFormat,
  HeaderFormat HeaderFormat,
  ILogger Logger);

[SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Global")]
internal record HeaderFormat(Regex Regex);
internal record ProjectSectionFormat;

[SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Global")]
internal record JsonFileReader(string FileName);

[SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Global")]
internal record AssetPreprocessor(ProjectConversion ProjectConversion);