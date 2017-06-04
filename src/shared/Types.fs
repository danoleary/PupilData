module Shared.Types

type Result = {
    Name : string
    Count : int
    Percentage : int
}

type DemographicResultSet = {
    Name : string
    CohortSize : int
    Green : Result
    Amber : Result
    Red : Result
    Absent: Result
}

type AllResults = {
    HeadlineFigures : Result list
    KeyCohorts : DemographicResultSet list
}