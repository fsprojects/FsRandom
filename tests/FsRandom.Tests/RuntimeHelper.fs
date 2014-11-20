[<AutoOpen>]
module internal FsRandom.RuntimeHelper

open System
open System.IO
open NUnit.Framework
open RDotNet

let private engine =
   match Microsoft.Win32.Registry.LocalMachine.OpenSubKey (@"SOFTWARE\R-core") with
   | null -> System.ApplicationException("Registry key is not found.") |> raise
   | rCore ->
      let is64bit = System.Environment.Is64BitProcess
      match rCore.OpenSubKey (if is64bit then "R64" else "R") with
      | null -> System.ApplicationException("Registry key is not found.") |> raise
      | r ->
         let getString key = r.GetValue (key) :?> string
         let (%%) dir name = System.IO.Path.Combine (dir, name) 
         let currentVersion = System.Version (getString "Current Version")
         let binPath = getString "InstallPath" %% "bin"
         if currentVersion < System.Version (2, 12) then
            binPath
         else
            binPath %% if is64bit then "x64" else "i386"
   |> fun path -> System.Environment.SetEnvironmentVariable ("PATH", path)
   let engine = REngine.CreateInstance ("REngine")
   engine.Initialize ()
   engine
let getREngine () = engine

let loadPackage (package:string) =
   let makeSafePath (path:string) = path.Replace (Path.DirectorySeparatorChar, '/')
   engine.Evaluate (System.String.Format ("""if (!require({0}, lib.loc=c(.libPaths(), "{1}"))) {{
      install.packages("{0}", lib="{1}", repos="http://cran.r-project.org")
      stopifnot(require({0}, lib.loc="{1}"))
   }}""", package, makeSafePath Environment.CurrentDirectory))
   |> ignore

type S = SymbolicExpressionExtension
let getP = function
   | List (testResult) -> testResult.["p.value"] |> S.AsNumeric |> Seq.head
   | _ -> failwith "not a test result object"

let inline curry f x y = f (x, y)
let inline uncurry f (x, y) = f x y
