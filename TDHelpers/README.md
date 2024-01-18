Validation example with Json.NET Schema https://www.newtonsoft.com/jsonschema

'''

        using Newtonsoft.Json.Schema;
        
        //change the directory with your TD.
        var TDtxt = File.ReadAllText("Insert directory");

        //change the directory with JSON Schema for TD.
        string TDschema = File.ReadAllText("insert directory");

        //Validation, requires Newtonsoft.Json.Schema to import
        JSchema jsonSchema = JSchema.Parse(TDschema);
        JObject TD = JObject.Parse(TDtxt); 
        bool valid = TD.IsValid(jsonSchema);

        if (valid)
        {
            var myJSONTD = JsonConvert.DeserializeObject<ThingDescription>(TDtxt);
        }
        else Console.WriteLine("Your TD file is not valid.");
'''
