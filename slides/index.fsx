(**
- title : 
- description : Pre-show - F# reintroduction
- author : Jason Kleban
- theme : League
- transition : default

***

### Why functional programming?

Jason Kleban

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
        Console.Writeline("{0}: {1}", i, elements[i].ToString());
        i++;
        if (i < elements.length) goto loop;

---

    [lang=cs]
    for (var i = 0; i < elements.length; i++)
    {
        // something with elements[i]
        Console.Writeline("{0}: {1}", i, elements[i].ToString());
    }

---

    [lang=cs]
    foreach (var element in elements)
    {
        // something with element
        Console.Writeline("{0}", element.ToString());
    }

' elements : IEnumerable<T> which can be recognized as the Iterator design **pattern**
' ... but we do happen to miss out on the native `i`

---
    
    [lang=cs]
    var i = 0;
    foreach (var element in elements)
    {
        // something with element
        Console.Writeline("{0}: {1}", i, element.ToString());
        i++;
    }

' if we wanted the `i`, the native mechanism does not offer it.  Just sayin'!

---

' anyway, forget the `i`.  Let's nest the loops and have some condition

    [lang=cs]
    foreach (var element in elements)
    {
        if (threshold <= element.level)
        {
            foreach (var el in element)
            {
                Console.Writeline("{0}, {1}", element, el);
            }
        }
    }

---
    
    [lang=cs]
    var filteredElements = new List<Element>();
    
    foreach (var element in elements)
    {
        if (threshold <= element.level)
        {
            filteredElements.Add(element);
        }
    }
    
    foreach (var element in filteredElements)
    {
        foreach (var el in element)
        {
            Console.Writeline("{0}, {1}", element, el);
        }
    }

' We can't readily get rid of the inner loop using the foreach mechanism by itself, but we can separate out the filtering logic

---

    [lang=cs]
    var filteredElements = new List<Element>();
    IFilter filter = new ThresholdFilter();
    
    foreach (var element in elements)
    {
        if (filter.Test(element))
        {
            filteredElements.Add(element);
        }
    }
    
    foreach (var element in filteredElements)
    {
        foreach (var el in element)
        {
            Console.Writeline("{0}, {1}", element, el);
        }
    }

' Using IFilter isn't much of an improvement but at least we could pass in the condition to this encapsulating method

---

    [lang=cs]
    var filteredElements = new List<Element>();
    Func<Element, bool> test = element => threshold <= element.level;
    
    foreach (var element in elements)
    {
        if (test(element))
        {
            filteredElements.Add(element);
        }
    }
    
    foreach (var element in filteredElements)
    {
        foreach (var el in element)
        {
            Console.Writeline("{0}, {1}", element, el);
        }
    }

' I start to bring in the newer C# concepts of Func<> instead of the IFilter interface - already a functional concept

---

    [lang=cs]
    void Run(Func<Element, bool> filter, Action<Subelement> operation)
    {
        var filteredElements = new List<Element>();
    
        foreach (var element in elements)
        {
            if (filter(element))
            {
                filteredElements.Add(element);
            }
        }
    
        foreach (var element in filteredElements)
        {
            foreach (var el in element)
            {
                operation(el);
            }
        }
    }

' Adds the wrapping method accepting filter and operation as parameters
' Without functional concepts of Func<> and Action<>, we'd have to create a lot more interfaces and classes to achieve this

---

    [lang=cs]
    void Run(Func<Element, bool> filter, Action<Subelement> operation)
    {
        var filteredElements = elements.Where(filter);

        var filteredSubelements = filteredElements.SelectMany(element => element);
    
        foreach (var el in filteredSubelements)
        {
            operation(el);
        }
    }

' Using LINQ, we can make this shorter, but it highlights remaining awkwardness.
' For a variety of best-practice reasons, we don't do Actions in LINQ expressions

---

    [lang=cs]
    IEnumerable<Subelement> Process(Func<Element, bool> filter)
    {
        return elements.Where(filter).SelectMany(element => element);
    }
    

' Here we shrink further, remove the operation all together and allow the caller to provide the operation after selection, since it no longer has to be
' embedded in the loop anyway.

***

### Why Functional Programming Matters

John Hughes, 1990

---

*)

type 'T LList = 
    | Node of head : 'T * tail : 'T LList
    | End

(**
<div style="display: none">
*)

let rec Of = function
    | head :: tail -> Node(head, Of tail)
    | [] -> End
let _Node head tail = Node(head, tail)

(**
</div>
*)

(*** define-output: llist1 ***)
Node (1, Node (2, Node (3, End)))
    |> printfn "%A"
    
(*** define-output: llist2 ***)
Of [1;2;3] 
    |> printfn "%A"

(*** include-output: llist1 ***)
(*** include-output: llist2 ***)
(**---*)

(**
<div style="display: none">
*)
module sum_attempt_1 =
(**
</div>
*)
    let rec sum = function
        | End -> 0
        | Node (head, tail) -> (+) head (sum tail)
        
    Of [1;2;3] 
        |> sum
        |> printfn "%A"

(**---*)

(*** define-output: sum ***)
let rec foldr op unit = function
    | End -> unit
    | Node (head, tail) -> op head (foldr op unit tail)

let sum = foldr (+) 0

Of [1;2;3] 
    |> sum 
    |> printfn "%A"
    
(*** include-output: sum ***)
(**---*)

(*** define-output: product ***)
//let rec foldr op unit = function
//    | End -> unit
//    | Node (head, tail) -> op head (foldr op unit tail)

let product = foldr (*) 1

Of [10;20;30] 
    |> product 
    |> printfn "%A"
    
(*** include-output: product ***)
(**---*)

(*** define-output: anyTrue ***)
//let rec foldr op unit = function
//    | End -> unit
//    | Node (head, tail) -> op head (foldr op unit tail)

let anyTrue = foldr (||) false

Of [false;true;false] 
    |> anyTrue 
    |> printfn "%A"
    
(*** include-output: anyTrue ***)
(**---*)

(*** define-output: allTrue ***)
//let rec foldr op unit = function
//    | End -> unit
//    | Node (head, tail) -> op head (foldr op unit tail)

let allTrue = foldr (&&) true

Of [false;true;false] 
    |> allTrue 
    |> printfn "%A"
    
(*** include-output: allTrue ***)
(**---*)

(*** define-output: copy ***)
//let rec foldr op unit = function
//    | End -> unit
//    | Node (head, tail) -> op head (foldr op unit tail)

let copy l = foldr (_Node) End l

Of [1;2;3] 
    |> copy 
    |> printfn "%A"
    
(*** include-output: copy ***)
(**---*)

(*** define-output: append ***)
//let rec foldr op unit = function
//    | End -> unit
//    | Node (head, tail) -> op head (foldr op unit tail)

let append a b = foldr (_Node) b a

append (Of [1;2;3]) (Of [4;5;6])
    |> printfn "%A"
    
(*** include-output: append ***)
(**---*)

(*** define-output: length ***)
//let rec foldr op unit = function
//    | End -> unit
//    | Node (head, tail) -> op head (foldr op unit tail)

let count a n = n + 1
let length l = foldr count 0 l

length (Of [4;5;6])
    |> printfn "%A"
    
(*** include-output: length ***)
(**---*)

(**
<div style="display: none">
*)
module doubleall_attempt_1 =
(**
</div>
*)
    let doubleandcons n list = _Node ((*) 2) list
    let doubleall l = foldr doubleandcons End l

    doubleall (Of [1;2;3])
        |> printfn "%A"

(**---*)

(**
<div style="display: none">
*)
module doubleall_attempt_2 =
(**
</div>
*)
    let double = (*) 2
    let fandcons f el list = _Node (f el) list
    let doubleandcons = fandcons double
    let doubleall l = foldr doubleandcons End l

    doubleall (Of [1;2;3])
        |> printfn "%A"

(**---*)

(**
<div style="display: none">
*)
module doubleall_attempt_3 =
(**
</div>
*)
    let double = (*) 2
    let fandcons f = _Node << f
    let doubleandcons = fandcons double
    let doubleall l = foldr doubleandcons End l

    doubleall (Of [1;2;3])
        |> printfn "%A"

(**---*)
    
(*** define-output: doubleall ***)
let double = (*) 2
let map f = foldr (_Node << f) End
let doubleall l = map double l

doubleall (Of [1;2;3])
    |> printfn "%A"
    
(*** include-output: doubleall ***)
(**---*)

(*** define-output: sumMatrix ***)
let sumMatrix = sum << map sum

Of  [(Of [1;2;3]);
     (Of [4;5;6]);
     (Of [7;8;9])]
    |> sumMatrix
    |> printfn "%A"
    
(*** include-output: sumMatrix ***)
(**---*)

(**
<div style="display: none">
*)
module list_final =
(**
</div>
*)
 let count a n = n + 1
 let double = (*) 2
 
 let rec foldr op unit l = 
     match l with 
     | End -> unit
     | Node (head, tail) -> op head (foldr op unit tail)
 
 let sum =           foldr (+)           0
 let product =       foldr (*)           1
 let anyTrue =       foldr (||)          false
 let allTrue =       foldr (&&)          true
 let copy l =        foldr (_Node)       End     l
 let append a b =    foldr (_Node)       b       a
 let length l =      foldr count         0       l
 let map f =         foldr (_Node << f)  End
 
 let doubleall l = map double l
    
(**
***
*)

(**
<div style="display: none">
*)
module trees_v1 =
(**
</div>
*)
    type 'T Tree = 
        | TNode of value : 'T * leaves : 'T Tree LList

    let _Tree (value : 'T) (leaves : 'T Tree LList) = TNode(value, leaves)

    _Tree 1 (_Node 
        (_Tree 2 End) 
        (_Node 
            (_Tree 3 (_Node 
                (_Tree 4 End) 
            End)) 
        End))
        |> printfn "%A"

(**---*)

type 'T Tree =
    | Tree of label : 'T * leaves : 'T Tree list

Tree (1,[ 
        Tree (2,[]);
        Tree (3,[
                Tree (4,[])])])
    |> printfn "%A"
    
(**---*)

let rec foldtree f g a (Tree(label, subtrees)) = 
    f label (foldleaves f g a subtrees)
and foldleaves f g a (l : 'T Tree list) = 
    match l with  
    | (subtree :: rest) -> 
        g (foldtree f g a subtree) (foldleaves f g a rest)
    | [] -> a
    
(**---*)
(*** define-output: sumtree ***)
let sumtree = foldtree (+) (+) 0

Tree (1,[ 
        Tree (2,[]);
        Tree (3,[
                Tree (4,[])])])
    |> sumtree
    |> printfn "%A"
    
(*** include-output: sumtree ***)
(**---*)

(*** define-output: labels ***)
let labels t = 
    foldtree 
        (fun c a -> c :: a) 
        (List.append) 
        [] 
        t
        
Tree (1,[ 
        Tree (2,[]);
        Tree (3,[
                Tree (4,[])])])
    |> labels
    |> printfn "%A"
    
(*** include-output: labels ***)
(**---*)

(*** define-output: labels ***)
let maptree f = 
    foldtree 
        ((fun v l -> Tree(v,l)) << f) 
        (fun c a -> c :: a) 
        []
        
Tree (1,[ 
        Tree (2,[]);
        Tree (3,[
                Tree (4,[])])])
    |> maptree double
    |> printfn "%A"

(*** include-output: labels ***)

(**---*)
(**
<div style="display: none">
*)
module tree_final =
(**
</div>
*)
 let rec foldtree f g a (Tree(label, subtrees)) = 
     f label (foldleaves f g a subtrees)
 and foldleaves f g a (l : 'T Tree list) = 
     match l with  
     | (subtree :: rest) -> 
         g (foldtree f g a subtree) (foldleaves f g a rest)
     | [] -> a
 
 let sumtree =   foldtree (+)                            (+)                 0
 let labels t =  foldtree (fun c a -> c :: a)            (List.append)       []      t
 let maptree f = foldtree ((fun v l -> Tree(v,l)) << f)  (fun c a -> c :: a) []
      
(**

***

    [lang=csharp]
    public abstract class LList<T>
    {
        internal static readonly LList<T> _unique_End = new LList<T>._End();

        public class Node : LList<T>
        {
            internal readonly T _head;
            internal readonly LList<T> _tail;

            public T head { get { return this._head; } }
            public LList<T> tail { get { return this._tail; } }

            internal Node(T _head, LList<T> _tail)
            {
                this._head = _head;
                this._tail = _tail;
            }
        }

        internal class _End : LList<T>
        { internal _End() { } }

        public static LList<T> NewNode(T _head, LList<T> _tail)
        { return new LList<T>.Node(_head, _tail); }
        public static LList<T> End { get { return LList<T>._unique_End; } }

        public bool IsNode { get { return this is LList<T>.Node; } }
        public bool IsEnd { get { return this is LList<T>._End; } }
    }  
*)

// type 'T LList = 
//   | Node of head : 'T * tail : 'T LList
//   | End

(**
    
---

    [lang=csharp]
    static U Foldr<T, U>(Func<T, U, U> op, U unit, LList<T> list)
    {
        var node = list as LList<T>.Node;

        if (node != null)
        {
            return op(
                node._head, 
                Foldr<T, U>(op, unit, node._tail));
        }
        return unit;
    }

    //       let rec foldr op unit = function
    //           | End -> unit
    //           | Node (head, tail) -> op head (foldr op unit tail)
    
---

    [lang=csharp]
    static int Sum(LList<int> list)
    {
        return Foldr(
            (a, b) => a + b, 
            0, 
            list);
    }

    //     let sum = foldr (+) 0
    
---

    [lang=csharp]
    static int Product(LList<int> list)
    {
        return Foldr(
            (a, b) => a * b,
            1,
            list);
    }


    //     let product = foldr (*) 1

---

    [lang=csharp]
    var l = LList<int>.NewNode(3,
        LList<int>.NewNode(4,
        LList<int>.NewNode(5, LList<int>.End)));

    Console.WriteLine(Sum(l));

    Console.WriteLine(Product(l));
*)

let l = Node (3, Node (4, Node (5, End)))

printfn "%A" (sum l)
printfn "%A" (product l)

(**

***

### F# Syntax Recap

  * Indentation matters - the rules are complicated but intuitive eventually
  * Parentheses are for precedence & tuples but not specifically for function arguments

  The syntax may look weird, but it isn't about saving keystrokes or space!
  Functional programming is beautifully consistent.
  F# and other implementations aren't perfectly consistent but they try.

***

### Neat projects

* Type Providers
* Akka.net
* WebSharper
* MBrace
* Freya
* Coeffects research by Tomas Petricek
* FCell
* LinqPad 5

*)