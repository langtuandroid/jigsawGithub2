using System.Collections;
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
			//public int[] m_experiment;		// delete me
			//public ExperimentClass[] m_experiment2;
			//public int test;
			//public ExperimentClass m_experiment3;
			public int[][] m_test2;

		}

		private QuestionManager m_questionManager;
		SingleQuizState[] m_playerProgress;

		// Use this for initialization
		void Start () {
			m_questionManager = QuestionManager.GetQuestionManager();
			BuildEmptyStateStructure();

			// AGTEMP: test
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
			string path = Application.persistentDataPath + "/jigsawSaveFile.json";
			if (System.IO.File.Exists(path))
			{
				StreamReader reader = new StreamReader (path);
				string json = reader.ReadToEnd ();
				reader.Close ();
				if (json != null)
				{
					// read in the saved state of the game. This may have different quizzes, and different numbers of questions in the quizzes.
					SaveClass saveClass = JsonUtility.FromJson<SaveClass> (json);

					// build an empty game state structure, with the right number of quizzes and questions for the current data in QuestionManager.
					BuildEmptyStateStructure();

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
							// If there are less questions in the saved data than in the local structure, then there will leave the last few questions untouched.
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

		void WriteStateToJSONFile()
		{
			SaveClass saveClass = new SaveClass();
			saveClass.m_playerProgress = m_playerProgress;
			saveClass.m_quizNames = m_questionManager.AllQuizNamesInCurrentMode();
//			saveClass.m_experiment = new int[10];
//			saveClass.m_experiment2 = new ExperimentClass[10];
//			saveClass.m_experiment3 = new ExperimentClass();

			saveClass.m_test2 = new int[3][];
			saveClass.m_test2[0] = new int[5];
			saveClass.m_test2[1] = new int[5];
			saveClass.m_test2[2] = new int[5];

			string json = JsonUtility.ToJson (saveClass);
			string path = Application.persistentDataPath + "/jigsawSaveFile.json";
			StreamWriter writer = new StreamWriter (path, false);
			Debug.Log("SaveProgressToFile 3");

			writer.WriteLine (json);
			writer.Close ();
		}
	}
}
