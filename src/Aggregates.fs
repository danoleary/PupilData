module Aggregates

open System
open Shared.Types
open FileParser
open Filters

let countBy csv filters =
    let rec apply items filters =
        match filters with
        | [] -> items
        | _ -> 
            let filter = List.head filters
            let filteredItems = items |> Seq.filter filter
            apply filteredItems (List.tail filters)
    (apply csv filters) |> Seq.length

let getPercentage csv filters = 
    let totalPupils = (csv |> Seq.length) - 1 
    let colourCount = countBy csv filters
    System.Math.Round((colourCount |> float) / ((totalPupils |> float)) * 100.0,0) |> int

let getResult csv name demogs : Result =
    {
        Name = name;
        Count = countBy csv demogs;
        Percentage = getPercentage csv demogs;
    };

let getDemographicResultSet csv name demogFilter =
       {
           Name = name;
           CohortSize = countBy csv [demogFilter];
           Green = getResult csv "Green" [demogFilter; AP2Filter greenFilter];
           Amber = getResult csv "Amber" [demogFilter; AP2Filter amberFilter];
           Red = getResult csv "Red" [demogFilter; AP2Filter redFilter]
           Absent = getResult csv "Absent" [demogFilter; absentAP2];
       }

let getHeadlineFigures csv = 
    let partialResult = getResult csv
    [    
        partialResult "GreenAP1" [AP1Filter greenFilter];
        partialResult "GreenAP2" [AP2Filter greenFilter];
        partialResult "AmberAP1" [AP1Filter amberFilter];
        partialResult "AmberAP2" [AP2Filter amberFilter];
        partialResult "RedAP1" [AP1Filter redFilter];
        partialResult "RedAP2" [AP2Filter redFilter];
        partialResult "AbsentAP1" [absentAP1];
        partialResult"AbsentAP2" [absentAP2];
    ]

let getKeyCohorts csv = 
    let partialResult = getDemographicResultSet csv
    [
        partialResult "Boys" boys
        partialResult "Girls" girls
        partialResult "PP" pupilPremium
        partialResult "Non PP" nonPupilPremium
    ]

let getAllResults filePath =
    let csv = parseCsv filePath
    { HeadlineFigures = getHeadlineFigures csv; KeyCohorts = getKeyCohorts csv; }