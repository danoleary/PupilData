module FileParser

open FSharp.Data
open System

type PupilData = CsvProvider<"./SampleFile.csv", HasHeaders=true>

let parseCsv (filePath: string) = 
    let csv = PupilData.Load filePath
    csv.Rows