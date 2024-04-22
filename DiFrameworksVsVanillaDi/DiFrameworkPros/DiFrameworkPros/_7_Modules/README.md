Some DI containers allow using modules. Modules are recipes for fragments of composition (or bundles of recipes).

They can be used to:
- parametrize fragment of object composition recipe
- logically split the composition into several parts so that each part could potentially be replaced
- If combined with visibility modifiers, can also serve as quasi-architectural component, where the module consists of registrations of mostly internal types and the registered public types serve as a boundary