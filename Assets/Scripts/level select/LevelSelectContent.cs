﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LevelSelectButtonData
{
	public int m_levelNum;
	public string m_name;
	public int m_levelID;
	public bool m_isLevel;
	public bool m_isButtonActive;
	public LevelButtonCallback m_callback;
	public int m_completionScore;
	public int m_maxCompletion;

	public LevelSelectButtonData(int levelNum, LevelButtonCallback callback, bool isLevel, string name = "", bool isButtonActive = true)
	{
		m_levelNum = levelNum;
        m_name = name;
		m_callback = callback;
		m_isLevel = isLevel;
		m_isButtonActive = isButtonActive;
	}

	public void SetCompletion(int score, int outOf)
	{
		m_completionScore = score;
		m_maxCompletion = outOf;
	}
}

public class LevelSelectContent : MonoBehaviour {

	[SerializeField] 
	GameObject m_buttonPrefab;
	[SerializeField] 
	Text m_sectionHeader;

    LevelButton[] m_levelButtons;

    // Use this for initialization
    void Start () 
	{
//		//AGTEMP - just use this data for the init for now
//		List<LevelSelectButtonData> testData = new List<LevelSelectButtonData>();
//		for(int i=1; i<20; i++)
//		{
//			testData.Add(new LevelSelectButtonData(i));
//		}
//
//		Init(testData);
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

    public Vector2 GetScrollPosForLevelButton(int n)
    {
        const int NUM_VISIBLE_BUTTONS = 5;

        int centreOffset = (NUM_VISIBLE_BUTTONS + 1) / 2;

        n = n - centreOffset;

        if (n > (m_levelButtons.Length - centreOffset))
        {
            n = m_levelButtons.Length - centreOffset;
        }
        if (n < 0)
        {
            n = 0;
        }

        GameObject gOb = m_levelButtons[n].gameObject;
        RectTransform rt = gOb.GetComponent<RectTransform>();
        Vector2 size = rt.sizeDelta;

        return new Vector2(0f, n * size.y);
    }

	public void Init(List<LevelSelectButtonData> buttonData)
	{
		ClearExistingButtons();

        m_levelButtons = new LevelButton[buttonData.Count];

        int number = 1;
		foreach(LevelSelectButtonData lbd in buttonData)
		{
			GameObject button = Instantiate(m_buttonPrefab, transform);
			LevelButton levelButton = button.GetComponent<LevelButton>();
            m_levelButtons[number-1] = levelButton;
            levelButton.Init(number++, lbd.m_levelNum, lbd.m_name, lbd.m_callback, lbd.m_isButtonActive);
			if (!lbd.m_isLevel)
			{
				// this is a button for a question - show it as either completed or not
				if (lbd.m_completionScore > 0)
				{
					levelButton.HideCompletionCount(false);				
					levelButton.SetCompletedCount(1, 1);
				}
				else
				{
					levelButton.HideCompletionCount(true);
				}
			}
			else
			{
				// this is a button for a quiz. Show the number of questions completed.
				levelButton.SetCompletedCount(lbd.m_completionScore, lbd.m_maxCompletion);
                if (lbd.m_completionScore == 0 && lbd.m_isButtonActive)
                {
                    // we've only just unlocked it. Flash it.
                    UnityStandardAssets._2D.Visibility vis = button.GetComponentInChildren<UnityStandardAssets._2D.Visibility>(true);
                    vis.Flash(5, 0.1f, 0.1f, true);
                }
            }
        }
	}

	void ClearExistingButtons()
	{
		LevelButton[] buttons = transform.GetComponentsInChildren<LevelButton>();
		foreach(LevelButton b in buttons)
		{
			b.transform.SetParent(null);
			Destroy(b.gameObject);
		}
	}

	public void SetHeaderText(string text)
	{
		m_sectionHeader.text = text;
	}


	public void SetContentAlpha(float alpha, int numberOfButtons = 0)
	{
        if (m_sectionHeader != null)
        {
            Color c = m_sectionHeader.color;
            c.a = alpha;
            m_sectionHeader.color = c;
        }

        int count = 0;
		LevelButton[] buttons = transform.GetComponentsInChildren<LevelButton>();
		foreach(LevelButton b in buttons)
		{
            b.SetContentAlpha((count < numberOfButtons) ? 1f : alpha);

            count++;
		}
	}
}
