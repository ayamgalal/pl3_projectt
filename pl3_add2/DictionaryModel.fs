module DictionaryModel

open Newtonsoft.Json
open System.IO
open System.Xml.Serialization

type DigitalDictionary() =
    let mutable dictionary = Map.empty<string, string>

    member this.AddWord(word: string, definition: string) =
        if word.Trim() = "" || definition.Trim() = "" then
            failwith "Word and definition cannot be empty."
        elif Map.containsKey (word.ToLower()) dictionary then
            failwith "Word already exists."
        else
            dictionary <- dictionary.Add(word.ToLower(), definition)


