namespace DiFrameworkCons.MultipleRecipes.MultipleLifestylesOfInstancesTheSameClass;

internal record EmergencyProcess(ThrottledOutbox ThrottledOutbox);
internal record ScheduledProcess(ThrottledOutbox ThrottledOutbox);
internal record OnDemandProcess(ThrottledOutbox ThrottledOutbox);
internal class ThrottledOutbox;