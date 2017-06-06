open System.IO
open Suave                
open Suave.Successful    
open Suave.Web            
open Suave.Operators
open Suave.Filters
open Newtonsoft.Json
open Aggregates

[<EntryPoint>]
let main args =
  let port =  System.UInt16.Parse args.[0]
  let ip = System.Net.IPAddress.Parse "127.0.0.1"
  let serverConfig =
    { Web.defaultConfig with
        homeFolder = Some __SOURCE_DIRECTORY__
        bindings = [ HttpBinding.create HTTP ip port ] }

  let clientRoot = Path.Combine(__SOURCE_DIRECTORY__, "client")

  let noCache =
    Writers.setHeader "Cache-Control" "no-cache, no-store, must-revalidate"
    >=> Writers.setHeader "Pragma" "no-cache"
    >=> Writers.setHeader "Expires" "0"

  let app = 
        choose [
            POST >=> 
                path "/api/data" 
                    >=> request 
                        (fun x -> 
                            let results = getAllResults x.files.Head.tempFilePath
                            OK (JsonConvert.SerializeObject  results) )

            path "/bundle.js" >=> noCache >=> Files.browseFile clientRoot (Path.Combine("public", "bundle.js"))

            path "/" >=> Files.browseFile clientRoot "index.html"
        ]

  startWebServer serverConfig app
  0