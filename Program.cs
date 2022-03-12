using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace BEAWebScrape
{
    class Program
    {
        HtmlWeb web = new HtmlWeb();
        string url = "https://champs.britishesports.org/tournaments/lol-spring-div-3/teams/vortexexe/matches";
        HtmlDocument container;

        List<string> teamUrls = new List<string>
        {
            "https://champs.britishesports.org/tournaments/lol-spring-div-1/teams/exeultedexe/matches",
            "https://champs.britishesports.org/tournaments/lol-spring-div-1/teams/hexedexe/matches",
            "https://champs.britishesports.org/tournaments/ow-spring-div-2/teams/legionexe/matches",
            "https://champs.britishesports.org/tournaments/ow-spring-div-1/teams/centurionexe/matches",
            "https://champs.britishesports.org/tournaments/rl-spring-div-2/teams/phoenixexe/matches",
            "https://champs.britishesports.org/tournaments/rl-spring-div-1/teams/eaglesexe/matches",
            "https://champs.britishesports.org/tournaments/valorant-student-spring-cup/teams/aresexe/matches",
            "https://champs.britishesports.org/tournaments/lol-spring-div-3/teams/vortexexe/matches"
        };

        static void Main(string[] args)
        {
            Program program = new Program();
            program.Run();
        }

        void Run()
        {
            while (true)
            {
                DisplayMenu();

                string res = Console.ReadLine();
                bool match = false;
                bool spec = false;

                switch (res)
                {
                    case "1":
                        match = true;
                        url = teamUrls[0];
                        break;

                    case "2":
                        match = true;
                        url = teamUrls[1];
                        break;

                    case "3":
                        match = true;
                        url = teamUrls[2];
                        break;

                    case "4":
                        match = true;
                        url = teamUrls[3];
                        break;

                    case "5":
                        match = true;
                        url = teamUrls[4];
                        break;

                    case "6":
                        match = true;
                        url = teamUrls[5];
                        break;

                    case "7":
                        match = true;
                        url = teamUrls[6];
                        break;

                    case "8":
                        match = true;
                        url = teamUrls[7];
                        break;

                    case "9":
                        spec = true;
                        break;
                }

                if (!match && !spec)
                {
                    Console.WriteLine("\nInvalid input");
                }
                else if (!spec && match)
                {
                    List<Match> matches = new List<Match>();

                    try
                    {
                        LoadWebpage();
                        FillMatchList(matches);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Error: No internet connection, or response could not be recieved by the application (Check your firewall settings)"); 
                    }
                }
                else
                {
                    List<Match> currentMatches = CreateMatchDayData();

                    foreach (Match foo in currentMatches)
                    {
                        int i = 1;

                        Console.WriteLine("");
                        Console.WriteLine("Team " + i + ":");
                        Console.WriteLine("Home team: " + foo.home);
                        Console.WriteLine("Away team: " + foo.away);
                        Console.WriteLine("");

                        i += 1;
                    }

                    Console.WriteLine("\nDo you want a composite graphic or seperate match graphics? 1 or 2");
                    var ans = Console.ReadLine();
                    var fs = SetMatchdayFile();

                    if (ans != "1" && ans != "2")
                    {
                        Console.WriteLine("Incorrect response");
                    }
                    else if (ans == "1")
                    {
                        using (StreamWriter sw = new StreamWriter(fs))
                        {
                            sw.WriteLine("home0,home1,home2,home3,home4,home5,home6,away0,away1,away2,away3,away4,away5,away6,homeScore0,homeScore1,homeScore2,homeScore3,homeScore4,homeScore5,homeScore6,awayScore0,awayScore1,awayScore2,awayScore3,awayScore4,awayScore5,awayScore6");
                            for (int i = 0; i < 4; i++)
                            {
                                for (int j = 0; j < 7; j++)
                                {
                                    switch (i)
                                    {
                                        case 0:
                                            sw.Write(currentMatches[j].home + ",");
                                            break;
                                        case 1:
                                            sw.Write(currentMatches[j].away + ",");
                                            break;
                                        case 2:
                                            sw.Write(currentMatches[j].homeScore + ",");
                                            break;
                                        case 3:
                                            if (j != 6)
                                            {
                                                sw.Write(currentMatches[j].awayScore + ",");
                                            }
                                            else
                                            {
                                                sw.Write(currentMatches[j].awayScore);
                                            }
                                            break;
                                    }
                                }
                            }
                            
                        }
                    }
                    else if (ans == "2")
                    {
                        using (StreamWriter sw = new StreamWriter(fs))
                        {
                            var fields = currentMatches[0].GetType().GetFields();

                            for (int i = 0; i < fields.Length; i++)
                            {
                                if (i == fields.Length - 1)
                                {
                                    sw.Write(fields[i].Name + sw.NewLine);
                                }
                                else
                                {
                                    sw.Write(fields[i].Name + ",");
                                }
                            }

                            foreach (Match currentMatch in currentMatches)
                            {
                                sw.WriteLine(currentMatch.matchId + "," + currentMatch.home + "," + currentMatch.away + "," + currentMatch.homeScore + "," + currentMatch.awayScore + "," + currentMatch.matchState);
                            }

                            Console.WriteLine("File has been created");
                        }
                    }
                }
            }
        }

        public FileStream SetMatchdayFile()
        {
            try
            {
                File.Delete(@"matchday.csv");
            }
            catch (Exception)
            {
                Console.WriteLine("Couldn't find matchday file, creating one instead");
            }
            var fs = File.Create(@"matchday.csv");

            return fs;
        }
        public void DisplayMenu()
        {
            Console.WriteLine("\nSelect an option:");
            Console.WriteLine("1. Exeulted.exe");
            Console.WriteLine("2. Hexed.exe");
            Console.WriteLine("3. Legion.exe");
            Console.WriteLine("4. Centurions.exe");
            Console.WriteLine("5. Phoenix.exe");
            Console.WriteLine("6. Eagles.exe");
            Console.WriteLine("7. Ares.exe");
            Console.WriteLine("8. Vortex.exe");
            Console.WriteLine("9. Create all graphic");
        }

        public void LoadWebpage()
        {
            container = web.Load(url);
        }

        public List<Match> CreateMatchDayData()
        {
            List<Match> matches = new List<Match>();

            for (int i = 0; i < teamUrls.Count; i++)
            {
                url = teamUrls[i];
                LoadWebpage();

                var matchNodes = container.DocumentNode.SelectNodes(@"//tr[position()>1]");

                for (int j = matchNodes.Count - 1; j >= 0;)
                {
                    var columns = matchNodes[j].SelectNodes(@"td");

                    if (columns[4].GetAttributeValue("title", "not found") == "Scheduled")
                    {
                        j--;
                    }
                    else
                    {
                        Match match = new Match();

                        match.matchId = int.Parse(columns[0].InnerText.Trim());
                        match.home = columns[2].InnerText.Trim();
                        match.away = columns[5].InnerText.Trim();

                        int result1;
                        bool try1 = int.TryParse(columns[3].InnerText.Trim(), out result1);

                        if (try1)
                        {
                            match.homeScore = result1;
                        }
                        else
                        {
                            match.homeScore = null;
                        }

                        int result2;
                        bool try2 = int.TryParse(columns[4].InnerText.Trim(), out result2);

                        if (try2)
                        {
                            match.awayScore = result2;
                        }
                        else
                        {
                            match.awayScore = null;
                        }

                        match.matchState = columns[4].GetAttributeValue("title", "not found");

                        matches.Add(match);
                        break;
                    }
                }
            }
            return matches;
        }

        public void FillMatchList(List<Match> matches)
        {
            var matchNodes = container.DocumentNode.SelectNodes(@"//tr[position()>1]");

            for (int i = 0; i < matchNodes.Count; i++)
            {
                var columns = matchNodes[i].SelectNodes(@"td");

                Match match = new Match();

                match.matchId = int.Parse(columns[0].InnerText.Trim());
                match.home = columns[2].InnerText.Trim();
                match.away = columns[5].InnerText.Trim();

                int result1;
                bool try1 = int.TryParse(columns[3].InnerText.Trim(), out result1);

                if (try1)
                {
                    match.homeScore = result1;
                }
                else
                {
                    match.homeScore = null;
                }

                int result2;
                bool try2 = int.TryParse(columns[4].InnerText.Trim(), out result2);

                if (try2)
                {
                    match.awayScore = result2;
                }
                else
                {
                    match.awayScore = null;
                }

                match.matchState = columns[4].GetAttributeValue("title", "not found");

                matches.Add(match);
            }

            foreach (Match match in matches)
            {
                Console.WriteLine("");
                Console.WriteLine("Home: " + match.home);
                Console.WriteLine("Away: " + match.away);
                Console.WriteLine("Home score: " + match.homeScore);
                Console.WriteLine("Away score: " + match.awayScore);
                Console.WriteLine("Match state: " + match.matchState);
                Console.WriteLine("");
            }
        }
    }

    class Match
    {
        public int matchId;
        public string home;
        public string away;

        public int? homeScore;
        public int? awayScore;
        public string matchState;
    }
}
