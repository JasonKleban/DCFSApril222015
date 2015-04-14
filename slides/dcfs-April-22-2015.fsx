(**
- title : DC F# - April 22, 2015
- description : Pre-show - F# reintroduction
- author : Riccardo Terrell
- theme : Night
- transition : default

***

### Why is functional programming relevant?

---

> Sometimes, the elegant implementation is just a function.  Not a method.  Not a class.  Not a framework.  Just a function.

\- @ID\_AA\_Carmack (John Carmack)

' - John Carmack of ID Software (Commander Keen, Wolfenstein 3D, Doom, Quake, ...) and now CTO of Oculus VR
' - We often implement an interface when we really just needed a function signature (IHandler.Handle())

---

> When the general atmosphere is bad, language must suffer ... but if thought corrupts language, language can also corrupt thought.

\- George Orwell

↬ Steve Goguen

' Steve does a neat talk, which is available online, strongly arguing that 
' 1) the languages you practice (spoken or coded) can constrain your ability to think!
' 2) many of the "Gang of Four"'s "Design Patterns" are overly elaborate in a language like C# but are native concepts in functional languages.

----

> In the OO world you hear a good deal about "patterns" ... When I see patterns in my programs, I consider it a sign of trouble. Any other regularity in the code is a sign ... that I'm generating by hand the expansions of some macro that I need to write.

\- Paul Graham

' - Paul Graham, co-founder of Y Combinator seed capital firm
' - Another Paul Graham quote about a hypothetical moderate-power Blub programming language
'
' > As long as our hypothetical Blub programmer is looking down the power continuum, he knows he's looking down. Languages less powerful than Blub 
' are obviously less powerful, because they're missing some feature he's used to. But when our hypothetical Blub programmer looks in the other 
' direction, up the power continuum, he doesn't realize he's looking up. What he sees are merely weird languages. He probably considers them about 
' equivalent in power to Blub, but with all this other hairy stuff thrown in as well. Blub is good enough for him, because he thinks in Blub.

---

### Why is functional programming relevant?

Functional programming encourages thinking about:

- purity
- immutability
- composition
- side-effects
- code-as-data

In turn, these concepts will lead you to:

- parallelizable,
- distributable,
- simpler,
- testible,
- maintainable
- *correct* code

***

    [lang=cs]
        var i = 0;
    loop:
        // something with elements[i]
        i++;
        if (i < elements.length) goto loop;

        for (var i = 0; i < elements.length; i++)
        {
            // something with elements[i]
        }

        foreach (var element in elements) // elements : IEnumerable<T> which implements the "Iterator" pattern
        {
            // something with element
        }

*)