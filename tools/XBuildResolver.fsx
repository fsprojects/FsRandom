// Modification of https://gist.github.com/3564010

#r "System.Xml.Linq.dll"

open System
open System.IO
open System.Xml.Linq
open Microsoft.FSharp.Core.LanguagePrimitives

type Trial<'a, 'b> =
   | Success of 'a
   | Failure of 'b

type Error =
   | None = 0
   | IO = 1
   | InvalidArgument = 2
   | InvalidInput = 3

let exit = EnumToValue >> Environment.Exit
   
let doEdit edit =
   function
      | Success input ->
         try
            Success (edit input)
         with
            | ex -> Failure (ex, Error.InvalidInput)
      | Failure (ex, error) ->
         Failure (ex, error)

let getUserInput () =
   match fsi.CommandLineArgs with
      | [| _ |] ->
         try
            Success <| Console.In.ReadToEnd ()
         with
            | ex -> Failure (ex, Error.IO)
      | [| _; path |] ->
         try
            Success <| File.ReadAllText (path)
         with
            | ex -> Failure (ex, Error.IO)
      | args ->
         let filename = Path.GetFileName (args.[0])
         let message = sprintf "Usage: Fsi.exe %s [project.fsproj]" filename
         let ex = ApplicationException (message) :> exn
         Failure (ex, Error.InvalidArgument)

let editProject rawProjectXml =
   let project = XDocument.Parse rawProjectXml
   query {
      for import in project.Descendants (XName.Get ("Import", "http://schemas.microsoft.com/developer/msbuild/2003")) do
      let project = import.Attribute (XName.Get ("Project"))
      where (project.Value.EndsWith ("Microsoft.FSharp.Targets"))
      select (import.Attribute (XName.Get ("Condition")))
   }
   |> Seq.iter (fun attribute -> attribute.Remove ())
   project

getUserInput ()
|> doEdit editProject
|> function
   | Success result ->
      Console.WriteLine (result)
      Error.None
   | Failure (ex, error) ->
      Console.Error.WriteLine (ex.Message)
      error
|> exit
