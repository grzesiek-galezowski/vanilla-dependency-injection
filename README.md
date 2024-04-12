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


Topics:
1. Where Vanilla DI is better/worse than a container?
  1. Autowiring
  2. Captive dependencies
  3. Circular dependencies
  4. Dead code detection
  5. Decorators/composites
  6. Extracting code into libraries
  7. Passing literals into constructors
  8. Handling missing dependencies
  9. Multiple constructors
  10. Multiple lifestyles for different instances of a type
  11. Similar subgraphs differing with leaves.
  12. 
1. How to do Vanilla DI well and scale it?
1. How to integrate Vanilla DI into ASP.NET Core?
