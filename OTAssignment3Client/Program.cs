using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using OTAssignment3Client.ServiceReference1;
using WordGame;
using Word = OTAssignment3Client.ServiceReference1;
using System.ServiceModel;

namespace OTAssignment3Client
{
    class Program
    {
        static void Main(string[] args)
        {
            WordGameClient proxy = new WordGameClient();

            bool canPlayGame = true;

            Console.WriteLine("Player's name?");
            String playerName = Console.ReadLine();

            if (!proxy.isGameBeingHosted())
            {
                Console.WriteLine("Welcome " + playerName + "! Do you want to host the game?");
                if (Console.ReadLine().CompareTo("yes") == 0)
                {
                    Console.WriteLine("Type the word to scramble.");
                    string inputWord = Console.ReadLine();
                    try
                    {
                        string scrambledWord = proxy.hostGame(playerName, inputWord);
                        canPlayGame = false;
                        Console.WriteLine("You're hosting the game with word '" + inputWord + "' scrambled as '" + scrambledWord + "'");
                        Console.ReadKey();
                    }
                    catch (FaultException<GameBeingHostedException> e)
                    {
                        Console.WriteLine(e.Detail.reason);
                    }

                }
            }
            if (canPlayGame)
            {
                Console.WriteLine("Do you want to play the game?");
                if (Console.ReadLine().CompareTo("yes") == 0)
                {
                    WordGame.Word gameWords;
                    try
                    {
                        gameWords = proxy.join(playerName);
                        Console.WriteLine("Can you unscramble this word? => " + gameWords.scrambledWord);
                        String guessedWord;
                        bool gameOver = false;
                        while (!gameOver)
                        {
                            guessedWord = Console.ReadLine();
                            gameOver = proxy.guessWord(playerName, guessedWord, gameWords.unscrambledWord);
                            if (!gameOver)
                            {
                                Console.WriteLine("Nope, try again...");
                            }
                        }
                        Console.WriteLine("YOU WON!!!");
                        proxy.Close();
                    }
                    catch (FaultException<MaximunNumberOfPlayersReachedException> e)
                    {
                        Console.WriteLine(e.Detail.reason);
                        Console.WriteLine("The maximum number of players is: " + e.Detail.playerNum);
                        Console.ReadLine();
                    }
                    catch (FaultException<HostCannotJoinTheGameException> e)
                    {
                        Console.WriteLine(e.Detail.reason);
                        Console.WriteLine(e.Detail.playerName + ", you can not host the game and play!");
                        Console.ReadLine();
                    }
                    catch (FaultException<GameIsNotBeingHostedException> e)
                    {
                        Console.WriteLine(e.Detail.reason);
                        Console.ReadLine();
                    }
                    catch (FaultException<PlayerNotPlayingTheGameException> e)
                    {
                        Console.WriteLine(e.Detail.reason);
                        Console.WriteLine(e.Detail.playerName + " you are not in this game.");
                        Console.ReadLine();
                    }

                }
            }
        }
    }
}
