// include Fake libs
#r "./packages/FAKE/tools/FakeLib.dll"
#r @"FakeLib.dll"

open Fake
open Fake.Azure
open System
open System.IO

Environment.CurrentDirectory <- __SOURCE_DIRECTORY__
let solutionFile = "src/SchoolData.fsproj"

Target "StageWebsiteAssets" (fun _ ->
    let blacklist =
        [ "typings"
          ".fs"
          ".references"
          "tsconfig.json" ]
    let shouldInclude (file:string) =
        blacklist
        |> Seq.forall(not << file.Contains)
    Kudu.stageFolder (Path.GetFullPath @"src/WebHost") shouldInclude)
    

Target "BuildSolution" (fun _ ->
    solutionFile
    |> MSBuildHelper.build (fun defaults ->
        { defaults with
            Verbosity = Some Minimal
            Targets = [ "Build" ]
            Properties = [ "Configuration", "Release"
                           "OutputPath", Kudu.deploymentTemp ] })
    |> ignore)

Target "Deploy" Kudu.kuduSync

"StageWebsiteAssets"
==> "BuildSolution"
==> "Deploy"

RunTargetOrDefault "Deploy"