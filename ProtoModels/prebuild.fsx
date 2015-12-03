#I "../packages"
#r "FSharp.Data/lib/net40/FSharp.Data"
#r "System.Xml.Linq"
#r "FAKE/tools/NuGet.Core"
#r "FAKE/tools/Newtonsoft.Json"
#r "FAKE/tools/Microsoft.Web.XmlTransform"
#r "FAKE/tools/FakeLib"
open FSharp.Data
open System.Xml.Linq
open Fake
open System.IO
open System.Globalization;
open Fake.ProcessHelper

[<Literal>]
let __SCHEMAS_DIR__ = "schemas"
[<Literal>]
let __MODELS_DIR__ = "models"

type Csproj = XmlProvider<"ProtoModels.csproj">
let config = Csproj.Load("ProtoModels.csproj")
let textInfo = (new CultureInfo("en-US",false)).TextInfo


let getFilesNames dir =
    !! dir
    |> Seq.map Path.GetFileName

let getCsFileName (proto:string) =
    match proto.Split '.' with
    | [|name; proto|] ->
        textInfo.ToTitleCase name
        |> sprintf "%s.cs"
        |> Some
    | _ ->
        None

let getProtoFileName (cs:string) =
    match cs.Split '.' with
    | [|name;"cs"|] ->
        name.ToLower()
        |> sprintf "%s.proto"
        |> Some
    | _ ->
        None


let compileProtoFile fileName =
    let args = sprintf "%s/%s --csharp_out=%s" __SCHEMAS_DIR__ fileName __MODELS_DIR__
    trace <| sprintf "compiling %s" args
    match Shell.Exec("protoc", args) with
    |0 ->    
        getCsFileName fileName 
        |> Option.iter (fun f' ->
            let fileName = sprintf "%s/%s" __MODELS_DIR__ f'
            if File.Exists fileName  then
                config.ItemGroups.[1].XElement.Add(Csproj.Compile(fileName.Replace('/','\\')).XElement)
        )
    | other  ->
        traceError <| sprintf "Error trying to compile exitCode %d, args : %s" other args

let isOutdatedOrNotExist protofile =
    getCsFileName protofile
    |> Option.fold(fun acc csFile->
        let protoInfo = FileInfo(Path.Combine [|__SCHEMAS_DIR__;protofile|])
        let csName = Path.Combine [|__MODELS_DIR__; csFile|]
        if File.Exists csName then
            let csInfo = FileInfo(csName)
            printfn "%A %A"  protoInfo.LastWriteTimeUtc  csInfo.LastWriteTime
            protoInfo.LastWriteTimeUtc > csInfo.LastWriteTime
        else
            true
    ) false
   
let compileOutdatedAndNonExistent () =
    Path.Combine [|__SCHEMAS_DIR__; "*.proto"|]
    |> getFilesNames 
    |> Seq.filter isOutdatedOrNotExist
    |> Seq.iter compileProtoFile


  
let removeCsFiles (filesToRemove: Set<string>)  =
    // removing from csproj 
    config.ItemGroups.[1].Compiles
    |> Array.filter (fun item -> 
        match item.Include.Split ('\\') with
        | [| __MODELS_DIR__ ; name|] 
            when filesToRemove.Contains name -> true
        | _ -> false )
    |> Array.iter( fun item ->
        item.XElement.Remove()
    )
    //deleting the file 
    filesToRemove
    |> Set.map (sprintf "%s/%s" __MODELS_DIR__)
    |> Set.filter File.Exists
    |> Set.iter File.Delete


Target "ProtoBuf" (fun _ ->
    compileOutdatedAndNonExistent ()

    let csFiles =
        Path.Combine [|__MODELS_DIR__; "*.cs"|]
        |> getFilesNames

    Path.Combine [|__SCHEMAS_DIR__; "*.proto"|]
    |> getFilesNames 
    |> Seq.choose getCsFileName
    |> Set.ofSeq
    |> Set.difference (Set.ofSeq csFiles)
    |> removeCsFiles


    config.XElement.Save("ProtoModels.csproj")
)

RunTargetOrDefault "ProtoBuf"