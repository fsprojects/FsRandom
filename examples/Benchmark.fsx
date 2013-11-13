#if INTERACTIVE
#I "../Build"
#r "FsRandom.dll"
#endif

open System
open FsRandom

type BenchmarkOption = {
   Iterate : int
   Size : int
   Round : int
   Trim : float
}
let option =
   let rec loop acc = function
      | [] -> acc
      | "--iterate" :: n :: rest -> loop { acc with Iterate = int n } rest
      | "--size" :: n :: rest -> loop { acc with Size = int n } rest
      | "--round" :: r :: rest -> loop { acc with Round = int r } rest
      | "--trim" :: t :: rest -> loop { acc with Trim = float t } rest
      | s -> failwith "unknown option: %s" s
#if INTERACTIVE
   // recursion iteration with fsharpi (Mac/Linux) is very slow.
   // Too large Iterate does not return so long.
   let defaultOption = { Iterate = 100000; Size = 100000; Round = 20; Trim = 0.2 }
   let args = fsi.CommandLineArgs
#else
   let defaultOption = { Iterate = 5000000; Size = 100000; Round = 20; Trim = 0.2 }
   let args = Environment.GetCommandLineArgs ()
#endif
   let args = (List.ofArray args).Tail
   loop defaultOption args

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
   let total = trim (int (float n * p), List.sort all) |> List.fold (+) TimeSpan.Zero
   TimeSpan.FromTicks (int64 (float total.Ticks / float n))

let r = Random ()
let xorshiftState = createState xorshift (123456789u, 362436069u, 521288629u, 88675123u)
let systemrandomState = createState systemrandom r

[<Literal>]
let ``1 / 2^52`` = 2.22044604925031308084726333618e-16
let inline generate53bit () =
   let u1 = r.Next ()
   let u2 = r.Next ()
   let r = (uint64 u1 <<< 26) ||| (uint64 (u2 &&& 0b00000011111111111111111111111111))
   (float r + 0.5) * ``1 / 2^52``

let fsx = Seq.ofRandom ``[0, 1)`` xorshiftState
let fss = Seq.ofRandom ``[0, 1)`` systemrandomState
let ds1 =
   let rec loop () = seq {
      yield r.NextDouble ()
      yield! loop ()
   }
   loop ()
let ds2 = seq { while true do yield r.NextDouble () }
let ds3 =
   let rec loop () = seq {
      yield generate53bit ()
      yield! loop ()
   }
   loop ()
let ds4 = seq { while true do yield generate53bit () }
   
let fsxArrayCreate n = Random.get (Array.randomCreate n ``[0, 1)``) xorshiftState
let fssArrayCreate n = Random.get (Array.randomCreate n ``[0, 1)``) systemrandomState
let fsxArrayInit n = Random.get (Array.randomInit n (fun _ -> ``[0, 1)``)) xorshiftState
let fssArrayInit n = Random.get (Array.randomInit n (fun _ -> ``[0, 1)``)) systemrandomState
let dsArrayInit n = Array.init n (fun _ -> r.NextDouble ())
let dsArrayInit53 n = Array.init n (fun _ -> generate53bit ())

let iterate = option.Iterate
let size = option.Size
let round = option.Round
let trim = option.Trim
let benchmarkSeq name s =
   GC.Collect ()
   printf "%s" name
   Seq.init round (fun _ -> s)
   |> Seq.map (time (Seq.take iterate >> Seq.length))
   |> Seq.map (fun s -> printf "."; s)
   |> trimmedMean trim
   |> printfn "\t%A"
let benchmarkArray name s =
   printf "%s" name
   Seq.init round (fun _ -> s)
   |> Seq.map (fun s -> time s size)
   |> Seq.map (fun s -> printf "."; s)
   |> trimmedMean trim
   |> printfn "\t%A"

printfn "---"
printfn "Iterates %d random numbers %d times" iterate round
printfn "---"
benchmarkSeq "*Seq.ofRandom (xorshift)" fsx
benchmarkSeq "*Seq.ofRandom (systemrandom)" fss
benchmarkSeq "^Recursion" ds1
benchmarkSeq "^Imperative" ds2
benchmarkSeq "*Recursion" ds3
benchmarkSeq "*Imperative" ds4
printfn "---"
printfn "Creates %d random arrays of size %d" round size
printfn "---"
benchmarkArray "*Array.randomCreate (xorshift)" fsxArrayCreate
benchmarkArray "*Array.randomCreate (systemrandom)" fssArrayCreate
benchmarkArray "*Array.randomInit (xorshift)" fsxArrayInit
benchmarkArray "*Array.randomInit (systemrandom)" fssArrayInit
benchmarkArray "^Array.init" dsArrayInit
benchmarkArray "*Array.init" dsArrayInit53
printfn "---"
printfn "%d%% trimmed mean" (int <| 100.0 * trim)
printfn "*, 53-bit resolution; ^, 31-bit resolution"
