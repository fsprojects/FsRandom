namespace FsRandom

open System
open System.Runtime.CompilerServices

[<Extension>]
module FsRandomExtensions =
   [<Extension>]
   let Select x (f:Func<_, _>) =
      Random.map f.Invoke x
   [<Extension>]
   let SelectMany x (f:Func<_, _>) (selector:Func<_, _, _>) = random {
      let! u = x
      let! v = f.Invoke u
      return selector.Invoke (u, v)
   }
