#r "../node_modules/fable-core/Fable.Core.dll"
#r "../node_modules/fable-react/Fable.React.dll"
#r "../node_modules/fable-elmish/Fable.Elmish.dll"
#r "../node_modules/fable-elmish-react/Fable.Elmish.React.dll"
#r "../node_modules/fable-powerpack/Fable.PowerPack.dll"

#load "../../shared/Types.fs"

open Fable.Core
open Fable.Import
open Shared.Types
open Elmish
open Fable.PowerPack
open Fable.PowerPack.Fetch
open Fable.PowerPack.Fetch.Fetch_types
module R = Fable.Helpers.React
open Fable.Core.JsInterop
open Fable.Helpers.React.Props
open Elmish.React
open Fable.Helpers.React
open Fable.Import.Browser
open Elmish.Browser.Navigation
open Elmish.UrlParser

type Model = 
    {results : AllResults option}

type Msg =
  | FetchSuccess of AllResults
  | FetchFailure of exn
  | FileUploaded of obj

let initialModel () = 
    {results = None}, Cmd.none 

let postData input =
    let formData = FormData.Create()
    formData.append("file.csv", input?files?item(0))
    let defaultProps =
        [ RequestProperties.Method HttpMethod.POST
        ;RequestProperties.Headers [unbox EncType "multipart/form-data"]
        ; RequestProperties.Body (formData |> unbox)]
    promise {
        return! Fable.PowerPack.Fetch.fetch ("/api/data") defaultProps
                |> Promise.bind (fun fetched -> fetched.text())
                |> Promise.map ofJson<AllResults>
    }

let update msg model : Model * Cmd<Msg> =
    match msg with
    | FetchSuccess data ->
        { model with results = Some data }, []
    | FetchFailure ex ->
        Browser.console.log (unbox ex.Message)
        Browser.console.log "exception occured" |> ignore
        model, []
    | FileUploaded files ->
        Browser.console.log "file selected!" |> ignore
        model, Cmd.ofPromise postData files FetchSuccess FetchFailure

let text s = text [] [unbox s ]

let getHeadLineResultCell results name = 
    text (
        match results with
        | Some x  -> 
            let result = x.HeadlineFigures |> List.find (fun y -> y.Name = name)
            sprintf "%i = %i%%" result.Count result.Percentage
        | _ ->  "")

let view model dispatch =
    div [][
        R.input [
            ClassName "input-control" 
            Type "file"
            OnChange (fun x -> FileUploaded x.target |> dispatch )
        ] []
        R.table [ClassName "table table-bordered"] [
            R.thead [][
                R.tr [][
                    R.th [ColSpan 8.0][unbox "Headline Figures"]
                ]
                R.tr [][
                    R.th [ColSpan 2.0][unbox "Green"]
                    R.th [ColSpan 2.0][unbox "Amber"]
                    R.th [ColSpan 2.0][unbox "Red"] 
                    R.th [ColSpan 2.0][unbox "Absent"] 
                ]
                R.tr [][
                    R.th [][unbox "Last AP"]
                    R.th [][unbox "This AP"]
                    R.th [][unbox "Last AP"]
                    R.th [][unbox "This AP"]
                    R.th [][unbox "Last AP"]
                    R.th [][unbox "This AP"]
                    R.th [][unbox "Last AP"]
                    R.th [][unbox "This AP"]
                ]
            ]
            R.tbody [][
                R.tr [][
                    R.td [][getHeadLineResultCell model.results "GreenAP1"]
                    R.td [][getHeadLineResultCell model.results "GreenAP2"]
                    R.td [][getHeadLineResultCell model.results "AmberAP1"]
                    R.td [][getHeadLineResultCell model.results "AmberAP2"]
                    R.td [][getHeadLineResultCell model.results "RedAP1"]
                    R.td [][getHeadLineResultCell model.results "RedAP2"]
                    R.td [][getHeadLineResultCell model.results "AbsentAP1"]
                    R.td [][getHeadLineResultCell model.results "AbsentAP2"]   
                ]
            ]
        ]
        R.table [ClassName "table table-bordered"] [
            R.thead [][
                R.tr [][
                    R.th [ColSpan 6.0][unbox "Key Cohorts"]
                ]
                R.tr [][
                    R.th [][unbox ""]
                    R.th [][unbox "Cohort Size"]
                    R.th [][unbox "Green"]
                    R.th [][unbox "Amber"] 
                    R.th [][unbox "Red"]
                    R.th [][unbox "Absent"] 
                ]
            ]
            R.tbody [][
                let getKeyCohortText cohort = 
                    text (sprintf "%i = %i%%" cohort.Count cohort.Percentage)
                match model.results with
                | Some x ->
                    for cohort in x.KeyCohorts do
                        yield
                            R.tr [][
                                R.td [][yield text cohort.Name]
                                R.td [][yield text cohort.CohortSize]
                                R.td [][yield getKeyCohortText cohort.Green]
                                R.td [][yield getKeyCohortText  cohort.Amber]
                                R.td [][yield getKeyCohortText  cohort.Red]
                                R.td [][yield getKeyCohortText cohort.Absent]
                            ]
                | _ -> ()
            ]
        ]
    ]

Program.mkProgram initialModel update view
|> Program.withConsoleTrace
|> Program.withReact "app"
|> Program.run 