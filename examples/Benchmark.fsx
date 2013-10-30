#r @"..\Build\FsRandom.dll"

open System
open FsRandom

let time f x =
   let stopwatch = System.Diagnostics.Stopwatch ()
   stopwatch.Start ()
   f x |> ignore
   stopwatch.Stop ()
   stopwatch.Elapsed
let trimmedMean p (s:seq<TimeSpan>) =
   let mutable n = 0
   let mutable all = []
   use e = s.GetEnumerator ()
   while e.MoveNext () do
      n <- n + 1
      all <- e.Current :: all
   let init xs =
      let rec loop = function
         | _, [] -> []
         | head, y :: ys -> head :: loop (y, ys)
      match xs with
         | [] -> failwith "empty list"
         | y :: ys -> loop (y, ys)
   let rec trim = function
      | 0, acc -> acc
      | _, [] -> failwith "empty list"
      | c, xs -> trim (c - 1, (List.tail >> init) xs)
   let total = trim (int (float n * p), all) |> List.fold (+) TimeSpan.Zero
   TimeSpan.FromTicks (int64 (float total.Ticks / float n))

let r = Random ()
let seed = createState xorshift (123456789u, 362436069u, 521288629u, 88675123u)

let fs = Seq.ofRandom ``[0, 1)`` seed
let ds1 =
   let rec loop () = seq {
      yield r.NextDouble ()
      yield! loop ()
   }
   loop ()
let ds2 = seq { while true do yield r.NextDouble () }
[<Literal>]
let ``1 / 2^52`` = 2.22044604925031308084726333618e-16
let ds3 =
   let rec loop () = seq {
      let u1 = r.Next ()
      let u2 = r.Next ()
      let r = (uint64 u1 <<< 26) ||| (uint64 (u2 &&& 0b00000011111111111111111111111111))
      yield (float r + 0.5) * ``1 / 2^52``
      yield! loop ()
   }
   loop ()

let n = 5000000
let round = 10
let trim = 0.2
let benchmark s =
   Seq.init round (fun _ -> s)
   |> Seq.map (time (Seq.take n >> Seq.length))
   |> trimmedMean trim
printfn "Iterates %d random numbers %d times" n round
printfn "%d%% trimmed mean" (int <| 100.0 * trim)
benchmark fs |> printfn "Seq.ofRandom*\t%A"
benchmark ds1 |> printfn "Recursion^\t%A"
benchmark ds2 |> printfn "Imperative^\t%A"
benchmark ds3 |> printfn "Recursion*\t%A"
printfn "*, 53-bit resolution; ^, 31-bit resolution"
