module Utilities

open System
open System.IO
open System.Text.Json

let readJson<'T> (filePath: string) : 'T list =
    if File.Exists(filePath) then
        let json = File.ReadAllText(filePath)
        if String.IsNullOrWhiteSpace(json) then
            [] 
        else
            try
                JsonSerializer.Deserialize<'T list>(json)
            with
            | :? System.Text.Json.JsonException ->
                printfn "Warning: Invalid JSON in file '%s'. Returning an empty list." filePath
                []
    else
        [] 

let writeJson<'T> (filePath: string) (data: 'T list) =
    let json = JsonSerializer.Serialize(data)
    File.WriteAllText(filePath, json)

let initializeFile filePath =
    if not (File.Exists(filePath)) then
        File.WriteAllText(filePath, "[]")

let rec tryFindInList predicate list =
    match list with
    | [] -> None
    | head :: tail -> if predicate head then Some head else tryFindInList predicate tail

let rec mapOverList f list =
    match list with
    | [] -> []
    | head :: tail -> f head :: mapOverList f tail

let rec filterListByCondition predicate list =
    match list with
    | [] -> []
    | head :: tail -> 
        if predicate head then 
            head :: filterListByCondition predicate tail
        else 
            filterListByCondition predicate tail

let rec applyToEach f list =
    match list with
    | [] -> ()
    | head :: tail -> 
        f head
        applyToEach f tail

let rec existsInList predicate list =
    match list with
    | [] -> false
    | head :: tail -> 
        if predicate head then 
            true
        else 
            existsInList predicate tail