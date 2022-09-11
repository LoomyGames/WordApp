using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using System.Linq;

public class GameController : MonoBehaviour
{
    public int POSN;
    public int ColumnN;
    public int[,] WordArray = new int[4, 4];
    public string[] WordsMade = new string[5];
    public TextMeshProUGUI[] varray = new TextMeshProUGUI[5];
    public Button[] buttonarray = new Button[16];

    public string[] correctAnswers = new string[4];
    public Color[] buttonColors = new Color[4];
    public bool[] gridlock = new bool[16];
    public string[] answers = new string[4];
    public TextMeshProUGUI[] hintBoxes = new TextMeshProUGUI[4];
    int[] hints = new int[4];
    string[] wordOrder = new string[4];
    string[] splitFile = new string[] { "\r\n", "\r", "\n" };
    public string[] allAnswers = new string[6500];
    int[,] storePositions = new int[4, 4];
    int[] totalScores = new int[4];
    Dictionary<string, string> randColl = new Dictionary<string, string>();
    public string[] wordStorage = new string[20];

    public TextAsset wordsFile;

    public Button submitButton;

    public int currentLevel = 0;

    public TextMeshProUGUI timerText;
    public TextMeshProUGUI attemptsText;
    public TextMeshProUGUI bottomLevel;
    private int currentAttempt = 5;

    public Text[] endWordsText = new Text[4];
    public Text[] endWordsScore = new Text[4];
    public Button[] meaningButtons;
    public Text meaningText;

    public Text endTotalScore;
    public TextMeshProUGUI alwaysTotal;
    public Text endWordTitle;

    public GameObject popupBox;

    private int currentTotal;
    private int globalTotal;

    public float timeValue = 90;

    PubNubManager pubNubMgr;

    WordData words;
    private void Start()
    {
        // initialise PubNub
        pubNubMgr = new PubNubManager();
        pubNubMgr.Init();

        // testing code for Alex - uncomment to see published data in log (Last published data)
        words = pubNubMgr.GetNewWords();
        /*if (words != null)
        {
            foreach (string word in words.wordStorage)
            {
                Debug.Log(word);
            }
        }*/

        GetDataFromFile();
        Get20Words();
        SelectNextWords(0);
        ClearButtons();
        GenerateGrid();
        LineHighlight(0);
    }

    private void Update()
    {
        if (timeValue > 0)
        {
            timeValue -= Time.deltaTime;
        }
        else
        {
            timeValue = 0;
            currentAttempt = 0;
            PressWin();
        }
        DisplayTime(timeValue);
    }

    void DisplayTime(float timeToDisplay)
    {
        if(timeToDisplay < 0)
        {
            timeToDisplay = 0;
        }

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timerText.text = String.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void ReduceTimer(int reduceTime)
    {
        timeValue -= reduceTime;
    }

    public void LineDefault(int h) //DEFAULT
    {
        int g = h * 4;
        if (g >= 15)
            return;
        for (int c = g; c <= g + 3; c++)
        {
            if (gridlock[c] == true)
            {
                buttonarray[c].GetComponent<Image>().color = Color.white;
                buttonarray[c].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.black;
            }
        }
    }

    public void LineHighlight(int h)//HIGHLIGHT
    {
        int g = h * 4;
        if (g >= 15)
            return;
        for (int c = g; c <= g + 3; c++)
        {
            if (gridlock[c] == true)
            {
                buttonarray[c].GetComponent<Image>().color = Color.black;
                buttonarray[c].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
            }
        }
    }

    public void BUTTON_CLICK(int index)
    {
        Button button = buttonarray[index].GetComponent<Button>();
        int A;
        int B;
        int D;
        D = index; //this is a value from 0 to 15
        if (POSN == 4)
        {
            POSN = 0;
        }
        
        A = D % 4;
        B = D / 4;
        if (POSN == B && gridlock[D] == true)
        {
            if (POSN == 0)
                ColumnN = A;
            button.GetComponent<Image>().color = buttonColors[ColumnN];
            gridlock[D] = false;
            LineDefault(POSN);
            WordsMade[ColumnN] = WordsMade[ColumnN] + button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
            varray[ColumnN].text = WordsMade[ColumnN];
            WordArray[ColumnN, B] = D;
            POSN = POSN + 1;
            LineHighlight(POSN);
            if (POSN > 3)
                LineHighlight(0);
        }
        CheckWin();
    }

    public void button1_Click() //resets everything
    {
        POSN = 0;
        for (int X = 0; X <= 3; X++)
        {
            for (int Y = 0; Y <= 3; Y++)
                WordArray[X, Y] = -1;
        }

        for (int c = 0; c <= 3; c++)
        {
            WordsMade[c] = "";
            varray[c].text = ""; //text
        }
        //Put down the window
        popupBox.SetActive(false);
        CheckWin();
        ClearButtons();
        LineHighlight(0);
    }

    public void CommandButton1_Click()
    {
        if (CheckCanEdit(0))
        {
            DELETECOLUMN(0);
        }
    }

    public void CommandButton2_Click()
    {
        if (CheckCanEdit(1))
        {
            DELETECOLUMN(1);
        }
    }

    public void CommandButton3_Click()
    {
        if (CheckCanEdit(2))
        {
            DELETECOLUMN(2);
        }
    }

    public void CommandButton4_Click()
    {
        if (CheckCanEdit(3))
        {
            DELETECOLUMN(3);
        }
    }

    private void DELETECOLUMN(int X) //deletes the column X
    {
        int xCopy = X;
        int B = 0;
        for (int c = 0; c <= 3; c++) //count how many values are in that column
        {
            int D = WordArray[xCopy, c];
            if (D != -1)
            {
                B++;
            }  
        }
        if(B == 4)
        {
            LineDefault(0);
        }

        if (B > 0) //go back and re-enable the previous button
        {
            int k = WordArray[xCopy, B - 1];
            gridlock[k] = true;
            LineHighlight(B - 1);

            WordArray[xCopy, B - 1] = -1;
            WordsMade[xCopy] = WordsMade[xCopy].Substring(0, WordsMade[xCopy].Length - 1); //String.substring
            varray[xCopy].text = WordsMade[xCopy]; // modifying the answers array with text
            LineDefault(B);
            POSN = B - 1;
            ColumnN = xCopy;
            ReduceTimer(10);
        }

    }

    public bool CheckCanEdit(int x)
    {
        int b = 0;
        for(int c = 0; c < 4; c++)
        {
            if(WordArray[x,c] != -1)
            {
                b++;
            }
        }

        if(b == 4)
        {
            for(int m = 0; m < 4; m++)
            {
                int counter = 0;
                for(int c = 0; c < 4; c++)
                {
                    if(WordArray[m,c] != -1)
                    {
                        counter++;
                    }
                }
                if (counter > 0 && counter < 4 && m != x)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void CheckWin()
    {
        int b = 0;
        for(int m = 0; m < 4; m++)
        {
            totalScores[m] = 0;
            for(int c = 0; c < 4; c++)
            {
                if (WordArray[m,c] != -1)
                {
                    b++;
                }
            }
        }

        if(b == 16)
        {
            submitButton.interactable = true;
        } 
        else
        {
            submitButton.interactable = false;
        }
    }

    public void PressWin()
    {
        for(int m = 0; m < 4; m++)
        {
            totalScores[m] = 0;
        }

        for (int i = 0; i < 4; i++)
        {
            totalScores[i] = IsPrecise(i);
        }

        for (int i = 0; i < 4; i++)
        {
            if (totalScores[i] == 0)
            {
                totalScores[i] = IsInWords(varray[i].text);
            }
        }
        for (int i = 0; i < 4; i++)
        {
            if (totalScores[i] == 0)
            {
                totalScores[i] = Contains(varray[i].text);
            }
        }
        for(int i = 0; i < 4; i++)
        {
            endWordsText[i].text = WordsMade[i];
            endWordsScore[i].text = "" + totalScores[i];
            if (totalScores[i] != 0)
            {
                int tempI = i;
                meaningButtons[i].interactable = true;
                meaningButtons[i].onClick.AddListener(() => DisplayMeaning(tempI));
            }
        }
        currentTotal = totalScores[0] + totalScores[1] + totalScores[2] + totalScores[3];
        endTotalScore.text = "" + currentTotal;
        popupBox.SetActive(true);
    }

    void DisplayMeaning(int i)
    {
        meaningText.text = WordsMade[i] + ": Add meaning here";
    }

    void ClearButtons()
    {
        POSN = 0;
        for(int x = 0; x < 4; x++)
        {
            WordsMade[x] = "";
            varray[x].text = "";
            meaningButtons[x].interactable = false;
            meaningButtons[x].onClick.RemoveAllListeners();
            for(int y = 0; y < 4; y++)
            {
                gridlock[x * 4 + y] = true;
                WordArray[x, y] = -1;
            }
            LineDefault(x);
        }
        LineHighlight(0);
    }

    void GenerateGrid()
    {
        
        string d, ch;
        int ind = 0;

        for(int a1 = 0; a1 < 4; a1++)
        {
            d = answers[a1];
            int x, rnd1;
            for(int b1 = 0; b1 < 4; b1++)
            {
                do
                {
                    rnd1 = UnityEngine.Random.Range(0, 4);
                    x = WordArray[b1, rnd1];
                }
                while (x != -1);

                if (b1 == 0)
                {
                    ind = rnd1;
                    wordOrder[ind] = d;
                }
                storePositions[ind, b1] = (b1 * 4) + rnd1;
                hints[ind] = hints[ind] + (rnd1 + 1);
                string temp = d.Substring(b1, 1);
                ch = temp;
                WordArray[b1, rnd1] = 0;
                buttonarray[(b1 * 4) + rnd1].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ch;
            }
            hintBoxes[ind].text = "" + hints[ind];
        }

        for(int x = 0; x < 4; x++)
        {
            for(int y = 0; y < 4; y++)
            {
                WordArray[x, y] = -1;
                gridlock[x * 4 + y] = true;
            }
        }

        for(int i = 0; i < 16; i++)
        {
            int temp = i;
            buttonarray[i].onClick.AddListener(delegate { BUTTON_CLICK(temp); });
        }
    }

    public void Gamble()
    {
        if(currentAttempt > 1)
        {
            currentAttempt--;
            attemptsText.text = "GOES LEFT: " + currentAttempt;
            ReduceTimer(60);
            popupBox.SetActive(false);
            button1_Click();
        }
    }

    public void Next()
    {
        if (currentLevel < 19)
        {
            button1_Click();
            for(int i = 0; i < 4; i++)
            {
                hints[i] = 0;
            }
            currentLevel++;
            currentAttempt = 5;
            globalTotal += currentTotal;
            endWordTitle.text = "WORD " + (currentLevel + 1);
            alwaysTotal.text = "TOTAL: " + globalTotal;
            attemptsText.text = "GOES LEFT: " + currentAttempt;
            bottomLevel.text = "LEVEL " + (currentLevel + 1);
            timeValue = 900;
            SelectNextWords(currentLevel);
            popupBox.SetActive(false);
            GenerateGrid();
        }
    }

    public void GetDataFromFile()
    {
        allAnswers = wordsFile.ToString().Split(splitFile, System.StringSplitOptions.None);
    }

    int Contains(string col)
    {
        var strSubNames = -1;
        strSubNames = Array.IndexOf(allAnswers, col);

        if(strSubNames != -1)
        {
            return 1;
        }

        return 0;
    }

    int IsInWords(string col)
    {
        var strSubNames = -1;
        strSubNames = Array.IndexOf(answers, col);

        if(strSubNames != -1)
        {
            return 2;
        }

        return 0;
    }

    int IsPrecise(int f)
    {
        int y = 0;
        for (int d = 0; d < 4; d++)
        {
            if (storePositions[f, d] == WordArray[f, d])
            {
                y++;
            }
        }

        if (y == 4)
        {
            return 3;
        }

        return 0;
    }

    void Get4Words()
    {
        int ULimit = allAnswers.Length;
        int numCount = 4;
        randColl = new Dictionary<string, string>();
        do
        {
            try
            {
                int i = UnityEngine.Random.Range(0, ULimit);
                randColl.Add(allAnswers[i], allAnswers[i]);
            }
            catch
            {
                //
            }
        }
        while (randColl.Count < numCount);

        for(int i = 0; i < randColl.Count; i++)
        {
            answers[i] = randColl.ElementAt(i).Value;
        }
    }

    void Get20Words()
    {
        for(int c = 0; c < 20; c++)
        {
            Get4Words();
            wordStorage[c] = answers[0] + "," + answers[1] + "," + answers[2] + "," + answers[3];
        }
    }

    public void SelectNextWords(int f)
    {
        string[] g = new string[4];

        g = wordStorage[f].Split(",");
        for(int i = 0; i < 4; i++)
        {
            answers[i] = g[i];
        }
    }

    public void UpdateFromPubNub() //the method required to update the words from pubnub, also resets the game beforehand
    {
        words = pubNubMgr.GetNewWords();
        if (words != null)
        {
            for (int i = 0; i < 4; i++)
            {
                hints[i] = 0;
            }
            timeValue = 900;
            currentAttempt = 5;
            wordStorage = words.wordStorage;
            SelectNextWords(0);
            ClearButtons();
            GenerateGrid();
            LineHighlight(0);
        }
        
    }
}
