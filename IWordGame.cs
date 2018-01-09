using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WordGame
{
    
    [ServiceContract]
    public interface IWordGame
    {
        // Returns true if the game is already being hosted or false otherwise
        [OperationContract]
        bool isGameBeingHosted();

        // User 'userName' tries to host the game with word 'wordToScramble'
        // The function returns the name of the person hosting the game
        // Exception: game is already being hosted by someone else
        [FaultContract(typeof(GameBeingHostedException))]
        [OperationContract]
        string hostGame(String userName, String wordToScramble);

        // Player 'playerName' tries to join the game
        // The function returns a Word object containing the host's (un)scrambled words
        // Exception: maximum number of players reached
        // Exception: host cannot join the game
        // Exception: nobody is hosting the game
        [FaultContract(typeof(MaximunNumberOfPlayersReachedException))]
        [FaultContract(typeof(HostCannotJoinTheGameException))]
        [FaultContract(typeof(GameIsNotBeingHostedException))]
        [OperationContract]
        Word join(string playerName);

        // Player 'playerName' guesses word 'guessedWord' compared with word 'unscrambledWord'
        // Returns true if 'guessedWord' is identical to 'unscrambledWord' or false otherwise
        // Exception: user is not playing the game
        [FaultContract(typeof(PlayerNotPlayingTheGameException))]
        [OperationContract]
        bool guessWord(string playerName, string guessedWord, string unscrambledWord);

    }

    [DataContract]
    public class Word
    {
        [DataMember]
        public string unscrambledWord; // word typed by the game's host
        [DataMember]
        public string scrambledWord;
    }

    [DataContract]
    public class GameBeingHostedException
    {
        [DataMember]
        public string reason;
    }
    [DataContract]
    public class MaximunNumberOfPlayersReachedException
    {
        [DataMember]
        public int playerNum;
        [DataMember]
        public string reason;
    }
    [DataContract]
    public class HostCannotJoinTheGameException
    {
        [DataMember] public string playerName;
        [DataMember] public string reason;
    }
    [DataContract]
    public class GameIsNotBeingHostedException
    {
        [DataMember] public string reason;
    }
    [DataContract]
    public class PlayerNotPlayingTheGameException
    {
        [DataMember] public string playerName;
        [DataMember] public string reason;
    }

}
