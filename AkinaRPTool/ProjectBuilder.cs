﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AkinaRPTool
{

    class ProjectBuilder
    {
        public static void BuildProject(string outputFile)
        {
            var data = JsonConvert.SerializeObject(MainWindow.clothes, Formatting.Indented);

            File.WriteAllText(outputFile, data);
        }

        public static void LoadProject(string inputFile)
        {
            string dir = Path.GetDirectoryName(inputFile);
            var data = JsonConvert.DeserializeObject<List<ClothData>>(File.ReadAllText(inputFile));

            MainWindow.clothes.Clear();

            var _clothes = data.OrderBy(x => x.Name, new AlphanumericComparer()).ToList();

            foreach (var cd in _clothes)
            {
                MainWindow.clothes.Add(cd);
            }
        }
    }

}