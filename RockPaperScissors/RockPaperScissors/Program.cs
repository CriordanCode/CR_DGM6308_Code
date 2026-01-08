using System;
using static Move;

int wins = 0;
int draws = 0;
int losses = 0;

while (true)
{
    Console.Clear();
    Console.WriteLine("Water, Grass, Fire, Air");
    Console.WriteLine();

    GetInput:
        Console.Write("Choose [W]ater, [G]rass, or [F]ire, or [A]ir, or [E]xit:");
        Move playerMove;
        switch ((Console.ReadLine() ?? "").Trim().ToLower())
    {
        case "w" or "water": playerMove = Water; break;
        case "g" or "grass": playerMove = Grass; break;
        case "f" or "fire": playerMove = Fire; break;
        case "a" or "air": playerMove = Air; break;
        case "e" or "exit": Console.Clear(); return;
        default: Console.WriteLine("Invalid Input. Try Again..."); goto GetInput;
    }
    
    Move computerMove = (Move)Random.Shared.Next(4);
    Console.WriteLine($"The Computer Chose {computerMove}.");
    
    switch (playerMove, computerMove)
    {
        case (Water, Grass) or (Grass, Fire) or (Fire, Water) or (Fire, Air):
            Console.WriteLine("You Lose.");
            losses++;
            break;
        case (Water, Fire) or (Grass, Water) or (Fire, Grass) or (Air, Fire):
            Console.WriteLine("You Win.");
            wins++;
            break;
        default:
            Console.WriteLine("This game was a draw.");
            draws++;
            break;
    }
    
    Console.WriteLine($"Score: {wins} wins, {losses} losses, {draws} draws");
    Console.WriteLine("Press Enter To Continue...");
    Console.ReadLine();
}

enum Move
{
    Water = 0,
    Grass = 1,
    Fire = 2,
    Air = 3,
}

