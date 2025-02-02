﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BExIS.Modules.Asm.Etities
{
    public class Classification
    {
        //public Input var_name;
        public List<Input> class_results;
        public List<string> keywords;
        public Classification()
        {
            this.class_results = new List<Input>();
            this.keywords = new List<string>();
        }
    }

    public class Input
    {
        public string entity_id;
        public string entity;
        public string charachteristic_id;
        public string variable_id_from_table;
        public string variable_value;
        public string predicted_class;
        public List<string> onto_match;
        public List<string> onto_no_path;
        public List<string> onto_no_node;
        public List<string> db_match; 
        public List<string> db_no_node;
        public List<string> onto_target_file;
        public List<string> db_no_path;
    }

}