using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameModel
{
    public List<char> boardCharacters;
    public List<string> answersList;
    public List<string> answeredList;
    public int currentLevel;
    public int currentCoins;
    public bool? flag;
    public string level_path;
    public string name;
    public string email;
    public string username;
    public string avatar;
    GameConfig gameConfig;

    public GameModel()
    {
        if (!flag.HasValue)
        {
            boardCharacters = new List<char>();
            answersList = new List<string>();
            answeredList = new List<string>();
            level_path = Application.dataPath + "/lvl.txt";
            gameConfig = Resources.Load<GameConfig>("gameConfig");
            currentCoins = gameConfig.initialCoins;
            currentLevel = gameConfig.initialLevel;
            name = "";
            email = "";
            username = "";
            avatar = "";
        }
        flag = false;
    }

    public void InstantiateGameConfig(int selected_level)
    {
        SetSelectedLevelWords(selected_level);
        gameConfig = Resources.Load<GameConfig>("gameConfig");
    }

    void SetSelectedLevelWords(int selected_level)
    {
        if (currentLevel != selected_level || answeredList.Count == 0)
        {
            // boardchar va answer list marboot be lvl ro biar
            string line = File.ReadLines(level_path).Skip(selected_level - 1).Take(1).First();
            answersList = line.Split(' ').ToList();
            boardCharacters = answersList[answersList.Count - 1].ToCharArray().ToList();
            answersList.RemoveAt(answersList.Count - 1);
            answeredList = new List<string>();
        }
    }

    public void IncrementCoins(int amount)
    {
        currentCoins += amount;
    }

    public int CheckWordCorrectness(string guessedWord)
    {
        for (int i = 0; i < answersList.Count; i++)
        {
            if (guessedWord == answersList[i] && !answeredList.Contains(guessedWord))
            {
                answeredList.Add(guessedWord);
                return i;
            }
        }
        return -1;
    }

    public int GetHelp()
    {
        if (currentCoins >= gameConfig.helpCost)
        {
            currentCoins -= gameConfig.helpCost;

            List<string> unansweredList = new List<string>();
            for (int i = 0; i < answersList.Count; i++)
            {
                if (!answeredList.Contains(answersList[i]))
                {
                    unansweredList.Add(answersList[i]);
                }
            }
            System.Random random = new System.Random();
            int index = random.Next(unansweredList.Count);
            index = answersList.IndexOf(unansweredList[index]);
            answeredList.Add(answersList[index]);
            return index;
        }
        else
        {
            return -1;
        }
    }

    public List<char> GetBoardCharacters()
    {
        return boardCharacters;
    }

    public List<string> GetAnswersList()
    {
        return answersList;
    }

    public List<string> GetAnsweredList()
    {
        return answeredList;
    }

    public string GetAnsweredWordAtIndex(int index)
    {
        return answeredList[index];
    }

    public int GetLevel()
    {
        return currentLevel;
    }

    public void IncrementCurrentLevel()
    {
        currentLevel += 1;
        answeredList.Clear();
    }

    public void SetName(string name_txt)
    {
        name = name_txt;
    }

    public string GetName()
    {
        return name;
    }

    public void SetEmail(string email_txt)
    {
        email = email_txt;
    }

    public string GetEmail()
    {
        return email;
    }

    public void SetUsername(string username_txt)
    {
        username = username_txt;
    }

    public string GetUsername()
    {
        return username;
    }

    public void SetAvatar(string avatar_path)
    {
        avatar = avatar_path;
    }

    public string GetAvatar()
    {
        return avatar;
    }

    public int GetCoin()
    {
        return currentCoins;
    }
}
