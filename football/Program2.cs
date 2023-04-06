using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Program2
{
    public static int RoundInput()
    {
            try
            {
                Console.Write("Enter the number of rounds: ");
                int rounds = int.Parse(Console.ReadLine()!);
                if(rounds > 32)
                {
                    throw new Exception();
                }
                return rounds;
            }
            catch (Exception)
            {
                Console.WriteLine("No such round, valid rounds are 1-32");
                return RoundInput();
            }
    }
    static void Main(string[] args)
    {
        // Read the teams file
        var teamLines = File.ReadAllLines("teams.csv").Skip(1);
        var teams = teamLines.Select(line =>
        {
            var fields = line.Split(';');
            if (fields.Length < 2) // make sure there are at least 2 fields (abbreviation and name)
            {
                return null; // skip this line
            }
            return new Team
            {
                Abbreviation = fields[0],
                Name = fields[1],
                SpecialRanking = fields.Length > 2 ? fields[2] : null,
            };
        }).Where(team => team != null).ToList();

        // Read the rounds files and calculate team stats
        var roundWithNoScore = new List<string>();
        int numberRounds = Program2.RoundInput();
        var rounds = Enumerable.Range(1, numberRounds);
        foreach (var round in rounds)
        {
            try
            {
                var roundLines = File.ReadAllLines($"round-{round}.csv").Skip(1);
                foreach (var line in roundLines)
                {
                    var fields = line.Split(';');
                    if(fields.Length == 4)
                    {
                        var homeTeam = teams.FirstOrDefault(team => team?.Name == fields[0] || team?.Abbreviation == fields[0]);
                        var awayTeam = teams.FirstOrDefault(team => team?.Name == fields[1] || team?.Abbreviation == fields[1]);
                        if (homeTeam != null && awayTeam != null)
                        {
                            var homeGoals = int.Parse(fields[2]);
                            var awayGoals = int.Parse(fields[3]);
                            homeTeam.GamesPlayed++;
                            awayTeam.GamesPlayed++;
                            homeTeam.GoalsFor += homeGoals;
                            awayTeam.GoalsFor += awayGoals;
                            homeTeam.GoalsAgainst += awayGoals;
                            awayTeam.GoalsAgainst += homeGoals;
                            homeTeam.GoalDifference += homeGoals - awayGoals;
                            awayTeam.GoalDifference += awayGoals - homeGoals;
                            if (homeGoals > awayGoals)
                            {
                                homeTeam.GamesWon++;
                                awayTeam.GamesLost++;
                                homeTeam.Points += 3;
                                homeTeam.LastFiveResults.Enqueue("W");
                                if (homeTeam.LastFiveResults.Count > 5)
                                {
                                    homeTeam.LastFiveResults.Dequeue();
                                }
                                homeTeam.CurrentStreak = string.Join("", homeTeam.LastFiveResults.Reverse());
                                awayTeam.LastFiveResults.Enqueue("L");
                                if (awayTeam.LastFiveResults.Count > 5)
                                {
                                    awayTeam.LastFiveResults.Dequeue();
                                }
                                awayTeam.CurrentStreak = string.Join("", awayTeam.LastFiveResults.Reverse());
                            }
                            else if (homeGoals < awayGoals)
                            {
                                awayTeam.GamesWon++;
                                homeTeam.GamesLost++;
                                awayTeam.Points += 3;
                                awayTeam.LastFiveResults.Enqueue("W");
                                if (awayTeam.LastFiveResults.Count > 5)
                                {
                                    awayTeam.LastFiveResults.Dequeue();
                                }
                                awayTeam.CurrentStreak = string.Join("", awayTeam.LastFiveResults.Reverse());
                                homeTeam.LastFiveResults.Enqueue("L");
                                if (homeTeam.LastFiveResults.Count > 5)
                                {
                                    homeTeam.LastFiveResults.Dequeue();
                                }
                                homeTeam.CurrentStreak = string.Join("", homeTeam.LastFiveResults.Reverse());
                            }
                            else
                            {
                                homeTeam.GamesDrawn++;
                                awayTeam.GamesDrawn++;
                                homeTeam.Points += 1;
                                awayTeam.Points += 1;
                                homeTeam.LastFiveResults.Enqueue("D");
                                if (homeTeam.LastFiveResults.Count > 5)
                                {
                                    homeTeam.LastFiveResults.Dequeue();
                                }
                                homeTeam.CurrentStreak = string.Join("", homeTeam.LastFiveResults.Reverse());
                                awayTeam.LastFiveResults.Enqueue("D");
                                if (awayTeam.LastFiveResults.Count > 5)
                                {
                                    awayTeam.LastFiveResults.Dequeue();
                                }
                                awayTeam.CurrentStreak = string.Join("", awayTeam.LastFiveResults.Reverse());
                            }

                        }
                        else
                        {
                            Console.WriteLine($"One or both of the teams in line {line} are not registered.");
                        }
                    }
                    else
                    {
                        roundWithNoScore.Add(round + ";" + line);
                    }
                    
                }
            }
            catch(FileNotFoundException)
            {
                Console.WriteLine("No rounds after the 32nd");
            }
        }
        File.WriteAllLines($"round-x-file.csv", roundWithNoScore);

        // Sort the teams by points, goal difference, and goals for
        var sortedTeams = teams.OrderByDescending(team => team?.Points)
            .ThenByDescending(team => team?.GoalDifference)
            .ThenByDescending(team => team?.GoalsFor)
            .ThenByDescending(team => team?.GoalsAgainst)
            .ToList();

        foreach (var team in sortedTeams)
        {
            if (!string.IsNullOrEmpty(team?.SpecialRanking))
            {
                var specialRankingTeam = sortedTeams.FirstOrDefault(t => t?.Abbreviation == team.SpecialRanking);
                if (specialRankingTeam != null)
                {
                    sortedTeams.Remove(team);
                    sortedTeams.Insert(sortedTeams.IndexOf(specialRankingTeam) + 1, team);
                }
            }
        }

        // Write the standings file
        Console.WriteLine("{0,-4}{1,-25}{2,-3}{3,-3}{4,-3}{5,-3}{6,-3}{7,-4}{8,-5}{9,-4}{10,-10}", "Pos", "Team", "GP", "W", "L", "D", "GF", "GA", "GD", "Pts", "Streak");
        for (int i = 0; i < sortedTeams.Count; i++)
        {
            var team = sortedTeams[i];

            // Set row color based on index
            if (i == 0)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
            }
            else if (i == 1)
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else if (i >= sortedTeams.Count - 2)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            if(numberRounds < 23)
            {   
                // Write team row
                Console.WriteLine("{0,-4}{1,-25}{2,-3}{3,-3}{4,-3}{5,-3}{6,-3}{7,-4}{8,-5}{9,-4}{10,-10}", i + 1, team?.Name, team?.GamesPlayed, team?.GamesWon, team?.GamesLost, team?.GamesDrawn, team?.GoalsFor, team?.GoalsAgainst, team?.GoalDifference, team?.Points, team?.CurrentStreak);
            }
            else
            {
                if(i < 6)
                {
                    Console.WriteLine("{0,-4}{1,-25}{2,-3}{3,-3}{4,-3}{5,-3}{6,-3}{7,-4}{8,-5}{9,-4}{10,-10}", i + 1, team?.Name, team?.GamesPlayed, team?.GamesWon, team?.GamesLost, team?.GamesDrawn, team?.GoalsFor, team?.GoalsAgainst, team?.GoalDifference, team?.Points, team?.CurrentStreak);
                }
                else
                {
                    if(i == 6)
                    {
                        Console.WriteLine("");
                    }
                    Console.WriteLine("{0,-4}{1,-25}{2,-3}{3,-3}{4,-3}{5,-3}{6,-3}{7,-4}{8,-5}{9,-4}{10,-10}", i - 5, team?.Name, team?.GamesPlayed, team?.GamesWon, team?.GamesLost, team?.GamesDrawn, team?.GoalsFor, team?.GoalsAgainst, team?.GoalDifference, team?.Points, team?.CurrentStreak);
                }
            }
            

            // Reset console color
            Console.ResetColor();
        }
                // Write the standings file
            var standingsLines = new List<string>();
            standingsLines.Add("Team,GP,W,L,D,GF,GA,GD,Pts,Streak");
            foreach (var team in sortedTeams)
            {
                standingsLines.Add($"{team?.Name},{team?.GamesPlayed},{team?.GamesWon},{team?.GamesLost},{team?.GamesDrawn},{team?.GoalsFor},{team?.GoalsAgainst},{team?.GoalDifference},{team?.Points},{team?.CurrentStreak}");
            }
            File.WriteAllLines("standings.csv", standingsLines);

            Console.WriteLine("Standings have been calculated and written to standings.csv.");
    }
}


class Team
{
    public string? Abbreviation { get; set; }
    public string? Name { get; set; }
    public string? SpecialRanking { get; set; }
    public int GamesPlayed { get; set; }
    public int GamesWon { get; set; }
    public int GamesLost { get; set; }
    public int GamesDrawn { get; set; }
    public int GoalsFor { get; set; }
    public int GoalsAgainst { get; set; }
    public int GoalDifference { get; set; }
    public int Points { get; set; }
    public string CurrentStreak { get; set; } = string.Empty;
    public Queue<string> LastFiveResults { get; set; } = new Queue<string>();
}
