using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

string[] frames = ["▌", "▌"];
string[] deathFrames = ["X", "X", "X", "X", "X"];
List<Note> notes;
List<Note> deadNotes;
TimeSpan delayTime = TimeSpan.FromMilliseconds(34);
TimeSpan spawnTimeMin = TimeSpan.FromMilliseconds(250);
TimeSpan spawnTimeMax = TimeSpan.FromMilliseconds(2000);
//Stopwatch to help track current time during the game
Stopwatch totalTime = new Stopwatch();
int targetLeft = 5;
int remainingMisses = 5;
(int Top, ConsoleKey Key)[] tracks =
[
    (4, ConsoleKey.UpArrow),
    (7, ConsoleKey.LeftArrow),
    (10, ConsoleKey.DownArrow),
    (13, ConsoleKey.RightArrow)
];

try
{
    int bufferWidth = Console.BufferWidth;
    Console.CursorVisible = false;
    DateTime lastSpawn;
    TimeSpan spawnTime;
    
    int score;
    int misses;
    //Variables to track streak and multiplier
    int currentStreak = 0;
    int streakMult = 1;

    void NewNote()
	{
		notes.Add(new Note()
		{
			Top = tracks[Random.Shared.Next(tracks.Length)].Top,
			Frame = 0,
			Left = Console.BufferWidth - 1,
		});
		lastSpawn = DateTime.Now;
		//spawnTime = TimeSpan.FromMilliseconds(Random.Shared.Next((int)spawnTimeMin.TotalMilliseconds, (int)spawnTimeMax.TotalMilliseconds));
        //Code to make the spawntime adjust based on the elapsed time going down 1 millisecond for everyone 100 lasted
        spawnTimeMax = TimeSpan.FromMilliseconds(2000 - (totalTime.ElapsedMilliseconds/100));
        if (spawnTimeMin > spawnTimeMax)
        {
            spawnTime = spawnTimeMin;
        } else
        {
            spawnTime = spawnTimeMax;
        }
	}

    void RenderMisses()
    {
        Console.SetCursorPosition(0, tracks[^1].Top + 5);
        Console.WriteLine("Remaining Misses: " + (remainingMisses - misses));
    }

    //Small method to write out the score
    void RenderScore()
    {
        Console.SetCursorPosition(0, tracks[^1].Top + 3);
        Console.WriteLine("Current Score: " + score);
    }
    
    //Similar method to write out combo streak and multiplier
    void RenderCombo()
    {
        Console.SetCursorPosition(0, tracks[^1].Top + 4);
        Console.WriteLine("Current Combo: " + currentStreak + "   Giving a " + streakMult + "x Multiplier!");
    }

    //Display time for the player and update it as frequently as called
    void RenderTime()
    {
        //Console.SetCursorPosition(0, tracks[^1].Top + 2);
        Console.SetCursorPosition(bufferWidth/2, 0);
        TimeSpan timePrint = totalTime.Elapsed;
        string elapsedTime = String.Format("{0:00}:{1:00}.{2:00}",
            timePrint.Minutes, timePrint.Seconds, timePrint.Milliseconds / 10);
        Console.WriteLine("Current Run: " + elapsedTime);
    }

PlayAgain:
    misses = 0;
    score = 0;
    notes = new List<Note>();
    deadNotes = new List<Note>();
    Console.Clear();
    Console.WriteLine("Rhythm");
    Console.WriteLine();
    Console.WriteLine("Press enter to play...");
    {
    GetInput:
        switch (Console.ReadKey(true).Key)
        {
            case ConsoleKey.Enter: break;
            case ConsoleKey.Escape:
                Console.Clear();
                Console.WriteLine("Rhythm Closed.");
                return;
            default: goto GetInput;
        }
    }
    totalTime.Reset();
    totalTime.Start();
    Console.Clear();
    Console.WriteLine("Rhythm");
    Console.WriteLine();
    Console.WriteLine("Time your button presses...");
    foreach (var (Top, Key) in tracks)
    {
        Console.SetCursorPosition(0, Top - 1);
        Console.WriteLine(new string('_', Console.BufferWidth));
        Console.SetCursorPosition(targetLeft, Top + 1);
        Console.Write($"^ {Key}");
    }
    
    RenderMisses();
    NewNote();
    while (true)
    {
    NextKey:
        //Render the time on the screen during play
        RenderTime();
        //Constantly Render the current score while game is active
        RenderScore();
        RenderCombo();
        while (Console.KeyAvailable)
        {
            ConsoleKey key = Console.ReadKey().Key;
            if (key is ConsoleKey.Escape)
            {
                Console.Clear();
                Console.WriteLine("Rhythm Closed.");
                return;
            }
            foreach (var (Top, Key) in tracks)
            {
                if (key == Key)
                {
                    foreach (Note note in notes)
                    {
                        if(note.Left > targetLeft + 2)
                        {
                            //On Miss, reset streak and streak multiplier
                            currentStreak = 0;
                            streakMult = 1;
                            break;
                        }
                        if (note.Top == Top && Math.Abs(note.Left - targetLeft) < 3)
                        {
                            notes.Remove(note);
                            note.Frame = deathFrames.Length - 1;
                            deadNotes.Add(note);
                            //Add to the current streak
                            currentStreak++;
                            //Cases for current streak being over 10 to make the mult increase by 1 every 10 notes hit
                            if (currentStreak >= 10)
                            {
                                streakMult = (currentStreak / 10) + 1;
                            }
                            //Changed score to increment by the streak multiplier
                            score += (1 * streakMult);
                            
                            goto NextKey;
                        }
                    }
                    if (++misses >= remainingMisses)
                    {
                        goto GameOver;
                    }
                RenderMisses();
                break;
            }
        }
    }
    if (bufferWidth != Console.BufferWidth)
    {
        Console.Clear();
        Console.Write("Rhythm Closed. Console was resized.");
        return;
    }
    if (notes[0].Left <= 0)
    {
        Console.SetCursorPosition(0, notes[0].Top);
        Console.Write(" ");
        notes.RemoveAt(0);
        if (++misses >= remainingMisses)
        {
            goto GameOver;
        }
        RenderMisses();
    }
    for (int i = 0; i < deadNotes.Count; i++)
    {
        if (deadNotes[i].Frame < 0){
            Console.SetCursorPosition(deadNotes[i].Left, deadNotes[i].Top);
            Console.Write(" ");
            deadNotes.RemoveAt(i--);
        }
        else
        {
            Console.SetCursorPosition(deadNotes[i].Left, deadNotes[i].Top);
            Console.Write(deathFrames[deadNotes[i].Frame]);
            deadNotes[i].Frame--;
        }
    }
    foreach (Note note in notes)
    {
        note.Frame--;
        if (note.Frame < 0)
        {
            Console.SetCursorPosition(note.Left, note.Top);
            Console.Write(" ");
            note.Frame = frames.Length - 1;
            note.Left--;
        }
        Console.SetCursorPosition(note.Left, note.Top);
        Console.Write(frames[note.Frame]);
    }
    if (DateTime.Now - lastSpawn > spawnTime)
    {
        NewNote();
    }
    Thread.Sleep(delayTime);
}
GameOver:
    
    Console.Clear();
    Console.WriteLine("Rhythm");
    Console.WriteLine();
    Console.WriteLine("Score: " + score);
    Console.WriteLine();
    //Code to print out the end time that the player lasted
    totalTime.Stop();
    string endTime = String.Format("You lasted : {0:00}:{1:00}.{2:00}", 
        totalTime.Elapsed.Minutes, totalTime.Elapsed.Seconds, totalTime.Elapsed.Milliseconds);
    Console.WriteLine(endTime);
    Console.WriteLine();
    Console.WriteLine("Play Again [enter], or quit [escape]?");
    {
    GetInput:
        switch (Console.ReadKey(true).Key)
        {
            case ConsoleKey.Enter: goto PlayAgain;
            case ConsoleKey.Escape:
                Console.Clear();
                Console.WriteLine("Rhythm Closed.");
                return;
            default: goto GetInput;
        }
    }
}
finally
{
    Console.CursorVisible = true;    
}


public class Note
{
    public int Top;
    public int Left;
    public int Frame;

}