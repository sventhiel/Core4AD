﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BExIS.Modules.Asm.UI.Models
{
    public class SpecialDataset_analysis
    {
        public SpecialDataset_analysis(JToken jtok)
        {
            foreach (JProperty xx in jtok)
            {
                if (xx.Name == "key1")
                    key1 = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(xx.Value.ToString());
                else if (xx.Name == "key2")
                    if (xx.Value.ToString() == "")
                        key2 = new Dictionary<JToken, JObject>();
                    else
                        key2 = JsonConvert.DeserializeObject<Dictionary<JToken, JObject>>(xx.Value.ToString());
            }
        }
        public List<Dictionary<string, string>> key1 { get; set; }
        public Dictionary<JToken, JObject> key2 { get; set; }
    }


}