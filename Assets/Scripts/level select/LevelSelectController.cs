﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets._2D;
using UnityEngine.SceneManagement;

public class LevelSelectController : MonoBehaviour 
{
	[SerializeField] 
	GameObject m_groupScrollView;
	[SerializeField] 
	LevelGroupContent m_levelGroupContent;
	[SerializeField] 
	GameObject m_questionScrollView;

    private QuestionManager m_questionManager;
    private AudioManager m_audioManager;
    private SettingsManager m_settingsManager;

    int m_screenWidth;
	Vector2 m_initialScrollViewPos;
    bool m_showingGroupView = false;

	static LevelSelectController m_instance;

	const float TRANSIT_TIME = 0.2f;


	public static LevelSelectController Instance
	{
		get
		{
			return m_instance;
		}
	}

	LevelSelectController()
	{
		m_instance = this;
	}

	void Start()
	{
        m_questionManager = QuestionManager.GetQuestionManager();
        m_audioManager = AudioManager.GetAudioManager();
        m_settingsManager = SettingsManager.GetSettingsManager();
        m_settingsManager.ResetSettings();

        m_initialScrollViewPos = m_groupScrollView.transform.localPosition;
		m_screenWidth = Screen.width;
		m_questionScrollView.transform.localPosition = new Vector2(m_screenWidth, m_initialScrollViewPos.y);
		m_levelGroupContent.Init();
		TransitToGroupView();
	}

	public void TransitToGroupView()
	{
		m_groupScrollView.SetActive(true);
		m_questionScrollView.SetActive(true);
		m_groupScrollView.GetComponent<UnityStandardAssets._2D.Movement>().MoveTo(new Vector2(0f, m_initialScrollViewPos.y),TRANSIT_TIME);
		m_questionScrollView.GetComponent<UnityStandardAssets._2D.Movement>().MoveTo(new Vector2(m_screenWidth, m_initialScrollViewPos.y),TRANSIT_TIME);
        m_showingGroupView = true;
    }

	public void TransitToQuestionView()
	{
		m_groupScrollView.SetActive(true);
		m_questionScrollView.SetActive(true);
		m_groupScrollView.GetComponent<UnityStandardAssets._2D.Movement>().MoveTo(new Vector2(-m_screenWidth, m_initialScrollViewPos.y),TRANSIT_TIME);
		m_questionScrollView.GetComponent<UnityStandardAssets._2D.Movement>().MoveTo(new Vector2(0f, m_initialScrollViewPos.y),TRANSIT_TIME);
        m_showingGroupView = false;
    }

	public void PressedBackButton()
	{
        if (m_showingGroupView)
        {
            SceneManager.LoadScene("jigsaw title screen");
        }
        else
        {
            TransitToGroupView();
        }
	}
}
