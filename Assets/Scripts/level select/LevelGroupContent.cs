using System.Collections;
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

	// Use this for initialization
	void Start () 
	{

	}

	public void Init()
	{
		m_questionManager = QuestionManager.GetQuestionManager();
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
		
	public void PressedQuestionButton(int levelNumber)
	{
		Debug.Log("PressedQuestionButton "+levelNumber);
	}

	public void PressedLevelButton(int levelNumber)
	{
		Debug.Log("PressedLevelButton "+levelNumber);
		FillTheQuestionSelectContent(levelNumber);
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
			levelData.Add(new LevelSelectButtonData(j, PressedQuestionButton, false, q.pictureFileName));
		}

		m_levelSelectContent.Init(levelData);
	}

	void FillWithGroupsAndQuizzes()
	{
		List<string> groupNames = GetGroupNames();
		int groupCount = 0;
		foreach(string groupName in groupNames)
		{
			// create a group for the UI
			GameObject group = Instantiate(m_levelGroupPrefab, transform);
			LevelSelectContent levelSelectContent = group.GetComponent<LevelSelectContent>();
			List<LevelSelectButtonData> levelData = new List<LevelSelectButtonData>();

			levelSelectContent.SetHeaderText(groupName);

			bool groupIsActive = (groupCount <= 0);

			for(int i=0; i<m_numberOfQuizzes; i++)
			{
				m_questionManager.SetQuiz(i);
				if (string.Compare(m_questionManager.GetGroupName(),groupName) == 0)
				{
					levelData.Add(new LevelSelectButtonData(i, PressedLevelButton, true, m_questionManager.GetQuizName(), groupIsActive));
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
