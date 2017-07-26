using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace UnityStandardAssets._2D
{
	public class PlayerProgress : MonoBehaviour {

		public class SaveClass
		{
			public int[][] m_playerProgress;
		}

		private QuestionManager m_questionManager;
		int[][] m_playerProgress;

		// Use this for initialization
		void Start () {
			m_questionManager = QuestionManager.GetQuestionManager();

		}



		void BuildEmptyStateStructure()
		{
			m_questionManager.SetSelectedQuizType(QuestionManager.QuizTypes.Jigsaw);
			string[] allQuizNames = m_questionManager.AllQuizNamesInCurrentMode();
			int[][] m_playerProgress = new int[allQuizNames.Length][];
			for (int i=0; i<allQuizNames.Length; i++)
			{
				m_questionManager.SetQuiz(allQuizNames[i]);
				int questionCount = m_questionManager.GetNumberOfQuestions();

				m_playerProgress[i] = new int[questionCount];
			}
		}

		// read the file and copy the data into the pre-contructed state structure, trimming or padding it out if necessary. This allows for the number of questions to change.
		void ReadJSONFileIntoStateStructure()
		{
			string path = Application.persistentDataPath + "/savefile.json";
			if (System.IO.File.Exists(path))
			{
				StreamReader reader = new StreamReader (path);
				string json = reader.ReadToEnd ();
				reader.Close ();
				if (json != null)
				{
					SaveClass saveClass = JsonUtility.FromJson<SaveClass> (json);

					// TODO: copy saveClass.m_playerProgress, item by item into m_playerProgress
				}
			}
		}

		void WriteStateToJSONFile()
		{
			SaveClass saveClass = new SaveClass();
			saveClass.m_playerProgress = m_playerProgress;

			string json = JsonUtility.ToJson (saveClass);
			string path = Application.persistentDataPath + "/savefile.json";
			StreamWriter writer = new StreamWriter (path, false);
			Debug.Log("SaveProgressToFile 3");

			writer.WriteLine (json);
			writer.Close ();
		}
	}
}
