﻿using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class StageData
{
    public int cards;
    public int stage;
}

public class Stage : MonoBehaviour
{
    public StageData mydata;
    public GameObject loadButton;

    public bool isLoaded;


    public float answertime;
    public float showtime;

    int fail;
    int success;
    

    int order;

    public GameObject cardprefab;
    public GameObject canvas;
    public Image timebar;

    public GameObject startButton;
    public GameObject answerPanel;
    public InputField answerfield;

    public Text resultText;

    List<GameObject> currentCards = new List<GameObject>();

    Stopwatch showWatch = new Stopwatch();
    Stopwatch answerWatch = new Stopwatch();

    List<int> numbers = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        order = 0;
        fail = 0;
        success = 0;
        mydata.cards = 3;
        showtime = 2000;
        answertime = 2000;
        answerPanel.SetActive(false);
        resultText.text = "";
        mydata.stage = 4;

    }

    // Update is called once per frame
    void Update()
    {

        if (showWatch.ElapsedMilliseconds > showtime)
        {
            if (order < mydata.cards-1)
            {
                ChangeShowcard(order);
                showWatch.Restart();
            }
            else
            {
                for(int i=0; i<currentCards.Count; i++)
                {
                    Destroy(currentCards[i].gameObject);
                }
                showWatch.Reset();

                order = 0;

                //답 입력 시작
                StartAnswer();

            }
    
        }

        if (answerWatch.IsRunning)
        {
            timebar.fillAmount =1-answerWatch.ElapsedMilliseconds/answertime;

            if (answerWatch.ElapsedMilliseconds > answertime)
            {
                if (numbers[numbers.Count-1-order].ToString() == answerfield.text)
                {
                    if (order < mydata.cards - 1)
                    {
                        answerfield.text = "";
                        order++;
                        answerWatch.Restart();
                    }
                    else
                    {
                        ResultCheck(true);
                    }
                }
                else
                {
                    ResultCheck(false);

                }
            }
        }
    }

    private void ResultCheck(bool pass)
    {
        answerfield.text = "";
        numbers.Clear();


        if (pass)
        {
            success++;

        }
        else
        {
            fail++;
        }

        answerWatch.Reset();
        order = 0;

        answerPanel.SetActive(false);
        startButton.SetActive(true);
        resultText.text = $"SUCCESS : {success}\nFAIL : {fail}";
        CheckTime();
    }

    private void CheckTime()
    {
        if (success+fail==4)
        {
            if (!isLoaded)
            {
                loadButton.SetActive(true);
            }

            if (success == 4)
            {
                Changestage(3);
            }
            else if (success == 3)
            {
                Changestage(2);
            }
            else if (success == 2)
            {
                Changestage(1);
            }
            else if (success == 1)
            {
                //Changestage(0); 변동이 없으므로 주석 처리
            }
            else
            {
                Changestage(-1);
            }

            success = 0;
            fail = 0;
            SaveData();
        }


    }

    private void Changestage(int v)
    {
        mydata.stage += v;

        
    }

    public void StartAnswer()
    {
        currentCards.Clear();
        answerPanel.SetActive(true);
        answerfield.ActivateInputField();
        answerWatch.Start();


    }

    public void ClickStart()
    {
        if (!isLoaded)
        {
            loadButton.SetActive(false);
        }
        

        if (mydata.stage > 4)
        {
            mydata.stage -= 4;
            mydata.cards++;
        }
        else if (mydata.stage < 1)
        {
            mydata.stage += 4;
            mydata.cards--;
        }

        if (mydata.stage == 1)
        {
            showtime = 4000;
            answertime = 4000;
        }
        else if (mydata.stage == 2)
        {
            showtime = 4000;
            answertime = 2000;

        }
        else if (mydata.stage == 3)
        {
            showtime = 2000;
            answertime = 4000;

        }
        else if (mydata.stage == 4)
        {
            showtime = 2000;
            answertime = 2000;

        }

        resultText.text = "";

        startButton.SetActive(false);

        List<GameObject> showCards=new List<GameObject>();

        for (int i=0; i<mydata.cards; i++)
        {
            numbers.Add(Random.Range(1,21));

            GameObject curCard = Instantiate(cardprefab);
            currentCards.Add(curCard);

            curCard.GetComponent<RectTransform>().SetParent(canvas.transform);
            curCard.GetComponent<RectTransform>().anchoredPosition = new Vector3(80 + 230 * i, 0, 0);
            curCard.GetComponentInChildren<Text>().color = new Color(0,0,0,0);
            curCard.GetComponentInChildren<Text>().text = numbers[i].ToString();

        }

        print("testtesttesttse");
        currentCards[0].GetComponentInChildren<Text>().color = new Color(0,0,0,1);
        showWatch.Start();



    }


    public void ChangeShowcard(int i)
    {
        print("change");

        currentCards[i].GetComponentInChildren<Text>().color = new Color(0, 0, 0, 0);
        currentCards[i + 1].GetComponentInChildren<Text>().color = new Color(0, 0, 0, 1);
        order++;
    }

    public void ClickLoad()
    {
        LoadData();
        Destroy(loadButton);
    }

    void SaveData()
    {
        success = 0;
        fail = 0;
        isLoaded = true;
        Destroy(loadButton);
        string jsondata=JsonUtility.ToJson(mydata,true);
        string path = Path.Combine(Application.dataPath, "savedata_rnm.json");
        File.WriteAllText(path, jsondata);
    }

    void LoadData()
    {
        isLoaded = true;
        string path = Path.Combine(Application.dataPath, "savedata_rnm.json");
        string jsonData = File.ReadAllText(path);
        mydata = JsonUtility.FromJson<StageData>(jsonData);
    }
}
