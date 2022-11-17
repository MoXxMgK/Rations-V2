using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Rations_V2.Utils
{
    public static class FilePaths
    {
        public static readonly string DataFolderName = "Data";
        public static readonly string FoodGroupsDataFolder = "FoodGroups";
        public static readonly string CowsDataFolder = "Cows"; // Not used yet

        /* private static Dictionary<int, string> _foodGroupsDataMap = new()
         {
             { 1, "Hay"},
             { 3, "Silage"},
             { 4, "Straw"},
             { 5, "Concentrates" },
             { 6, "Roots"},
             { 7, "GreenForege"},
             { 8, "Other"},
             { 9, "FodderfromDestillery" },
             { 10, "FodderfromExtracted" },
             { 11, "Fodderfrommillfeeds" },
             { 12, "Foddersugar" },
         };*/
         private static Dictionary<int, string> _foodGroupsDataMap = new()
        {
            { 1, "Foddersugar" },
            { 2, "FodderfromExtracted" },
            { 3, "Concentrates" },
            { 4, "FodderfromDestillery"},
            { 5, "Fodderfrommillfeeds" },
            { 6, "Roots"},
            { 8, "Hay"},
            { 9, "Silage"},
            { 10, "Straw"},
            { 11, "GreenForege"},
            { 12, "Other"},
        };

        public static string GetFoodGroupDataPathById(int id)
        {
            string cwd = Directory.GetCurrentDirectory();

            if (_foodGroupsDataMap.Keys.Contains(id))
            {

                string dataPath = Path.Combine(cwd, DataFolderName, FoodGroupsDataFolder, _foodGroupsDataMap[id] + ".xml");
                return dataPath;
            }

            return "";
        }
    }
}
