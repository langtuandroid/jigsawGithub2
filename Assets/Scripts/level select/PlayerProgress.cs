﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace UnityStandardAssets._2D
{
	public class PlayerProgress : MonoBehaviour 
	{

		[System.Serializable]
		public class SingleQuizState : System.Object
		{
			public int[] m_questions;		// records 0 or 1 for each question in each quiz (for answered or not answered)
		}

		public class SaveClass
		{
			public string[] m_quizNames;
			public SingleQuizState[] m_playerProgress;		
		}

		private QuestionManager m_questionManager;
		SingleQuizState[] m_playerProgress;


        public int TotalPlayerScore()
        {
            int count = 0;
            for(int quizNum = 0; quizNum < m_playerProgress.Length; quizNum++)
            {
                count += QuizScore(quizNum);
            }
            return count;
        }

        public int QuizNumQuestions(int quizNum)
        {
            SingleQuizState quizState = m_playerProgress[quizNum];
            return(quizState.m_questions.Length);
        }

		public int QuizScore(int quizNum)
		{
			Debug.Log("QuizScore "+quizNum);
			SingleQuizState quizState = m_playerProgress[quizNum];
			int tally = 0;

			int count = quizState.m_questions.Length;
			for(int i=0; i<count; i++)
			{
				if (quizState.m_questions[i] > 0)
				{
					tally++;
				}
			}

			return tally;
		}

		public int QuestionScore(int quizNum, int questionNum)
		{
			SingleQuizState quizState = m_playerProgress[quizNum];
			int count = quizState.m_questions.Length;
			if (questionNum < count)
			{
				return quizState.m_questions[questionNum];
			}
			return -1;
		}

		public static PlayerProgress GetPlayerProgress()
		{
			// tries to find in the scene
			// if not, then create one and initialise it.
			GameObject playerProgressObject = GameObject.Find ("PlayerProgressPrefab(Clone)");
			if (playerProgressObject)
			{
				// return the existing one
				return playerProgressObject.GetComponent<PlayerProgress>();
			}
			else
			{
				// create a new one
				playerProgressObject = Instantiate(Resources.Load("PlayerProgressPrefab")) as GameObject;
				PlayerProgress playerProgress = playerProgressObject.GetComponent<PlayerProgress>();
				DontDestroyOnLoad(playerProgress.gameObject);	// make it persist between scenes
				playerProgress.Init();
				return playerProgress;
			}
		}


		// Use this for initialization
		void Start () {

		}

		public void Init()
		{
			m_questionManager = QuestionManager.GetQuestionManager();
			BuildEmptyStateStructure();

			ReadJSONFileIntoStateStructure();
		}

		void CompleteNQuestions(SingleQuizState quizState, int numQuestions)
		{
			int count = quizState.m_questions.Length;
			for(int i=0; i<count; i++)
			{
				if (i < numQuestions)
				{
					quizState.m_questions[i] = 1;
				}
				else
				{
					quizState.m_questions[i] = 0;
				}
			}
		}

		public void SetQuestionState(int quizNumber, int questionNumber, bool state)
		{
			SingleQuizState quizState = m_playerProgress[quizNumber];
			quizState.m_questions[questionNumber] = state ? 1 : 0;
			WriteStateToJSONFile();
		}



		void BuildEmptyStateStructure()
		{
			m_questionManager.SetSelectedQuizType(QuestionManager.QuizTypes.Jigsaw);
			string[] allQuizNames = m_questionManager.AllQuizNamesInCurrentMode();
			m_playerProgress = new SingleQuizState[allQuizNames.Length];
			for (int i=0; i<allQuizNames.Length; i++)
			{
				m_questionManager.SetQuiz(allQuizNames[i]);
				int questionCount = m_questionManager.GetNumberOfQuestions();

				m_playerProgress[i] = new SingleQuizState();
				m_playerProgress[i].m_questions = new int[questionCount];
			}
		}

		// read the file and copy the data into the pre-constructed state structure, trimming or padding it out if necessary. This allows for the number of questions to change.
		void ReadJSONFileIntoStateStructure()
		{
			// build an empty game state structure, with the right number of quizzes and questions for the current data in QuestionManager.
			BuildEmptyStateStructure();

			string path = Application.persistentDataPath + "/jigsawSaveFile.json";
            Debug.Log("ReadJSONFileIntoStateStructure : " + path);
            if (System.IO.File.Exists(path))
			{
				StreamReader reader = new StreamReader (path);
				string json = reader.ReadToEnd ();
				reader.Close ();
				if (json != null)
				{
					// read in the saved state of the game. This may have different quizzes, and different numbers of questions in the quizzes.
					SaveClass saveClass = JsonUtility.FromJson<SaveClass> (json);

					// copy saveClass.m_playerProgress, item by item into m_playerProgress, trimming or padding it out if necessary
					for (int i=0; i<saveClass.m_quizNames.Length; i++)
					{
						string savedQuizName = saveClass.m_quizNames[i];
						if (m_questionManager.QuizExists(savedQuizName))
						{
							int index = m_questionManager.QuizIndex(savedQuizName);
							m_questionManager.SetQuiz(index);
							int questionCount = m_questionManager.GetNumberOfQuestions();
							int savedQuestionCount = saveClass.m_playerProgress[i].m_questions.Length;

							// copy across the state of the questions. 
							// If there are less questions in the saved data than in the local structure, then it will leave the state of last few questions untouched.
							// If there are more questions in the saved data then it will leave off the last few questions from the saved data.
							for (int j=0; (j<questionCount) && (j<savedQuestionCount); j++)
							{
								m_playerProgress[index].m_questions[j] = saveClass.m_playerProgress[i].m_questions[j];
							}
						}
					}
				}
			}
		}

		public void WriteStateToJSONFile()
		{
			SaveClass saveClass = new SaveClass();
			saveClass.m_playerProgress = m_playerProgress;
			saveClass.m_quizNames = m_questionManager.AllQuizNamesInCurrentMode();

			string json = JsonUtility.ToJson (saveClass);
			string path = Application.persistentDataPath + "/jigsawSaveFile.json";
			StreamWriter writer = new StreamWriter (path, false);
			Debug.Log("SaveProgressToFile : "+ path);

			writer.WriteLine (json);
			writer.Close ();
		}
	}
}
