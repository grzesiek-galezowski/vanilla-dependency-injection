namespace DiFrameworkPros._1_AutomaticDependencyResolution;

public class Person(Kitchen kitchen, Logger logger);
public class Kitchen(Knife knife, Logger logger);
public class Knife(Logger logger);
public class Logger(LoggingChannel loggingChannel);
public class LoggingChannel;
