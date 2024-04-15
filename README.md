# vanilla-dependency-injection
Resources related to vanilla dependency injection. Moved from the training examples repository.

# Notable sources on DI and Vanilla DI vs containers:

* [Krzysztof Koźmic - IoC container solves a problem you might not have but it’s a nice problem to have](https://kozmic.net/2012/10/23/ioc-container-solves-a-problem-you-might-not-have-but-its-a-nice-problem-to-have/)

* [Mark Seemann - When to use a DI container?](https://blog.ploeh.dk/2012/11/06/WhentouseaDIContainer/)
* [Mark Seemann - Pure Dependency Injection](https://blog.ploeh.dk/2014/06/10/pure-di/)
* [Vanilla Dependency Injection Manifesto](https://github.com/vanilla-manifesto/vanilla-di-manifesto)
* [Nikola Malovic - Inversion Of Control, Single Responsibility Principle and Nikola’s laws of dependency injection](https://vuscode.wordpress.com/2009/10/16/inversion-of-control-single-responsibility-principle-and-nikola-s-laws-of-dependency-injection/)
* [Yacoub Massad - Why DI containers fail with “complex” object graphs]( https://criticalsoftwareblog.com/2015/08/23/why-di-containers-fail-with-complex-object-graphs/)
* [Nat Pryce - Dependency "Injection" Considered Harmful](http://www.natpryce.com/articles/000783.html)
* [Dan North - Why Every Element of SOLID is Wrong](https://speakerdeck.com/tastapod/why-every-element-of-solid-is-wrong) - source of the phrase "new is the new new" and "Assemble into small components that fit in your head".
* [Vaughn Vernon - 
Your Brain on Inversion of Control and Dependency Injection](https://kalele.io/your-brain-on-inversion-of-control-and-dependency-injection/) - the section `Dependency Injection without an IoCC`

* [Mark Seemann - Type-safe DI Composition](https://blog.ploeh.dk/2022/01/10/type-safe-di-composition/)

# Supporting sources

* [Mark Seemann - Dependency Whac-a-mole](https://blog.ploeh.dk/2023/10/02/dependency-whac-a-mole/)
* [Jimmy Bogard - You Probably Don't Need to Worry About MediatR](https://www.jimmybogard.com/you-probably-dont-need-to-worry-about-mediatr/) - the section `[Violates] the Explicit Dependencies Principle`


# Topics

1. Vanilla DI vs a DI container
    1. Autowiring (`_1_Autowiring.cs` and handcoded)
        1. When is not helping
        1. When is helping - reusing recipe
        1. When is not helping again - multiple recipes for the same type
        1. Minimizing recipe reuse problem in Vanilla DI with local functions - `_1_Autowiring.cs`
    1. Managing scopes (`_2_LifetimeScopeMagament.cs`)
        1. Container example
        1. Vanilla DI and manual Dispose
        1. What if Dispose throws an exception?
        1. Disposal subsystem
    1. Handling missing dependencies (`MissingDependency.cs`)
        1. Container - runtime
        1. Vanilla DI - compile time
    1. Circular dependencies (`CircularDependencies.cs`)
        1. Uncomment assertions
        1. CD without container validations
        1. CD with container validations
        1. CD with container and lambdas (!!)
        1. Conclusion: avoid lambdas in MsDi
        1. CD with Vanilla DI
    1. Dead code detection (`DeadCode.cs_`)
    1. Extracting code into libraries (`ExtractingLibrary.cs`)
    1. Simple vs complex dependency graphs
    1. Multiple lifestyles for different instances of a type (`MultipleLifestylesOfInstancesTheSameClass.cs`)
    1. Decorators/composites (`DecoratorsWithMultipleChains.cs`)
        1. Optional - different syntaxes for containers (autofac, scrutor)
    1. Similar subgraphs differing with leaves.
        1. `MultipleObjectOfSameTypeConfiguredDifferentlyAndNamingPropagation2.cs` - with polymorphism
            1. Vanilla DI
            1. MsDi without modules
            1. MsDi with modules
            1. Vanilla DI with refactoring
    1. [Optional - can also be discussed in aspnet core example] Graph readability
    1. [Optional - needs more code] Multiple constructors & factory methods
    1. [Optional - needs more code] Passing literals into constructors
1. How to integrate Vanilla DI into ASP.NET Core? (`BasicIntegration`)
    1. Composition root object
    1. Minimal API
    1. Controllers
    1. SignalR
1. How to do Vanilla DI well and scale it?
    1. Refactoring object composition - simple techniques
    1. Decomposing object composition into components
    1. Testing subcomponents
