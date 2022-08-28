using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighSchoolBruisical
{
    public class Selection
    {
        public int player1, player2, level;
        private string player1name, player2name, levelname;

        public string GetPlayer1()
        {
            return player1name;
        }

        public string GetPlayer2()
        {
            return player2name;
        }

        public string GetLevel()
        {
            return levelname;
        }

        public Selection()
        {
            player1name = "none";
            player2name = "none";
            levelname = "none";
        }

        public void SetPlayer1(int p1)
        {
            player1 = p1;
            if (player1 == 0)
                player1name = "BillyNoMates";
            else if (player1 == 1)
                player1name = "Blazer";
            else if (player1 == 2)
                player1name = "Chubbernaught";
            else if (player1 == 3)
                player1name = "Max";
            else if (player1 == 4)
                player1name = "Janitor";
            else if (player1 == 5)
                player1name = "Headmaster";
        }

        public void SetPlayer2(int p2)
        {
            player2 = p2;
            if (player2 == 0)
                player2name = "BillyNoMates";
            else if (player2 == 1)
                player2name = "Blazer";
            else if (player2 == 2)
                player2name = "Chubbernaught";
            else if (player2 == 3)
                player2name = "Max";
            else if (player2 == 4)
                player2name = "Janitor";
            else if (player2 == 5)
                player2name = "Headmaster";
        }

        public void SetLevel(int lvl)
        {
            level = lvl;
            if (level == 0)
                levelname = "room";
            else if (level == 1)
                levelname = "yard";
            else if (level == 2)
                levelname = "gym";
            else if (level == 3)
                levelname = "hallway";
        }
    }
}
