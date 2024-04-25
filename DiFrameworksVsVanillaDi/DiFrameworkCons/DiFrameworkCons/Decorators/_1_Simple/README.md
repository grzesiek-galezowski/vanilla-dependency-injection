
Assembling decorators is a challenge in DI containers because typically a decorator depends on the same interface it implements. So if we want to avoid manual composition in this case, we need to have several types registered for the same interface and then instruct each registration which other  implementation of the same interface should be its dependency

In the example in this folder, answer is decorated by a traced answer which is decorated by synchronized answer.
