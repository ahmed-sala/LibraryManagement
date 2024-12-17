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
