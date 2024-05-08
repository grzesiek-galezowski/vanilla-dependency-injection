using Castle.Core.Logging;
using System.Text.RegularExpressions;
using Pure.DI;

namespace DiFrameworkCons.CompileTimeVsRuntime.ExtractingLibrary;

/// <summary>
/// With Pure.DI, we can copy-paste fragments of composition
/// from the generated code. It's not super pretty, but it only takes
/// a short time to clean it up (at least in this example).
///
/// The composition root still needs to be changed manually though,
/// differently than with Vanilla DI where extraction refactorings
/// automatically adjust the composition root.
/// </summary>
public class ExtractingLibrary_PureDiLibrary
{
  [Test]
  public static void ContainerContainsSomeDeadCodeWithMsDi()
  {
    //GIVEN
    var composition = new Composition5();

    //WHEN
    var resolvedInstance = composition.Preprocessor;

    //THEN
    resolvedInstance.Should().NotBeNull();
  }
}

partial class Composition5
{
  public void Setup()
  {
    DI.Setup(nameof(Composition5))
      .Bind().As(Lifetime.Transient).To<AssetPreprocessor>()
      .Bind().As(Lifetime.Transient).To<ProjectConversion>()
      .Bind().As(Lifetime.Transient).To<ProjectSectionFormat>()
      .Bind().As(Lifetime.Transient).To(_ => new JsonFileReader("file.json"))
      .Bind().As(Lifetime.Transient).To(_ => new HeaderFormat(new Regex("^.+$")))
      .Bind<ILogger>().As(Lifetime.Transient).To<ConsoleLogger>()
      .Root<AssetPreprocessor>("Preprocessor");
  }
}