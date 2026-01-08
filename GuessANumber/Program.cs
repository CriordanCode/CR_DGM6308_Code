using System;

int value = Random.Shared.Next(1, 101);
int maxGuesses = 10;
int guessesRemaining = maxGuesses;
while (true)
{
    Console.WriteLine("Guess a Number (1-100) in 10 tries: ");
    bool valid = int.TryParse((Console.ReadLine() ?? "").Trim(), out int input);
    
    if (!valid) Console.WriteLine("Invalid.");
    else if (input == value) break;
    else if (input != value && guessesRemaing{
        Console.WriteLine($"Incorrect. Too {(input < value ? "Low" : "High")}.");
        guessesRemaining--;
    }
}

Console.WriteLine("You Guessed It!.");
Console.Write("Press any key to exit...");
Console.ReadKey(true);