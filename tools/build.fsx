#I "FSharp.Formatting/lib/net40"
#I "RazorEngine/lib/net40/"
#I "Microsoft.AspNet.Razor/lib/net40"
#r "FakeLib.dll"
#r "ICSharpCode.SharpZipLib.dll"
#r "System.Web.Razor.dll"
#r "RazorEngine.dll"
#r "FSharp.Literate.dll"
#r "FSharp.CodeFormat.dll"
#r "FSharp.MetadataFormat.dll"
#r "FSharp.Markdown.dll"

open System
open System.IO
open System.Reflection
open Fake
open Fake.FileHelper
open FSharp.Literate
open FSharp.MetadataFormat

let baseDir = Path.GetDirectoryName (__SOURCE_DIRECTORY__)
let inline (~%) name = Path.Combine (baseDir, name)
let inline (%) dir name = Path.Combine (dir, name)

let nugetToolPath = % ".nuget" % "NuGet.exe"
let buildDir = % "Build"
let deployDir = % "Deploy"
let docsDir = buildDir % "docs"

let mainSolution = % "FsRandom" % "FsRandom.fsproj"
let projectName = "FsRandom"
let zipName = deployDir % "FsRandom.zip"

type BuildParameter = {
   Help : bool
   Documentation : bool
   DocumentationRoot : string
   Debug : bool
   Deploy : bool
   CleanDeploy : bool
   NoZip : bool
   NoNuGet : bool
   Key : string option
}
let buildParams =
   let isOption (s:string) = s.StartsWith ("-")
   let rec loop acc = function
      | [] -> acc
      | "-h" :: _ | "--help" :: _ -> { acc with Help = true }  // don't care other arguments
      | "--docs" :: args -> loop { acc with Documentation = true } args
      | "--docs-root" :: path :: args -> loop { acc with DocumentationRoot = path } args
      | "--debug" :: args -> loop { acc with Debug = true } args
      | "--deploy" :: args -> loop { acc with Deploy = true } args
      | "--clean-deploy" :: args -> loop { acc with CleanDeploy = true } args
      | "--no-zip" :: args -> loop { acc with NoZip = true } args
      | "--no-nuget" :: args -> loop { acc with NoNuGet = true } args
      | "--key" :: path :: args -> loop { acc with Key = Some (path) } args
      | arg :: _ ->
         eprintfn "Unknown parameter: %s" arg
         exit 1
   let defaultBuildParam = {
      Help = false
      Documentation = false
      DocumentationRoot = "."
      Debug = false
      Deploy = false
      CleanDeploy = false
      NoZip = false
      NoNuGet = false
      Key = None
   }
   let args = fsi.CommandLineArgs |> Array.toList  // args = ["build.fsx"; ...]
   loop defaultBuildParam args.Tail

if buildParams.Help then
   printfn """FsRandom Build Script

#Usage

fsi.exe build.fsx [<options>]

# Options
-h | --help       Show this help
--docs            Build documentation files
--docs-root <uri> Specify the root uri of the documentation
                  Default: .
--debug           Debug build
--deploy          Create a zip archive and a NuGet package
                  See --no-zip and --no-nuget
--clean-deploy    Clean up deploy directory before build
--no-zip          Do not create zip archive
--no-nuget        Do not build NuGet package
--key <key_path>  Sign assembly with the specified key"""
   exit 0

let addBuildProperties =
   let debugSymbol properties =
      match buildParams.Debug with
         | true -> ("DebugSymbols", "true") :: ("DebugType", "full") :: properties
         | false -> ("DebugSymbols", "false") :: ("DebugType", "pdbonly") :: properties
   let setKey properties =
      match buildParams.Key with
         | Some (path) when File.Exists (path) -> ("SignAssembly", "true") :: ("AssemblyOriginatorKeyFile", path) :: properties
         | Some (path) -> failwithf "Key file not found at %s" path
         | None -> properties
   debugSymbol >> setKey
let configuration = "Configuration", if buildParams.Debug then "Debug" else "Release"
let setBuildParams (p:MSBuildParams) = {
   p with
      Targets = ["Build"]
      Properties = configuration :: addBuildProperties p.Properties
}
let setCleanParams (p:MSBuildParams) = {
   p with
      Targets = ["Clean"]
      Properties = [configuration]
}

Target "CleanBuild" (fun () ->
   build setCleanParams mainSolution
   CleanDir buildDir
)
Target "CleanDeploy" (fun () ->
   CleanDir deployDir
)
Target "Clean" DoNothing

let getProducts projectName =
   if buildParams.Debug then ["*"] else ["dll"; "XML"]
   |> Seq.collect (fun s -> % projectName % "bin" % snd configuration % (sprintf "*.%s" s) |> (!+) |> Scan)
Target "Build" (fun () ->
   build setBuildParams mainSolution
   getProducts projectName
   |> Copy buildDir
   !+ (buildDir % "**")
   |> Scan
   |> Log "Build-Output: "
)
Target "EnsureDeploy" (fun () ->
   ensureDirectory deployDir
)

let getMainAssemblyVersion assemblyPath =
   let assembly = Assembly.LoadFrom (assemblyPath)
   let infoVersion = assembly.GetCustomAttributes (typeof<AssemblyInformationalVersionAttribute>, false)
   (infoVersion.[0] :?> AssemblyInformationalVersionAttribute).InformationalVersion
let updateNuGetParams version (p:NuGetParams) = {
   p with
      NoPackageAnalysis = false
      OutputPath = deployDir
      ToolPath = nugetToolPath
      WorkingDir = baseDir
      Version = version
}
let pack projectName =
   let assemblyName = sprintf "%s.dll" projectName
   let assemblyPath = buildDir % assemblyName
   let version = getMainAssemblyVersion assemblyPath
   let nuspecPath = % (sprintf "%s.nuspec" projectName)
   NuGetPack (updateNuGetParams version) nuspecPath
Target "NuGet" (fun () ->
   ensureDirectory deployDir
   pack projectName
)

Target "Documentation" (fun () ->
   let info = [
      "project-name", "FsRandom"
      "project-author", "RecycleBin"
      "project-summary", "Purely functional random number generating framework designed for F#"
      "project-github", "https://github.com/kos59125/FsRandom"
      "project-nuget", "https://nuget.org/packages/FsRandom"
   ]

   // Paths with template/source/output locations
   let content    = % "docs"
   let templates  = content % "templates"
   let formatting = __SOURCE_DIRECTORY__ % "FSharp.Formatting"
   let docTemplate = formatting % "templates" % "docpage.cshtml"

   // Where to look for *.cshtml templates (in this order)
   let layoutRoots = [ templates; formatting % "templates"; formatting % "templates" % "reference" ]

   // Copy static files and CSS + JS from F# Formatting
   let copyFiles () =
      ensureDirectory (docsDir % "images")
      CopyRecursive (content % "images") (docsDir % "images") true
      |> Log "Copying images: "
      ensureDirectory (docsDir % "content")
      CopyRecursive (formatting % "content") (docsDir % "content") true 
      |> Log "Copying styles and scripts: "

   // Build documentation from `*.fsx` files in `docs`
   let buildDocumentation () =
      let subdirs = Directory.EnumerateDirectories(content, "*.fsx", SearchOption.AllDirectories)
      for dir in Seq.append [content] subdirs do
         let sub = if dir.Length > content.Length then dir.Substring(content.Length + 1) else "."
         Literate.ProcessDirectory
            ( dir, docTemplate, docsDir % sub, replacements = ("root", buildParams.DocumentationRoot)::info,
              layoutRoots = layoutRoots )

   let buildReference () =
      let referenceDir = docsDir % "reference"
      ensureDirectory referenceDir
      for lib in Directory.GetFiles (buildDir, "*.dll") do
         MetadataFormat.Generate
            ( lib, referenceDir, layoutRoots, parameters = ("root", buildParams.DocumentationRoot)::info )

   // Generate
   copyFiles()
   buildDocumentation()
   buildReference ()
)

Target "Zip" (fun () ->
   let files =
      if buildParams.Documentation && buildParams.DocumentationRoot = "." then
         !+ (buildDir % "*.*") ++ (docsDir % "**")
      else
         !+ (buildDir % "*.*")
   files
   |> Scan
   |> Zip buildDir zipName
)

Target "Deploy" (fun () ->
   !+ (deployDir % "*.*")
   |> Scan
   |> Log "Build-Output: "
)

// Clean dependency
"CleanBuild"
=?> ("CleanDeploy", buildParams.CleanDeploy)
==> "Clean"

// Build dependency
"Clean"
==> "Build"

// Documentation dependency
"Build"
==> "Documentation"

// NuGet dependency
"Build"
==> "EnsureDeploy"
==> "NuGet"

// Zip dependency
"Build"
==> "EnsureDeploy"
==> "Zip"

// Deploy dependency
"Build"
=?> ("Documentation", buildParams.Documentation)
=?> ("Zip", not buildParams.NoZip)
=?> ("NuGet", not buildParams.NoNuGet)
==> "Deploy"

let deploy = buildParams.Deploy && (not buildParams.NoZip || not buildParams.NoNuGet)
Run <| if deploy then "Deploy" elif buildParams.Documentation then "Documentation" else "Build"
