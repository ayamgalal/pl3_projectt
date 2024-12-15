open System
open System.Drawing
open System.Windows.Forms
open System.IO
open DictionaryModel
[<STAThread>]
[<EntryPoint>]
let main _ =
    let dictionary = DigitalDictionary()

    // Calm color theme setup
    let themeBackgroundColor = Color.FromArgb(240, 240, 240)  // Light Gray for background
    let themeButtonColor = Color.FromArgb(200, 200, 255)      // Light Blue for buttons
    let themeTextColor = Color.Black                          // Black text
    let themeFont = new Font("Segoe UI", 9.0f, FontStyle.Regular)

    let form = new Form(Text = "Digital Dictionary", Width = 400, Height = 350, StartPosition = FormStartPosition.CenterScreen)
    form.BackColor <- themeBackgroundColor
    form.Font <- themeFont

    // Create UI components
    let wordLabel = new Label(Text = "Word:", Top = 20, Left = 10, Width = 80, ForeColor = themeTextColor)
    let wordTextBox = new TextBox(Top = 20, Left = 100, Width = 150)

    let definitionLabel = new Label(Text = "Definition:", Top = 60, Left = 10, Width = 80, ForeColor = themeTextColor)
    let definitionTextBox = new TextBox(Top = 60, Left = 100, Width = 150)

    let resultListBox = new ListBox(Top = 100, Left = 10, Width = 360, Height = 100)
    resultListBox.BackColor <- Color.Lavender
    resultListBox.ForeColor <- themeTextColor

    let createButton (text: string) (top: int) (left: int) =
        let button = new Button(Text = text, Top = top, Left = left, Width = 80, Height = 30)
        button.BackColor <- themeButtonColor
        button.ForeColor <- themeTextColor
        button.FlatStyle <- FlatStyle.Flat
        button.FlatAppearance.BorderSize <- 0
        button.Font <- themeFont
        button

    let addButton = createButton "Add" 210 10
    let updateButton = createButton "Update" 210 100
    let deleteButton = createButton "Delete" 210 190
    let searchByKeywordButton = createButton "Search W" 250 10
    let searchByDefinitionButton = createButton "Search D" 250 100
    let saveButton = createButton "Save" 250 190
    let loadButton = createButton "Load" 250 280

    // Add components to form
    form.Controls.AddRange([|
        wordLabel; wordTextBox; definitionLabel; definitionTextBox;
        resultListBox; addButton; updateButton; deleteButton;
        searchByKeywordButton; searchByDefinitionButton; saveButton; loadButton
    |])

    // Handling the update button click event
    updateButton.Click.Add(fun _ ->
        try
            dictionary.UpdateWord(wordTextBox.Text, definitionTextBox.Text)
            MessageBox.Show("Word updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information) |> ignore
        with ex ->
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore
    )

    // Handling the add button click event
    addButton.Click.Add(fun _ ->
        try
            dictionary.AddWord(wordTextBox.Text, definitionTextBox.Text)
            MessageBox.Show("Word added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information) |> ignore
            wordTextBox.Clear()
            definitionTextBox.Clear()
        with ex ->
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore
    )

    // Handling the save button click event
    saveButton.Click.Add(fun _ -> 
    let saveFileDialog = new SaveFileDialog()
    saveFileDialog.Filter <- "JSON files (*.json)|*.json|XML files (*.xml)|*.xml"
    saveFileDialog.Title <- "Save Dictionary"
    saveFileDialog.FileName <- "dictionary" // Default file name

    if saveFileDialog.ShowDialog() = DialogResult.OK then
        let filePath = saveFileDialog.FileName
        try
            if dictionary.SaveToFile(filePath) then
                MessageBox.Show("Dictionary saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information) |> ignore
                MessageBox.Show($"The dictionary has been saved to: {filePath}", "Save Complete", MessageBoxButtons.OK, MessageBoxIcon.Information) |> ignore
            else
                MessageBox.Show("Failed to save the dictionary.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore
        with ex -> 
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore
    )

    // Handling the delete button click event
    deleteButton.Click.Add(fun _ ->
        try
            dictionary.DeleteWord(wordTextBox.Text)
            if dictionary.SaveToFile("dictionary.json") then
                MessageBox.Show("Word deleted and dictionary saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information) |> ignore
            else
                MessageBox.Show("Failed to save the dictionary after deletion.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore
            wordTextBox.Clear()
            definitionTextBox.Clear()
        with ex ->
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore
    )

    // Handling the search by keyword button click event
    searchByKeywordButton.Click.Add(fun _ ->
        let keyword = wordTextBox.Text
        let results = dictionary.SearchByKeyword(keyword)
        resultListBox.Items.Clear()
        if Seq.isEmpty results then
           MessageBox.Show("No results found for the given keyword.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information) |> ignore
        else
        results |> Seq.iter (fun (key, value) ->
            resultListBox.Items.Add($"{key}: {value}") |> ignore
        )
    )

    // Handling the search by definition button click event
    searchByDefinitionButton.Click.Add(fun _ ->
        let definition = definitionTextBox.Text
        let results = dictionary.SearchByDefinition(definition)
        resultListBox.Items.Clear()
        if Seq.isEmpty results then
           MessageBox.Show("No results found for the given keyword.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information) |> ignore
        else
        results |> Seq.iter (fun (key, value) ->
            resultListBox.Items.Add($"{key}: {value}") |> ignore
        )
        MessageBox.Show("Definition search completed.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information) |> ignore
    )

    
    loadButton.Click.Add(fun _ -> 
    let openFileDialog = new OpenFileDialog()
    openFileDialog.Filter <- "JSON files (.json)|.json|XML files (.xml)|.xml"
    openFileDialog.Title <- "Load Dictionary"

    if openFileDialog.ShowDialog() = DialogResult.OK then
        let filePath = openFileDialog.FileName
        try
            if dictionary.LoadFromFile(filePath) then
                MessageBox.Show("Dictionary loaded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information) |> ignore
                resultListBox.Items.Clear()
                dictionary.SearchByKeyword("") 
                |> Seq.iter (fun (key, value) -> 
                    resultListBox.Items.Add($"{key}: {value}") |> ignore
                )
            else
                MessageBox.Show("Failed to load the dictionary.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore
        with ex -> 
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore
    )

    // Start the form
    Application.Run(form)
    0
