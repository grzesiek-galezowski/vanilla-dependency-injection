namespace DiFrameworkPros._1_AutomaticDependencyResolution;

#pragma warning disable CS9113 // Parameter is unread.
public class Person(Kitchen kitchen, Logger logger);
public class Kitchen(Knife knife, Logger logger);
public class Knife(Logger logger);
public class Logger(LoggingChannel loggingChannel);
public class LoggingChannel;
#pragma warning restore CS9113 // Parameter is unread.
