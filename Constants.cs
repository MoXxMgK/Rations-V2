using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rations_V2
{
    public static class Constants
    {
        public static readonly string AppName = "PRiMM";

       public static readonly CultureInfo CultureInfo = new("en-US");
      // public static readonly CultureInfo CultureInfo = new("cs-CS");

        #region FoodGroupNames
        public static readonly List<(int, string)> FoodGroupsData = new List<(int, string)>()
        {
            (1, "Cukrovarská krmiva"),
            (2, "Extrahované šroty"),
            (3, "Jadrná krmiva"),
            (4, "Lihovarské, pivovarské, pekárenské a škrobárenské zbytky"),
            (5, "Mlýnská krmiva"),
            (6, "Okopaniny"),
            (7, "Senáž"),
            (8, "Seno"),
            (9,"Siláže"),
            (10,"Sláma"),
            (11, "Zelená píce"),
            (12, "Jiné")
        };
        #endregion

        #region CowIds
        public static readonly int IdLactatingCows = 1;
        public static readonly int IdFirstCalfHeifers = 2;
        public static readonly int IdHeifers6_12 = 3;
        public static readonly int IdHeifers12_24 = 4;
        public static readonly int IdBullCalves6_12 = 5;
        public static readonly int IdBullCalves12_24 = 6;
        public static readonly int IdFattening = 7;
        #endregion

        #region Математические константы
        public static readonly double AL = 0.35;
        public static readonly double KP_MEAT = 0.04;
        public static readonly double TF_MILK_CS = 1;
        public static readonly double TF_MILK_SR = 0.16;
        public static readonly double TF_MEAT_6_12_CS = 14;
        public static readonly double TF_MEAT_12_CS = 7;
        public static readonly int T1 = 3;
        public static readonly int T2 = 55;
        #endregion
    }
}
