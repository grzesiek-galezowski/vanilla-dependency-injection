using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Castle.Core.Logging;

namespace DiFrameworkCons.CompileTimeVsRuntime.ExtractingLibrary;

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