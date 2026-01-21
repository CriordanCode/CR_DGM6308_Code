using System;
using System.Diagnostics;

Exception? exception = null;

const string menu = """

        Quick Draw
        
        Face your opponent and wait for the signal. Once the
        signal is given, shoot your opponent by pressing [Space]
        before they shoot you. If you choose to block by
        pressing [B] beforehand you can survive the shot. If you
        block *too soon* they will block as well and guarantee
        their next shot hits. It's all about your reaction time.
        
        Choose Your Opponent:
        [1] Easy....1000 milliseconds
        [2] Medium...500 milliseconds
        [3] Hard.....250 milliseconds
        [4] Harder...125 milliseconds
        [escape] give up
        """;

const string wait = """

	  Quick Draw
	                                                        
	              _O                          O_            
	             |/|_          wait          _|\|           
	             /\                            /\           
	            /  |                          |  \          
	  ------------------------------------------------------
	""";

const string fire = """

	  Quick Draw
	                                                        
	                         ********                       
	                         * FIRE *                       
	              _O         ********         O_            
	             |/|_                        _|\|           
	             /\          spacebar          /\           
	            /  |                          |  \          
	  ------------------------------------------------------
	""";

const string loseTooSlow = """

	  Quick Draw
	                                                        
	                                                        
	                                                        
	                                        > ╗__O          
	           //            Too Slow           / \         
	          O/__/\         You Lose          /\           
	               \                          |  \          
	  ------------------------------------------------------
	""";

const string loseTooFast = """

	  Quick Draw
	                                                        
	                                                        
	                                                        
	                         Too Fast       > ╗__O          
	           //           You Missed          / \         
	          O/__/\         You Lose          /\           
	               \                          |  \          
	  ------------------------------------------------------
	""";

const string win = """

	  Quick Draw
	                                                        
	                                                        
	                                                        
	            O__╔ <                                      
	           / \                               \\         
	             /\          You Win          /\__\O        
	            /  |                          /             
	  ------------------------------------------------------
	""";


//New ASCII drawings for the new scenario cases that can play out.

const string blockWin = """

	  Quick Draw
	                                                        
	                                                        
	                                                        
	              _O |      You Chose                            
	             |/|_|        Block                 \\          
	             /\  |       You Win             /\__\O           
	            /  | |                          /             
	  ------------------------------------------------------
	""";

const string doubleBlock = """

	  Quick Draw
	                                                        
	                                                        
	                                               
	              _O          You Both     > ╗ O_            
	             |/|_|         Block         |_|\|           
	             /\  |        You Lose       |  /\           
	            /  | |                       | |  \              
	  ------------------------------------------------------
	""";

try
{
    while (true)
    {
        Console.Clear();
        Console.WriteLine(menu);
        TimeSpan? requiredReactionTime = null;
        //Code to check for which difficulty has been selected
        while (requiredReactionTime is null)
        {
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.D1 or ConsoleKey.NumPad1: requiredReactionTime = TimeSpan.FromMilliseconds(1000); break;
                case ConsoleKey.D2 or ConsoleKey.NumPad2: requiredReactionTime = TimeSpan.FromMilliseconds(500); break;
                case ConsoleKey.D3 or ConsoleKey.NumPad3: requiredReactionTime = TimeSpan.FromMilliseconds(250); break;
                case ConsoleKey.D4 or ConsoleKey.NumPad4: requiredReactionTime = TimeSpan.FromMilliseconds(125); break;
            }
        }
        Console.Clear();
        //Set the random time when it will signal to shoot
        TimeSpan signal = TimeSpan.FromMilliseconds(Random.Shared.Next(4000, 10000));
        Console.WriteLine(wait);
        Stopwatch stopwatch = new();
        stopwatch.Restart();
        bool tooFast = false;
        //Value to hold if they blocked or not
        bool hasBlocked = false;
        TimeSpan blockTimer = default;
        while (stopwatch.Elapsed < signal && (!tooFast || !hasBlocked))
        {
            if (Console.KeyAvailable && Console.ReadKey(true).Key is ConsoleKey.Spacebar)
            {
                tooFast = true;
            }
            //Check to see if the block key is pressed and get the time at which it is pressed
            if (Console.KeyAvailable && Console.ReadKey(true).Key is ConsoleKey.B)
            {
                hasBlocked = true;
                blockTimer = stopwatch.Elapsed;
            }
        }

        //Check to see if the player chose to block too soon (4 seconds before the draw happens)
        bool compBlocks = false;
        if (signal - blockTimer > TimeSpan.FromMilliseconds(4000) && hasBlocked)
        {
            compBlocks = true;
        }


        Console.Clear();
        Console.CursorVisible = false;
        Console.WriteLine(fire);
        stopwatch.Restart();
        bool tooSlow = true;
        TimeSpan reactionTime = default;
        while (!tooFast && stopwatch.Elapsed < requiredReactionTime && tooSlow)
        {
            if (Console.KeyAvailable && Console.ReadKey(true).Key is ConsoleKey.Spacebar)
            {
                tooSlow = false;
                reactionTime = stopwatch.Elapsed;
            }
        }
        //If they blocked make sure the tooSlow or tooFast cases don't trigger
        if(hasBlocked)
        {
            tooSlow = false;
            tooFast = false;
        }

        Console.Clear();
        Console.WriteLine(
            tooFast ? loseTooFast :
            tooSlow ? loseTooSlow :
            //2 Cases for if they blocked, happen in the order of loss then win since both variables can be true at the same time it will pick
            //the loss case first if that is also true
            compBlocks ? doubleBlock :
            hasBlocked ? blockWin :
            $"{win}{Environment.NewLine} Reaction Time: {reactionTime.TotalMilliseconds} milliseconds");
        Console.WriteLine("     Play Again [enter] or quit [escape]?");
        Console.CursorVisible = false;


    GetEnterOrEscape:
        switch (Console.ReadKey(true).Key)
        {
            case ConsoleKey.Enter: break;
            case ConsoleKey.Escape: return;
            default: goto GetEnterOrEscape;
        }
    }
}

catch (Exception e)
{
    exception = e;
    throw;
}

finally
{
    Console.Clear();
    Console.CursorVisible = true;
    Console.WriteLine(exception?.ToString() ?? "Quick Draw was closed.");
}


