#r @"FakeLib.dll"
#r @"ICSharpCode.SharpZipLib.dll"

open System
open System.IO
open System.Reflection
open Fake

let baseDir = Path.GetDirectoryName (__SOURCE_DIRECTORY__)
let inline (~%) name = Path.Combine (baseDir, name)
let inline (%) dir name = Path.Combine (dir, name)

let nugetToolPath = % ".nuget" % "NuGet.exe"
let buildDir = % "Build"
let deployDir = % "Deploy"

let mainSolution = % "FsRandom" % "FsRandom.fsproj"
let projectName = "FsRandom"
let zipName = deployDir % "FsRandom.zip"

type BuildParameter = {
   Help : bool
   Debug : bool
   CleanDeploy : bool
   NoZip : bool
   NoNuGet : bool
   Key : string option
}
let buildParams =
   let rec loop acc = function
      | [] -> acc
      | "-h" :: _ | "--help" :: _ -> { acc with Help = true }  // don't care other arguments
      | "--debug" :: args -> loop { acc with Debug = true } args
      | "--clean-deploy" :: args -> loop { acc with CleanDeploy = true } args
      | "--no-zip" :: args -> loop { acc with NoZip = true } args
      | "--no-nuget" :: args -> loop { acc with NoNuGet = true } args
      | "--no-deploy" :: args -> loop { acc with NoZip = true; NoNuGet = true } args
      | "--key" :: path :: args -> loop { acc with Key = Some (path) } args
      | arg :: _ ->
         eprintfn "Unknown parameter: %s" arg
         exit 1
   let defaultBuildParam = {
      Help = false
      Debug = false
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
--debug           Debug build
--clean-deploy    Clean up deploy directory before build
--no-zip          Do not create zip archive
--no-nuget        Do not build NuGet package
--no-deploy       Same as --no-zip --no-nuget
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
   !+ (buildDir % "*.*")
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

Target "Zip" (fun () ->
   !+ (buildDir % "*.*")
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
=?> ("Zip", not buildParams.NoZip)
=?> ("NuGet", not buildParams.NoNuGet)
==> "Deploy"

let deploy = not buildParams.NoZip || not buildParams.NoNuGet
Run <| if deploy then "Deploy" else "Build"
