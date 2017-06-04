module Filters

open FileParser

let AP1Filter colourFilter = fun (x: PupilData.Row) -> 
                        match System.Double.TryParse(x.``Above / Below``) with
                        | (true, f) -> colourFilter f
                        | _ -> false

let AP2Filter colourFilter = fun (x: PupilData.Row) -> 
                        match System.Double.TryParse(x.``Above / Below 3``) with
                        | (true, f) -> colourFilter f
                        | _ -> false

let greenFilter = fun x ->x >= 0.0
let amberFilter = fun x -> x = -1.0
let redFilter = fun x -> x < -1.0

let absentAP1 = fun (x: PupilData.Row) -> 
                        match System.Double.TryParse(x.``Above / Below``) with
                        | (true, f) -> false
                        | _ -> true
let absentAP2 = fun (x: PupilData.Row) -> 
                        match System.Double.TryParse(x.``Above / Below 3``) with
                        | (true, f) -> false
                        | _ -> true
let boys = fun (x: PupilData.Row) -> x.Gender = "M" 
let girls = fun (x: PupilData.Row) -> x.Gender = "F" 
let pupilPremium = fun (x: PupilData.Row) -> x.``Pupil Premium Indicator`` = "Y"
let nonPupilPremium = fun (x: PupilData.Row) -> x.``Pupil Premium Indicator`` = "N"