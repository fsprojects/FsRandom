module FsRandom.StateMonad

type State<'s, 'a> = 's -> 'a * 's

val inline ( |>> ) : m:State<'s, 'a> -> f:('a -> State<'s, 'b>) -> State<'s, 'b>
val inline ( &>> ) : m:State<'s, 'a> -> b:State<'s, 'b> -> State<'s, 'b>
val inline bindState : m:State<'s, 'a> -> f:('a -> State<'s, 'b>) -> State<'s, 'b>
val inline returnState : a:'a -> State<'s, 'a>
val inline getState : State<'s, 's>
val inline setState : state:'s -> State<'s, unit>
val inline runState : State<'s, 'a> -> 's -> 'a * 's
val inline evaluateState : State<'s, 'a> -> 's -> 'a
val inline executeState : State<'s, 'a> -> 's -> 's

type StateBuilder =
   new : unit -> StateBuilder
   member Bind : m:State<'s, 'a> * f:('a -> State<'s, 'b>) -> State<'s, 'b>
   member Combine : a:State<'s, 'a> * b:State<'s, 'b> -> State<'s, 'b>
   member Return : a:'a -> State<'s, 'a>
   member ReturnFrom : m:State<'s, 'a> -> State<'s, 'a>
   member Zero : unit -> State<'s, unit>
   member Delay : (unit -> State<'s, 'a>) -> State<'s, 'a>
   member While : condition:(unit -> bool) * m:State<'s, unit> -> State<'s, unit>
   member For : source:seq<'a> * f:('a -> State<'s, unit>) -> State<'s, unit>
val state : StateBuilder
