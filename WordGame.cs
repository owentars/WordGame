using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WordGame
{
    [ServiceBehavior]
    public class WordGame : IWordGame
    {
        // the maximum number of players allowed playing simultaneously
        private const int MAX_PLAYERS = 5;
        // the user hosting the game. If it's null nobody is hosting the game
        private static String userHostingTheGame = null;
        // the Word object that contains the scrambled and unscrambled words
        private static Word gameWords;
        // the list of players playing the game
        private static List<string> activePlayers = new List<string>();

        //Checks if game has host
        [OperationBehavior]
        public bool isGameBeingHosted()
        {
            if (userHostingTheGame == null)
            {
                return false;
            }

            return true;
        }

        [OperationBehavior]
        public string hostGame(string playerName, string wordToScramble)
        {

            if (userHostingTheGame != null)
            {
                GameBeingHostedException exception = new GameBeingHostedException();
                exception.reason = "Game already being hosted";
                throw new FaultException<GameBeingHostedException>(exception);

            }
            userHostingTheGame = playerName;
            gameWords = new Word();
            gameWords.unscrambledWord = wordToScramble;
            gameWords.scrambledWord = scrambleWord(wordToScramble);

            return gameWords.scrambledWord;

        }

        [OperationBehavior]
        public Word join(string playerName)
        {
            activePlayers.Add(playerName);
            if (activePlayers.Count > MAX_PLAYERS)
            {
                MaximunNumberOfPlayersReachedException exception = new MaximunNumberOfPlayersReachedException();
                exception.playerNum = MAX_PLAYERS;
                exception.reason = "Too many players in game!";
                throw new FaultException<MaximunNumberOfPlayersReachedException>(exception);
            }
            if (playerName == userHostingTheGame)
            {
                HostCannotJoinTheGameException exception = new HostCannotJoinTheGameException();
                exception.playerName = playerName;
                exception.reason = "Host cannot join game!";
                throw new FaultException<HostCannotJoinTheGameException>(exception);
            }
            if (!isGameBeingHosted())
            {
                GameIsNotBeingHostedException exception = new GameIsNotBeingHostedException();
                exception.reason = "Game is no longer being hosted!";
                throw new FaultException<GameIsNotBeingHostedException>(exception);
            }
            return gameWords;
        }

        [OperationBehavior]
        public bool guessWord(string playerName, string guessedWord, string unscrambledWord)
        {

            if (!activePlayers.Contains(playerName))
            {
                PlayerNotPlayingTheGameException exception = new PlayerNotPlayingTheGameException();
                exception.playerName = playerName;
                exception.reason = "Player is not in current game.";
                throw new FaultException<PlayerNotPlayingTheGameException>(exception);
            }
            if (guessedWord == unscrambledWord)
            {
                return true;
            }

            return false;

        }

        private string scrambleWord(string word)
        {
            char[] chars = word.ToArray();
            Random r = new Random(2011);
            for (int i = 0; i < chars.Length; i++)
            {
                int randomIndex = r.Next(0, chars.Length);
                char temp = chars[randomIndex];
                chars[randomIndex] = chars[i];
                chars[i] = temp;
            }
            return new string(chars);
        }
    }
}
