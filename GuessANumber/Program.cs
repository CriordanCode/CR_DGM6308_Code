using System;

//Variables to set the limit for the chosen number and the chosen number itself
int maxValue = 100;

//Betting Variables
int initMoney = 100;
int currentMoney = initMoney;
int betAmt = 0;
int betMult = 2;

MakeABet:
    while (true)
    {
        Console.WriteLine("You have $" + currentMoney + " to bet with.");
        Console.WriteLine("Please enter your current wager:");
        bool validBet = int.TryParse((Console.ReadLine() ?? "").Trim(), out betAmt);
        if(validBet)
        {
            if(betAmt > currentMoney)
            {
                Console.WriteLine("You do not have enough to bet that amount, try again.");
            } else
            {
                currentMoney -= betAmt;
                //Set Valid Ceiling based on the bet amount
                maxValue = betAmt * 10;
                break;
            }
        }
        else Console.WriteLine("Invalid Bet Amount");
    }

//Code for Requirement B that is made obsolete by Requirement D
//While loop to get a valid ceiling for the random number selector
// while(true){
//     Console.WriteLine("Set the max range for the Computer to choose from:");
//     bool validRange = int.TryParse((Console.ReadLine() ?? "").Trim(), out maxValue);
//     if (validRange) break;
//     else Console.WriteLine("Invalid maximum value given");
// }


//Choose the Value based on an alternated range
int value = Random.Shared.Next(1, maxValue+1);

//Variables to set the total number of guesses and count the remaining guesses
int maxGuesses = maxValue/20;
int guessesRemaining = maxGuesses;

//Variable to check if the player has won
bool hasWon = false;

//Variables for more info to give (within 33%, within 20%, within 10%, within 5%)
int cold = maxValue/3;
int warm = maxValue/5;
int hot = maxValue/10;
int hotter = maxValue/20;



while (true)
{
    //Code to prompt the user for their guess and letting them know what their remaining guesses are
    Console.WriteLine("Guess a Number (1-" + maxValue + ") in " + guessesRemaining + " tries: ");
    
    //Check the input for if it was a valid number
    bool valid = int.TryParse((Console.ReadLine() ?? "").Trim(), out int input);
    
    //Check case for what the input is against the chosen value 
    if (!valid) Console.WriteLine("Invalid.");
    else if (input == value) {
        hasWon = true;
        break;
    }
    else if (input != value && guessesRemaining > 0){
        Console.WriteLine($"Incorrect. Too {(input < value ? "Low" : "High")}.");
        
        //Code to figure out how far away the guess is
        int distAway = Math.Abs(value - input);
        
        //Cases to determine what extra information to give the player
        if (distAway < hotter)
        {
            Console.WriteLine("Your guess was SUPER HOT!");
        } else if (distAway < hot)
        {
            Console.WriteLine("Your guess was Hotter!");
        } else if (distAway < warm)
        {
            Console.WriteLine("Your guess was Hot!");
        } else if (distAway < cold)
        {
            Console.WriteLine("Your guess was Warm.");  
        } else
        {
            Console.WriteLine("Your guess was Cold.");
        }

        

        //Subtract one from the guesses and end the loop if that was the last guess
        guessesRemaining--;
        if(guessesRemaining == 0)
        {
            break;
        }
    }
    else break;
}

//Code to check if the user has won after the loop was broken, if not prompt them with a loss
if (hasWon == true){
    Console.WriteLine("You Guessed It!.");
    currentMoney += (betAmt * betMult);
}
else
{
    Console.WriteLine("You Lose!");
}
if(currentMoney <= 0)
{
    Console.WriteLine("You have no more money to bet with. Better luck next time.");
} else if(currentMoney >= (initMoney * 2))
{
    Console.WriteLine("The house has no more money left. You Win!");
    Console.WriteLine("You ended with $" + currentMoney);
} else
{
    goto MakeABet;
}

Console.Write("Press any key to exit...");
Console.ReadKey(true);