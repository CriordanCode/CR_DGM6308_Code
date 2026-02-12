using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

Exception? exception = null;

try
{
    
    //Declare the location for where the resource file is
    const string wordsResource = "Shared.FiveLetterWords.txt";
    //Create an assembly
    Assembly assembly = Assembly.GetExecutingAssembly();
    Console.WriteLine(assembly.GetManifestResourceInfo);
    List<string> words = new();

    {
        foreach (string x in assembly.GetManifestResourceNames())
        {
            Console.WriteLine($"name: {x}");
            Console.WriteLine(x);
            
        }
        Console.ReadLine();
        using Stream stream = assembly.GetManifestResourceStream("Shared.FiveLetterWords.txt")!;
        if (stream is null)
        {
            Console.WriteLine("Error: Missing \"FiveLetterWords.txt\" embedded resource.");
            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();
            return;
        }
        //Creates a stream to read through the word file
        using StreamReader streamReader = new(stream);
        //Until the stream gets to the end of the file add each line to the words list
        while (!streamReader.EndOfStream)
        {
            string line = streamReader.ReadLine()!;
            words.Add(line.Trim().ToUpperInvariant());
        }
    }

    //Label for the start of game & ASCII code to setup the board
    PlayAgain:
        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.Black;
        Console.Clear();
        Console.WriteLine("""
		    Wordle
		    ╔═══╦═══╦═══╦═══╦═══╗
		    ║   ║   ║   ║   ║   ║
		    ╠═══╬═══╬═══╬═══╬═══╣
		    ║   ║   ║   ║   ║   ║
		    ╠═══╬═══╬═══╬═══╬═══╣
		    ║   ║   ║   ║   ║   ║
		    ╠═══╬═══╬═══╬═══╬═══╣
		    ║   ║   ║   ║   ║   ║
		    ╠═══╬═══╬═══╬═══╬═══╣
		    ║   ║   ║   ║   ║   ║
		    ╠═══╬═══╬═══╬═══╬═══╣
		    ║   ║   ║   ║   ║   ║
		    ╚═══╩═══╩═══╩═══╩═══╝
		    Controls:
		    - a b, c, ... y, z: input letters
		    - left/right arrow: move cursor
		    - enter: submit or confirm
		    - escape: exit
		    """);
        int guess = 0;
        int cursor = 0;
        //Get a random word from the list of words and convert to uppercase
        string word = words[Random.Shared.Next(words.Count)];
        char[] letters = [' ', ' ', ' ', ' ', ' ', ' '];
    //Label to jump to getting input from the user
    GetInput:
        //Math to set the position of the cursor based on the X and Y grid
        Console.SetCursorPosition(3 + cursor * 4, 2 + guess * 2);
        ConsoleKey key = Console.ReadKey(true).Key;
        switch (key)
        {
            //Case if the key press is within the bounds of Key A and Key Z
            case >= ConsoleKey.A and <= ConsoleKey.Z:
                ClearMessageText();
                Console.SetCursorPosition(3 + cursor * 4, 2 + guess * 2);
                char c = (char)(key - ConsoleKey.A + 'A');
                letters[cursor] = c;
                Console.Write(c);
                cursor = Math.Min(cursor + 1, 4);
                goto GetInput;
            //Case if the key press is the left arrow to move the cursor to the left
            case ConsoleKey.LeftArrow:
                cursor = Math.Max(cursor - 1, 0);
                goto GetInput;
            //Case if the key press is the right arrow to move the cursor to the right
            case ConsoleKey.RightArrow:
                cursor = Math.Max(cursor + 1, 4);
                goto GetInput;
            //Case if the key pressed is enter signalling the user is making a guess
            case ConsoleKey.Enter:
                //Check to make sure the word is a valid word
                if (letters.Any(l => l < 'A' || l > 'Z') || !words.Contains(new string(letters)))
                {
                    ClearMessageText();
                    Console.SetCursorPosition(0,19);
                    Console.WriteLine(" You must input a valid word.");
                    goto GetInput;
                }
                bool correct = true;
                //Go through and set colors for the guesses based on how correct the user is
                for (int i = 0; i < 5; i++)
                {
                    Console.SetCursorPosition(2 + i * 4, 2 + guess * 2);
                    if (word[i] == letters[i])
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                    }
                    else if (CheckForYellow(i, word, letters))
                    {
                        correct = false;
                        Console.BackgroundColor = ConsoleColor.DarkYellow;
                    }
                    else
                    {
                        correct = false;
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                    }
                    Console.Write($" {letters[i]} ");
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                //If the user's guess is the right word give them a win screen
                if (correct)
                {
                    ClearMessageText();
                    Console.SetCursorPosition(0, 19);
                    Console.WriteLine(" You Win!");
                    if (PlayAgainCheck())
                    {
                        goto PlayAgain;
                    }
                    else
                    {
                        return;
                    }
                }
                //Guess is wrong so set new guess
                else
                {
                    letters = [' ', ' ', ' ', ' ', ' '];
                    guess++;
                    cursor = 0;
                }
                //If they have guessed 5 times trigger a loss
                if (guess > 5)
                {
                    ClearMessageText();
                    Console.SetCursorPosition(0, 19);
                    Console.WriteLine($" You lose! Word: {word}");
                    if (PlayAgainCheck())
                    {
                        goto PlayAgain;    
                    }
                    else
                    {
                        return;
                    }
                }
                goto GetInput;
            //Case for escape key press to end game
            case ConsoleKey.Escape:
                return;
            //Case for End and Home keys to play again
            case ConsoleKey.End or ConsoleKey.Home:
                goto PlayAgain;
            //Default returns for extra input if the key is not one of the specified cases
            default:
                goto GetInput;
        }


}
//Catch excpetions incase anything goes wrong
catch (Exception e)
{
    exception = e;
    throw;
}
//At the end run this bit of code
finally
{
    Console.ResetColor();
    Console.Clear();
    Console.WriteLine(exception?.ToString() ?? "Wordle was closed.");
}

//Helper method to check if a letter is yellow meaning it is somewhere in the word but not where it was guessed
bool CheckForYellow(int index, string word, char[] letters)
{
    int letterCount = 0;
    int incorrectCountBeforeIndex = 0;
    int correctCount = 0;
    for (int i = 0; i < word.Length; i++)
    {
        if (word[i] == letters[index])
        {
            letterCount++;
        }
        if (letters[i] == letters[index] && word[i] == letters[index])
        {
            correctCount++;
        }
        if (i < index && letters[i] == letters[index] && word[i] != letters[index])
        {
            incorrectCountBeforeIndex++;
        }
    }
    return letterCount - correctCount - incorrectCountBeforeIndex > 0;
}

//Helper method to check if the player wants to play again
bool PlayAgainCheck()
{
    Console.WriteLine($" Play again [enter] or quite [escape]?");
GetPlayAgainInput:
    switch (Console.ReadKey(true).Key)
    {
        case ConsoleKey.Enter:
            return true;
        case ConsoleKey.Escape:
            return false;
        default:
            goto GetPlayAgainInput;
    }
}

//Helper method to clear the lines
void ClearMessageText()
{
    Console.SetCursorPosition(0, 19);
    Console.WriteLine("                                         ");
    Console.WriteLine("                                         ");
}