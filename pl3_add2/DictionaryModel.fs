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
    member this.UpdateWord(word: string, definition: string) =
        if word.Trim() = "" || definition.Trim() = "" then
            failwith "Word and definition cannot be empty."
        elif not (Map.containsKey (word.ToLower()) dictionary) then
            failwith "Word does not exist."
        else
            dictionary <- dictionary.Add(word.ToLower(), definition)

    member this.SaveToFile(filePath: string) =
        try
            if filePath.EndsWith(".json") then
                let json = JsonConvert.SerializeObject(dictionary, Formatting.Indented)
                File.WriteAllText(filePath, json)
            elif filePath.EndsWith(".xml") then
                let serializer = XmlSerializer(typeof<Map<string, string>>)
                use writer = new StreamWriter(filePath)
                serializer.Serialize(writer, dictionary)
            else
                failwith "Unsupported file format."
            true
        with
        | ex -> 
            printfn "Error saving file: %s" ex.Message
            false

