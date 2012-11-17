module RecycleBin.Random.StateMonad

type State<'s, 'a> = 's -> 'a * 's

val inline ( |>> ) : m:State<'s, 'a> -> f:('a -> State<'s, 'b>) -> State<'s, 'b>
val inline ( &>> ) : m:State<'s, 'a> -> b:State<'s, 'b> -> State<'s, 'b>
val bindState : m:State<'s, 'a> -> f:('a -> State<'s, 'b>) -> State<'s, 'b>
val returnState : a:'a -> State<'s, 'a>
val getState : State<'s, 's>
val setState : state:'s -> State<'s, unit>

type StateBuilder =
   new : unit -> StateBuilder
   member Bind : m:State<'s, 'a> * f:('a -> State<'s, 'b>) -> State<'s, 'b>
   member Return : a:'a -> State<'s, 'a>
   member ReturnFrom : m:State<'s, 'a> -> State<'s, 'a>
