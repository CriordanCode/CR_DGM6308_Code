using System;
using System.Collections.Generic;

class Program
  {
    public static void Main(string[] args)
    {
        JokeManager manager = new JokeManager();
        UserInterface ui = new UserInterface(manager);
        ui.Start();
    }
  }

public class JokeManager
{

    List<string> jokeList;

    public JokeManager()
    {
        jokeList = new List<string>();
    }    

    public void AddJoke(string joke)
    {
        jokeList.Add(joke);
        return;
    }

    public string DrawJoke()
    {
        if(jokeList.Count == 0)
        {
            return "Jokes are in short supply";
        }
        else
        {
            int index = Random.Shared.Next(0, jokeList.Count);
            return jokeList[index];
        }
    }

    public void PrintJokes()
    {
        foreach(string joke in jokeList)
        {
            Console.WriteLine(joke);
        }
    }
}

public class UserInterface
{
    JokeManager manager;

    public UserInterface(JokeManager joker)
    {
        manager = joker;
    }

    public void Start()
    {
        while (true)
        {
            Console.WriteLine("Commands:");
            Console.WriteLine("1 - Add a joke to the list.");
            Console.WriteLine("2 - Draw a random joke from the list.");
            Console.WriteLine("3 - Print all the jokes in the list.");
            Console.WriteLine("X - Exit the menu.");


            string input = Console.ReadLine();
            
            if(input == "1")
            {
               Console.WriteLine("Please enter your joke:");
                string newJoke = Console.ReadLine();
                manager.AddJoke(newJoke); 
            }
            else if(input == "2")
            {
                Console.WriteLine("Drawing a random joke:");
                Console.WriteLine(manager.DrawJoke());
            }
            else if(input == "3")
            {
                Console.WriteLine("Printing Jokes:");
                manager.PrintJokes();
            }
            else if(input == "X")
            {
                break;
            }
            
        }
    }
}