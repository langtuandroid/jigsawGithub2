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

    const bool MUST_COMPLETE_QUESTIONS_WITHIN_QUIZ_IN_SEQUENCE = true;
    const bool MUST_COMPLETE_QUIZZES_IN_SEQUENCE = true;

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

        bool active = true;
        int activeCount = m_numberOfLevelsInGroup;
		List<LevelSelectButtonData> levelData = new List<LevelSelectButtonData>();
		for(int j=0; j<m_numberOfLevelsInGroup; j++)
		{
			Debug.Log("question number "+j);
			Question q = m_questionManager.GetQuestion(j);
            int score = m_playerProgress.QuestionScore(levelNumber, j);
            LevelSelectButtonData datum = new LevelSelectButtonData(j, PressedQuestionButton, false, q.pictureFileName, active);
			datum.SetCompletion(score, 1);
            if (MUST_COMPLETE_QUESTIONS_WITHIN_QUIZ_IN_SEQUENCE && (score == 0) && active)
            {
                activeCount = j + 1;
                active = false;
            }
			levelData.Add(datum);
		}

		m_levelSelectContent.Init(levelData);

        m_levelSelectContent.SetContentAlpha(0.3f, activeCount);
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
		bool allGroupsClearSoFar = true;
		foreach(string groupName in groupNames)
		{
			// create a group for the UI
			GameObject group = Instantiate(m_levelGroupPrefab, transform);
			LevelSelectContent levelSelectContent = group.GetComponent<LevelSelectContent>();
			List<LevelSelectButtonData> levelData = new List<LevelSelectButtonData>();

			levelSelectContent.SetHeaderText(groupName);

			// group 0 is always active. Others are active if all previous groups have been cleared.
			bool groupIsActive = allGroupsClearSoFar;
			allGroupsClearSoFar = allGroupsClearSoFar && GroupIsClear(groupName);

            bool quizIsActive = true;
            int numberOfSelectableQuizzes = 0;
            int quizCount = 0;
            for (int i=0; i<m_numberOfQuizzes; i++)
			{
				m_questionManager.SetQuiz(i);
				if (string.Compare(m_questionManager.GetGroupName(),groupName) == 0)
				{
					LevelSelectButtonData datum = new LevelSelectButtonData(i, PressedLevelButton, true, m_questionManager.GetQuizName(), groupIsActive && (quizIsActive || !MUST_COMPLETE_QUIZZES_IN_SEQUENCE));
					int score = m_playerProgress.QuizScore(i);
					datum.SetCompletion(score,m_questionManager.GetNumberOfQuestions());
                    quizCount++;
                    if (MUST_COMPLETE_QUIZZES_IN_SEQUENCE && (score == 0) && quizIsActive)
                    {
                        // found the first non=active quiz. All quizzes up to and including this one are selectable, others are not.
                        numberOfSelectableQuizzes = quizCount;
                        quizIsActive = false;
                    }
                    levelData.Add(datum);
				}
			}
            if (numberOfSelectableQuizzes == 0)
            {
                // didn't find a non-active quiz. They are all selectable.
                numberOfSelectableQuizzes = quizCount;
            }
			levelSelectContent.Init(levelData);

            Debug.Log("groupIsActive " + groupIsActive + ", activeQuizCount " + numberOfSelectableQuizzes);

            if (!groupIsActive)
            {
                numberOfSelectableQuizzes = 0;
            }

			levelSelectContent.SetContentAlpha(0.3f, numberOfSelectableQuizzes);

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
