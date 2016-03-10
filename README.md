# Monads
C# library of basic monads and accompanying demos from my presentation to [CoderCamp 
Hamilton][1] [S16E03 on March 9, 2016][2]. It is licensed under the MIT License and 
can be used under those terms.

- **Maybe&lt;T>** is implemented as a struct similar to **Nullable&lt;T>**, but allowing any
 subclass of **object** as its basetype whether **struct** or **class**.

- **MaybeX&lt;T>** is also implemented as a struct, with an implicit cast to **Maybe&lt;T>**, 
that is optimized for **class** basetypes, by not storing the *HasValue* property explicitly.

- **Reader&lt;E,T>** and **State&lt;S,T>** are delegate implementations of these well-known monads.

- **IO&lt;T>** is also a elegate implementation, with many common Console I/O functions predefined.

All the monads above:

- come with the methods *Select()*, *SelectMany()* and 
*SelectMany(,)* predefined to enable the **LINQ** Comprehension (or Query) syntax.

- Have been fully annotated with CodeContracts to statically disprove the 
largest possible number of null references. 

The slide presentation is available [here as Mathless Monads in C#][3].

Enjoy!

Pieter Geerkens

[1]: http://www.codercamphamilton.com/
[2]: http://www.codercamphamilton.com/Events/2016/03/09/CoderCamp-S16E03
[3]: https://github.com/pgeerkens/Presentations
