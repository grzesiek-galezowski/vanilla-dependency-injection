using System.Text.RegularExpressions;
using Castle.Core.Logging;

namespace DiFrameworkCons.CompileTimeVsRuntime.ExtractingLibrary.Autofac;

public static class ExtractingLibrary_Autofac
{
  [Test]
  public static void ShouldAllowEasyExtractionOfLibraryFromCode()
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
}