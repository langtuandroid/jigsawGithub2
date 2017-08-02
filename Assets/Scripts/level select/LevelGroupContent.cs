﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets._2D;

public class LevelGroupContent : MonoBehaviour {

//	[SerializeField] 
//	Transform m_levelSelectContent;
	[SerializeField] 
	GameObject m_levelGroupPrefab;
	[SerializeField] 
	LevelSelectContent m_levelSelectContent;

	QuestionManager m_questionManager;
	int m_numberOfQuizzes;
	int m_numberOfLevelsInGroup;
	private PlayerProgress m_playerProgress;
	private int m_currentlySelectedQuiz;

	// Use this for initialization
	void Start () 
	{

	}

	public void Init()
	{
		m_questionManager = QuestionManager.GetQuestionManager();
		m_playerProgress = PlayerProgress.GetPlayerProgress();

		m_questionManager.SetSelectedQuizType(QuestionManager.QuizTypes.Jigsaw);
		m_numberOfQuizzes = m_questionManager.GetNumberOfQuizzes();
		//FillWithQuizzesAndQuestions();
		FillWithGroupsAndQuizzes();
	}

//	void FillWithQuizzesAndQuestions()
//	{
//		Debug.Log("number of groups = "+m_numberOfQuizzes);
//		for(int i=0; i<m_numberOfQuizzes; i++)
//		{
//			m_questionManager.SetQuiz(i);
//			m_numberOfLevelsInGroup = m_questionManager.GetNumberOfQuestions();
//			Debug.Log("m_numberOfLevelsInGroup = "+m_numberOfLevelsInGroup);
//
//			// create a group for the UI
//			GameObject group = Instantiate(m_levelGroupPrefab, transform);
//			LevelSelectContent levelSelectContent = group.GetComponent<LevelSelectContent>();
//			List<LevelSelectButtonData> levelData = new List<LevelSelectButtonData>();
//			for(int j=0; j<m_numberOfLevelsInGroup; j++)
//			{
//				Debug.Log("question number "+j);
//				Question q = m_questionManager.GetQuestion(j);
//				levelData.Add(new LevelSelectButtonData(j, PressedQuestionButton, true, q.pictureFileName));
//			}
//
//			levelSelectContent.Init(levelData);
//		}
//	}
		
	public void PressedQuestionButton(int buttonNumber)
	{
		Debug.Log("PressedQuestionButton "+buttonNumber);
		m_playerProgress.SetQuestionState(m_currentlySelectedQuiz, buttonNumber, true);
		FillTheQuestionSelectContent(m_currentlySelectedQuiz);
		FillWithGroupsAndQuizzes();
	}

	public void PressedLevelButton(int buttonNumber)
	{
		Debug.Log("PressedLevelButton "+buttonNumber);
		m_currentlySelectedQuiz = buttonNumber;
		FillTheQuestionSelectContent(buttonNumber);
		LevelSelectController.Instance.TransitToQuestionView();
	}

	public void FillTheQuestionSelectContent(int levelNumber)
	{
		m_questionManager.SetQuiz(levelNumber);
		m_numberOfLevelsInGroup = m_questionManager.GetNumberOfQuestions();
		Debug.Log("m_numberOfLevelsInGroup = "+m_numberOfLevelsInGroup);

		List<LevelSelectButtonData> levelData = new List<LevelSelectButtonData>();
		for(int j=0; j<m_numberOfLevelsInGroup; j++)
		{
			Debug.Log("question number "+j);
			Question q = m_questionManager.GetQuestion(j);
			LevelSelectButtonData datum = new LevelSelectButtonData(j, PressedQuestionButton, false, q.pictureFileName);
			int score = m_playerProgress.QuestionScore(levelNumber,j);
			datum.SetCompletion(score, 1);
			levelData.Add(datum);
		}

		m_levelSelectContent.Init(levelData);
	}

	bool GroupIsClear(string groupName)
	{
		int incompleteQuestionsCount = 0;
		for(int i=0; i<m_numberOfQuizzes; i++)
		{
			m_questionManager.SetQuiz(i);
			if (string.Compare(m_questionManager.GetGroupName(),groupName) == 0)
			{
				// this quiz is in the specified group
				int score = m_playerProgress.QuizScore(i);
				int outOf = m_questionManager.GetNumberOfQuestions();
				int remainingQuestions = outOf - score;
				incompleteQuestionsCount += remainingQuestions;
			}
		}

		return (incompleteQuestionsCount == 0);
	}

	void FillWithGroupsAndQuizzes()
	{
		ClearExistingGroups();
		List<string> groupNames = GetGroupNames();
		int groupCount = 0;
		bool allClearSoFar = true;
		foreach(string groupName in groupNames)
		{
			// create a group for the UI
			GameObject group = Instantiate(m_levelGroupPrefab, transform);
			LevelSelectContent levelSelectContent = group.GetComponent<LevelSelectContent>();
			List<LevelSelectButtonData> levelData = new List<LevelSelectButtonData>();

			levelSelectContent.SetHeaderText(groupName);

			// group 0 is always active. Others are active if all previous groups have been cleared.
			bool groupIsActive = allClearSoFar;
			allClearSoFar = allClearSoFar && GroupIsClear(groupName);

			for(int i=0; i<m_numberOfQuizzes; i++)
			{
				m_questionManager.SetQuiz(i);
				if (string.Compare(m_questionManager.GetGroupName(),groupName) == 0)
				{
					LevelSelectButtonData datum = new LevelSelectButtonData(i, PressedLevelButton, true, m_questionManager.GetQuizName(), groupIsActive);
					int score = m_playerProgress.QuizScore(i);
					datum.SetCompletion(score,m_questionManager.GetNumberOfQuestions());
					levelData.Add(datum);
				}
			}	
			levelSelectContent.Init(levelData);

			if(!groupIsActive)
			{
				levelSelectContent.SetContentAlpha(0.3f);
			}

			groupCount++;
		}
	}

	void ClearExistingGroups()
	{
		LevelSelectContent[] groups = transform.GetComponentsInChildren<LevelSelectContent>();
		foreach(LevelSelectContent b in groups)
		{
			b.transform.SetParent(null);
			Destroy(b.gameObject);
		}
	}

	List<string> GetGroupNames()
	{
		List<string> groupNames = new List<string>();
		for(int i=0; i<m_numberOfQuizzes; i++)
		{
			m_questionManager.SetQuiz(i);
			string groupName = m_questionManager.GetGroupName();
			if (!groupNames.Contains(groupName))
			{
				groupNames.Add(groupName);
			}
		}

		return groupNames;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

}
