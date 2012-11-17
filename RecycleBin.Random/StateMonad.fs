module RecycleBin.Random.StateMonad

type State<'s, 'a> = 's -> 'a * 's

let bindState (m : State<'s, 'a>) (f : 'a -> State<'s, 'b>) = fun s0 -> let v, s' = m s0 in f v s'
let returnState x = fun s -> x, s
let getState = fun s -> s, s
let setState (x : 's) = fun (_ : 's) -> (), x

let inline (|>>) m f = bindState m f
let inline (&>>) m b = bindState m (fun _ -> b)

type StateBuilder () =
   member this.Bind (m, f) = bindState m f
   member this.Return (x) = returnState x
   member this.ReturnFrom (m : State<'s, 'a>) = m
