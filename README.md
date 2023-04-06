Solution:

The program reads data from CSV files representing football rounds and teams, and calculates team statistics based on the results of the rounds. The program then sorts the teams based on their points, goal difference, goals for, and goals against.

Usage:

The program requires two CSV files to be present in the same directory where the program is executed: "teams.csv": a file containing team data with each line representing a team and separated by semicolons (;). The fields in each line should be in the following order: abbreviation, name, and special ranking (optional). The first line is skipped as it is assumed to be a header. "round-X.csv" (where X is the round number): a file containing the results of a specific round of matches, with each line representing a match and separated by semicolons (;). The fields in each line should be in the following order: home team, away team, home goals, and away goals. If the home and away teams have the same abbreviation or name as specified in the "teams.csv" file, the program will update the team statistics accordingly. Compile and run the program in a C# environment.

Follow the prompts to enter the number of rounds (1-32) you want to process. The program will then read the corresponding "round-X.csv" files and calculate the team statistics.

The program will generate an output file named "round-x-file.csv" (where X is the round number) containing the lines from the "round-X.csv" file that did not have scores (i.e., less than four fields).

Rule/Error Help Information:

The program checks if the number of rounds entered by the user is between 1 and 32. If an invalid value is entered, an exception is thrown, and the user is prompted to enter a valid value.

The program checks if the "round-X.csv" file exists for each round from 1 to the entered number of rounds. If a file is not found, a "FileNotFoundException" is caught, and an error message is displayed.

The program checks if the "teams.csv" file and "round-X.csv" files have the correct format (i.e., the correct number of fields separated by semicolons). If a line in the files does not have the correct format, the program skips that line and continues processing the next line.

The program checks if the home and away teams in each match have a valid abbreviation or name as specified in the "teams.csv" file. If a team is not found, an error message is displayed.

The program calculates team statistics, such as games played, games won, games lost, games drawn, points, goals for, goals against, and goal difference, based on the results of the rounds. If the results of a round do not match the expected format (i.e., four fields representing home team, away team, home goals, and away goals), the program skips that line and continues processing the next line.

The program sorts the teams based on their points, goal difference, goals for, and goals against. If two or more teams have the same points, goal difference, goals for, and goals against, the program does not provide any special ranking based on team names or abbreviations.
