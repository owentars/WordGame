using System;
using System.ServiceModel;
using WordGame;

namespace WordGameClient
{
    class Program
    {
        static void Main(string[] args)
        {
            WordScrambleClient.ServiceReference.WordGameClient proxy = new WordScrambleClient.ServiceReference.WordGameClient();


            bool canPlayGame = true;

            Console.WriteLine("Player's name?");
            string playerName = Console.ReadLine();



            //Checks if game is being hosted yet
            if (!proxy.isGameBeingHosted())
            {
                Console.WriteLine("Welcome " + playerName + "! Do you want to host the game?");
                if (Console.ReadLine().ToUpper().CompareTo("YES") == 0)
                {
                    Console.WriteLine("Type the word to scramble.");
                    string inputWord = Console.ReadLine();
                    try
                    {
                        string scrambledWord = proxy.hostGame(playerName, inputWord);
                        canPlayGame = false;
                        Console.WriteLine("You're hosting the game with word '" + inputWord + "' scrambled as '" +
                                          scrambledWord + "'");
                        Console.ReadKey();
                    }
                    catch (FaultException<GameBeingHostedException> e)
                    {
                        Console.WriteLine(e.Detail.reason);
                    }

                }
                else
                {
                    
                }
            }

            //Checks if user is able to play game, if hosting they can't
            if (canPlayGame)
            {
                Console.WriteLine("Do you want to play the game?");
                if (Console.ReadLine().ToUpper().CompareTo("YES") == 0)
                {
                    Word gameWords;
                    try
                    {
                        //Attempts to join player to game, if more than MAX_PLAYERS then unable to
                        gameWords = proxy.join(playerName);
                        Console.WriteLine("Can you unscramble this word? => " + gameWords.scrambledWord);
                        string guessedWord;
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
                        Console.ReadLine();
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
