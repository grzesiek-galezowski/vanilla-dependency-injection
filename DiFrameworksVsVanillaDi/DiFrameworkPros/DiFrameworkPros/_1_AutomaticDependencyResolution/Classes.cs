namespace DiFrameworkPros._1_AutomaticDependencyResolution;

internal class Person(Kitchen kitchen, Logger logger);
internal class Kitchen(Knife knife, Logger logger);
internal class Knife(Logger logger);
internal class Logger(LoggingChannel loggingChannel);
internal class LoggingChannel;
