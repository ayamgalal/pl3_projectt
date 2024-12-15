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
    member this.DeleteWord(word: string) =
        if word.Trim() = "" then
            failwith "Word cannot be empty."
        dictionary <- dictionary.Remove(word.ToLower())

    //Search function   
    member this.SearchWord(word: string) =
        dictionary.TryFind(word.ToLower())

    member this.SearchByKeyword(keyword: string) =
        let trimmedKeyword = keyword.Trim().ToLower()
        dictionary
        |> Map.filter (fun key value ->
            trimmedKeyword = "" || key.Contains(trimmedKeyword) || value.Contains(trimmedKeyword))
        |> Map.toSeq



    // search by definition
    member this.SearchByDefinition(definition: string) =
     let trimmedDefinition = definition.Trim().ToLower()
     if trimmedDefinition = "" then
         Seq.empty // Return an empty sequence if the definition is empty
     else
         dictionary
         |> Map.filter (fun key value -> value.ToLower().Contains(trimmedDefinition))
         |> Map.toSeq
         
    member this.LoadFromFile(filePath: string) =
        try
            if filePath.EndsWith(".json") then
                let json = File.ReadAllText(filePath)
                dictionary <- JsonConvert.DeserializeObject<Map<string, string>>(json)
            elif filePath.EndsWith(".xml") then
                let serializer = XmlSerializer(typeof<Map<string, string>>)
                use reader = new StreamReader(filePath)
                dictionary <- serializer.Deserialize(reader) :?> Map<string, string>
            else
                failwith "Unsupported file format."
            true
        with
        | ex -> 
            printfn "Error loading file: %s" ex.Message
            false
            