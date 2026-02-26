using System.Text;
bool closed = false;

try
{
    StringBuilder welcomePrintout = new StringBuilder();
    welcomePrintout.AppendLine();
    welcomePrintout.AppendLine("Welcome to War");
    welcomePrintout.AppendLine();
    welcomePrintout.AppendLine("In this game you will split a normal 52 card deck");
    welcomePrintout.AppendLine("with the computer and play against them. Drawing one");
    welcomePrintout.AppendLine("card at a time, the two of you will compare cards");
    welcomePrintout.AppendLine("with the Highcard winning that round. Continue until");
    welcomePrintout.AppendLine("all cards have been played, the player with the most");
    welcomePrintout.AppendLine("rounds won will win the game!");
    welcomePrintout.AppendLine();
    welcomePrintout.AppendLine();
    welcomePrintout.AppendLine();
    welcomePrintout.AppendLine("Press [enter] to continue...");
    Console.WriteLine(welcomePrintout);
    Console.ReadKey(true);
    Console.Clear();
    Console.WriteLine("Press Enter to Draw...");

    while (!closed)
    {
        Game game = new Game();
        game.Shuffle();
        while(game.playerOneCards.Count > 0 && game.playerOneScore < 14 && game.playerTwoScore < 14)
        {
            
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.Enter: Console.Clear(); game.Draw(); break;
                default: Console.Clear(); Console.WriteLine("Press enter to draw..."); break; 
            }
            Console.WriteLine("Press Enter To Draw Again...");
            Console.WriteLine();
            Console.WriteLine($"Player One Score: {game.playerOneScore}");
            Console.WriteLine($"Player Two Score: {game.playerTwoScore}");
        }
        switch (game.playerOneScore.CompareTo(game.playerTwoScore))
        {
                case > 0: Console.WriteLine("Player One Wins the Game!"); break;
                case < 0: Console.WriteLine("Player Two Wins The Game!"); break;
                case   0: Console.WriteLine("Its a tie!"); break;
        }
        Console.WriteLine();
        Console.WriteLine("Press Enter to Play Again!");
        Console.ReadKey(true);
        Console.Clear();
    }
}
finally
{
    
}

public class Game
{
    public List<Card> deckCards;
    public List<Card> playerOneCards;
    public List<Card> playerTwoCards;
    public int playerOneScore;
    public int playerTwoScore;
    public Game()
    {
        deckCards = new List<Card>();
        playerOneCards = new List<Card>();
        playerTwoCards = new List<Card>();
        playerOneScore = 0;
        playerTwoScore = 0;

        while(deckCards.Count != 52)
        {
            for(int i = 2; i <= 14; i++)
            {
                for(int j = 0; j < 4; j++)
                {
                    deckCards.Add(new Card(j, i));
                }
            }
        }
    }

    public void Shuffle()
    {
        while(playerOneCards.Count < 26)
        {
            playerOneCards.Add(deckCards[Random.Shared.Next(0,deckCards.Count)]);
            deckCards.Remove(playerOneCards[^1]);
        }
        while(deckCards.Count > 0)
        {
            playerTwoCards.Add(deckCards[Random.Shared.Next(0,deckCards.Count)]);
            deckCards.Remove(playerTwoCards[^1]);
        }
    }

    public void Draw()
    {
        List<string> cardReadout = new List<string>();
        for(int i = 0; i < 5; i++)
        {
            cardReadout.Add("");
        }
        playerOneCards[0].PrintCard(cardReadout);
        AddBlank(cardReadout);
        playerTwoCards[0].PrintCard(cardReadout);
        for(int i = 0; i < 5; i++)
        {
            Console.WriteLine(cardReadout[i]);
        }
        if(playerOneCards[0].value > playerTwoCards[0].value)
        {
            playerOneScore++;
            Console.WriteLine("Player One wins the round.");
        }
        else if (playerOneCards[0].value < playerTwoCards[0].value)
        {
            playerTwoScore++;
            Console.WriteLine("Player Two wins the round.");
        }
        else
        {
            Console.WriteLine("Cards are Equal. It's a Draw!");
        }
        playerOneCards.RemoveAt(0);
        playerTwoCards.RemoveAt(0);
    }
    public void AddBlank(List<String> display)
    {
        for(int i = 0; i < display.Count; i++)
        {
            display[i] += "   ";
        }
    }
}

public class Card
{
    public int value { get; }
    public int suit { get; }

    public Card (int suitInput, int valueInput)
    {
        suit = suitInput;
        value = valueInput;
    }

    public void PrintCard(List<string> display)
    {
        display[0] +=  "╔═════╗";
        display[1] += $"║{ValueToString()}░░░║";
        display[2] +=  "║░░░░░║";
        display[3] += $"║░░░{ValueToString()}║";
        display[4] +=  "╚═════╝";
    }

    public string ValueToString()
    {
        switch (value)
        {
            case < 10 : return ($"0" + value);
            case 10 : return "10";
            case 11 : return " J";
            case 12 : return " Q";
            case 13 : return " K";
            case 14 : return " A";
            default: return "Value Not Supported";
        }
    }
}