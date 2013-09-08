module FsRandom.StateMonad

type State<'s, 'a> = 's -> 'a * 's

let inline bindState (m : State<'s, 'a>) (f : 'a -> State<'s, 'b>) = fun s0 -> let v, s' = m s0 in f v s'
let inline returnState x = fun s -> x, s
let inline getState s = s, s
let inline setState (x : 's) = fun (_ : 's) -> (), x
let inline runState (m:State<'s, 'a>) x = m x
let inline evaluateState (m:State<'s, 'a>) x = m x |> fst
let inline executeState (m:State<'s, 'a>) x = m x |> snd

let inline (|>>) m f = bindState m f
let inline (&>>) m b = bindState m (fun _ -> b)

type StateBuilder () =
   member this.Bind (m, f) = m |>> f
   member this.Combine (a, b) = a &>> b
   member this.Return (x) = returnState x
   member this.ReturnFrom (m : State<'s, 'a>) = m
   member this.Zero () = fun x -> (), x
   member this.Delay (f) = returnState () |>> f
   member this.While (condition, m) =
      if condition () then
         m |>> (fun () -> this.While (condition, m))
      else
         this.Zero ()
   member this.For (source : seq<'a>, f) =
      use e = source.GetEnumerator ()
      this.While (e.MoveNext, this.Delay (fun () -> f e.Current))
let state = StateBuilder ()
