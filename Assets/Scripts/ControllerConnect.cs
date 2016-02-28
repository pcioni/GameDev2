﻿using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public class ControllerConnect : MonoBehaviour
{
    public GameObject mainMenu;

    private Regex joystickRegex;
    private List<int> controllerIds;
    private int controllerCount;
    private readonly int MAX_PLAYERS = 4;
    private readonly Color32 ACTIVE_COLOR = new Color32(56, 142, 60, 255);
    private readonly Color32 INACTIVE_COLOR = new Color32(211, 47, 47, 255);

    private List<Color32> playerColors;
    private readonly Color32 P1_Color = new Color32(244, 67, 54, 255);
    private readonly Color32 P2_Color = new Color32(33, 150, 243, 255);
    private readonly Color32 P3_Color = new Color32(76, 175, 80, 255);
    private readonly Color32 P4_Color = new Color32(255, 235, 59, 255);

    private int characterCount = 4;

    private List<GameObject> csList;
    private List<List<GameObject>> csIconList;
    private List<GameObject> pbList;
    private List<GameObject> dbList;
    private GameObject cb;

    private List<List<int>> characterSelections;
    private List<int> playerSelections;

    private List<float> timeSinceLastMovement;
    private readonly float TIME_TO_MOVE = 0.2f;

    private bool isBackPressed;
    private Curtain curtain;

	// Use this for initialization
	void Start ()
	{
        joystickRegex = new Regex(@"Joystick([0-9]+)Button([0-9]+)");
        controllerIds = new List<int>();
        controllerCount = Input.GetJoystickNames().Length;
        controllerCount = 3;

        playerColors = new List<Color32>();
        playerColors.Add(P1_Color);
        playerColors.Add(P2_Color);
        playerColors.Add(P3_Color);
        playerColors.Add(P4_Color);

        csList = new List<GameObject>();
        for (var i = 1; i < characterCount+1; i++) csList.Add(transform.Find("CS " + i).gameObject);

        csIconList = new List<List<GameObject>>();
        for (var i = 0; i < characterCount; i++)
        {
            csIconList.Add(new List<GameObject>());

            for (var i2 = 0; i2 < 4; i2++)
            {
                csIconList[i].Add(csList[i].transform.Find("icon " + (i2+1)).gameObject);
            }
        }

        pbList = new List<GameObject>();
        for (var i = 1; i < 5; i++) pbList.Add(transform.Find("PB " + i).gameObject);

        dbList = new List<GameObject>();
        for (var i = 1; i < 5; i++) dbList.Add(transform.Find("DB " + i).gameObject);

        cb = transform.Find("CB").gameObject;

        characterSelections = new List<List<int>>();
        for (var i = 0; i < characterCount; i++) characterSelections.Add(new List<int>());
        for (var i = 0; i < controllerCount; i++) characterSelections[0].Add(i);

        playerSelections = new List<int>();
        for (var i = 0; i < controllerCount; i++) playerSelections.Add(0);

        timeSinceLastMovement = new List<float>();
        for (var i = 0; i < controllerCount; i++) timeSinceLastMovement.Add(0);

	    isBackPressed = false;
        curtain = transform.Find("Curtain").gameObject.GetComponent<Curtain>();

        generateCharacterUI();
	    generatePlayerUI();

	    updateUI();
	}
	
	// Update is called once per frame
	void Update ()
	{

	    detectControllers();
        moveSelections();

        if (Input.GetKeyDown(KeyCode.S)) test1();
        if (Input.GetKeyDown(KeyCode.D)) test2();


        if (Input.GetKeyDown(KeyCode.JoystickButton1) && !isBackPressed)
	    {
	        isBackPressed = true;
            curtain.close();
	    }

	    if (isBackPressed && !curtain.isRunning)
	    {
	        isBackPressed = false;
	        gotoMainMenu();
            curtain.instantOpen();
	    }

        animateTest();

	}

    void generateCharacterUI()
    {
        for (var i = 0; i < characterCount; i++)
        {
            GameObject cs = csList[i];
            List<int> currentSelected = characterSelections[i];

            for (var i2 = 0; i2 < 4; i2++)
            {
                GameObject csIcon = cs.transform.Find("icon " + (i2+1)).gameObject;
                Image csIconImage = csIcon.GetComponent<Image>();

                if (currentSelected.Count > i2) csIconImage.color = playerColors[currentSelected[i2]];
                else csIconImage.color = new Color32(158,158,158,255);
            }


        }
    }

    void generatePlayerUI()
    {
        for (var i = 0; i < 4; i++)
        {
            GameObject pb = pbList[i];
            GameObject db = dbList[i];

            Image pbImage = pb.GetComponent<Image>();
            Image dbImage = db.GetComponent<Image>();

            if (controllerCount > i)
            {
                pbImage.color = playerColors[i];
                dbImage.color = playerColors[i];
            }

            else {
                pbImage.color = new Color32(33, 33, 33, 255);
                dbImage.color = new Color32(33, 33, 33, 255);
            }

        }
    }


    void detectControllers()
    {

        if (Input.anyKeyDown)
        {
            foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(kcode))
                {
                    String buttonString = kcode.ToString();

                    Match match = joystickRegex.Match(buttonString);

                    // if a key is pressed on a controller/joystick
                    if (match.Success)
                    {
                        int controllerId = Int32.Parse(match.Groups[1].Value);
                        int controllerInput = Int32.Parse(match.Groups[2].Value);

                        controllerDetected(controllerId, controllerInput);
                        Debug.logger.Log(controllerId + " " + controllerInput);
                    }

                    else Debug.logger.Log(buttonString);

                }
            }
        }


    }


    void controllerDetected(int controllerId,int buttonId)
    {
        // 0 is a, 1 is b for Xbox controller
        switch (buttonId)
        {
            case 0 : addController(controllerId); break;
            case 1 : removeController(controllerId); break;
        }


    }

    void addController(int controllerId)
    {

        // only add new controller if it doesn't exist and player count is < MAX_PLAYERS
        if (!controllerIds.Contains(controllerId) && controllerIds.Count < MAX_PLAYERS)
        {
            controllerIds.Add(controllerId);
            Debug.logger.Log("Player " + getPlayerIndex(controllerId) + " added");
            updateUI();
        }


    }

    void removeController(int controllerId)
    {
        // only remove if it exists
        if (controllerIds.Contains(controllerId))
        {
            Debug.logger.Log("Player " + getPlayerIndex(controllerId) + " removed");
            controllerIds.Remove(controllerId);
            updateUI();
        }

    }

    int getPlayerIndex(int controllerId)
    {
        return controllerIds.IndexOf(controllerId);
    }

    void updateUI()
    {

        Debug.logger.Log("Count: " + controllerIds.Count);
    }

    void moveSelections()
    {

        for (var i = 0; i < controllerCount; i++)
        {
            if (timeSinceLastMovement[i] > TIME_TO_MOVE)
            {

                int currentPosition = playerSelections[i];
                int prevPosition = currentPosition;

                // move down
                if (Input.GetAxis("Joy" + (i + 1) + "_LeftStickVertical") < -0.5f)
                {
                    if (currentPosition == (characterCount - 1)) currentPosition = 0;
                    else currentPosition++;
                }

                // move up
                else if (Input.GetAxis("Joy" + (i + 1) + "_LeftStickVertical") > 0.5f)
                {
                    if (currentPosition == 0) currentPosition = (characterCount - 1);
                    else currentPosition--;
                }

                // if position changed, 
                if (prevPosition != currentPosition)
                {
                    characterSelections[prevPosition].Remove(i);
                    playerSelections[i] = currentPosition;
                    characterSelections[currentPosition].Add(i);

                    generateCharacterUI();
                    generatePlayerUI();

                    timeSinceLastMovement[i] = 0;
                }

            }

            timeSinceLastMovement[i] += Time.deltaTime;
        }

    }

    public void onClick(String gameMode)
    {
        gameObject.SetActive(true);
        mainMenu.SetActive(false);
    }

    void gotoMainMenu()
    {
        gameObject.SetActive(false);
        mainMenu.SetActive(true);
    }

    void animateTest()
    {
        for (var i = 0; i < controllerCount; i++)
        {
            if (timeSinceLastMovement[i] < TIME_TO_MOVE && Time.fixedTime > 1)
            {
                int iconIndex = characterSelections[playerSelections[i]].IndexOf(i);
                float scale = timeSinceLastMovement[i] / TIME_TO_MOVE;
                csIconList[playerSelections[i]][iconIndex].transform.localScale = new Vector3(scale, scale, 1);

            }
        }
        
        
    }

    void test1()
    {
        if (timeSinceLastMovement[0] > TIME_TO_MOVE)
        {
            int currentPosition = playerSelections[0];
            int prevPosition = currentPosition;

            if (currentPosition == (characterCount - 1)) currentPosition = 0;
            else currentPosition++;

            characterSelections[prevPosition].Remove(0);
            playerSelections[0] = currentPosition;
            characterSelections[currentPosition].Add(0);

            generateCharacterUI();
            generatePlayerUI();

            timeSinceLastMovement[0] = 0;
        }
    }

    void test2()
    {
        if (timeSinceLastMovement[1] > TIME_TO_MOVE)
        {
            int currentPosition = playerSelections[1];
            int prevPosition = currentPosition;

            if (currentPosition == (characterCount - 1)) currentPosition = 0;
            else currentPosition++;

            characterSelections[prevPosition].Remove(1);
            playerSelections[1] = currentPosition;
            characterSelections[currentPosition].Add(1);

            generateCharacterUI();
            generatePlayerUI();

            timeSinceLastMovement[1] = 0;
        }
    }


}
