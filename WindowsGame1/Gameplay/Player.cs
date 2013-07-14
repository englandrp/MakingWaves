using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;

namespace WindowsGame1
{
    //player class stores a serializable playersave, 
    public class Player
    {
        public PlayerSave playerSave;

        public Player(string theName, bool face, bool audio, bool background, bool score, bool borisColorDefault,bool cardvideos, bool carddiagrams, int precision, bool leftHanded)
        {
            playerSave.playerName = theName;  
            playerSave.face = face; 
            playerSave.audio = audio; 
            playerSave.background = background;
            playerSave.score = score;
            playerSave.borisColorDefault = borisColorDefault;
            playerSave.signAttempts = new List<signAttempt>();
            playerSave.carddiagrams = carddiagrams;
            playerSave.cardvideos = cardvideos;
            playerSave.precision = precision;
            playerSave.signPacksPlayed = new List<string>();
            playerSave.signPacksPrecision = new List<int>();
            playerSave.leftHanded = leftHanded;

            for (int i = 0; i < 10; i++) //create random saves for testing
            {
                Random ran = new Random();
                signAttempt anAttempt = new signAttempt();
                anAttempt.completed = ran.Next(0, 2) == 0;
                anAttempt.secondsToComplete = ran.Next();
                anAttempt.accuracyPerAttempt = new List<float>();
                for (int j = 0; j < 10; j++)
                {
                    float temp = (float)ran.Next();
                    anAttempt.accuracyPerAttempt.Add(temp);
                }
                playerSave.signAttempts.Add(anAttempt);
            }
        }

        public int GetPrecision(string packName)
        {
            for (int i = 0; i < playerSave.signPacksPlayed.Count; i++)
            {
                if (playerSave.signPacksPlayed[i] == packName)
                {
                    return playerSave.signPacksPrecision[i];
                }
            }
            return 0;
        }
    }

    [Serializable]
    public struct PlayerSave
    {
        public string playerName; 
        string loginImage;
        public bool leftHanded;

        List<bool> completedSigns; //probably not needed, just scores
        List<int> signScores;
        public List<string> signPacksPlayed;
        public List<int> signPacksPrecision;
        public List<signAttempt> signAttempts;

        public bool face; //show emotions?
        public bool audio; //play audio?
        public bool background; //show background?
        public bool score; //show UI/score?
        public bool borisColorDefault; //pink/blue?
        public bool cardvideos; //use videos with the card game?
        public bool carddiagrams; //use diagrams or photos with the card game?
        public int precision; // other setting for overall precision
    }

    [Serializable]
    public struct signAttempt //every time the player attempts a sign, this data is stored to record it. 
    {
        public int precision; //current precision settings for kinect input
        public string signPack; //name of signpack
        public int signNo; //sign's number in it's pack
        public bool completed; //was the user successful
        public int secondsToComplete; //how long it took
        public List<float> accuracyPerAttempt; //logs the accuracy of each attempt until it's correctly answered
    }
}
