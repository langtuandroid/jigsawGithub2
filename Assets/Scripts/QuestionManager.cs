using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using System.Reflection;


namespace UnityStandardAssets._2D
{
	public struct Question
	{
		public string questionText;
		public int correctAnswer;
		public string[] answers;
		public string[] pictures;
		public string pictureFileName;
		public string pictureFileName2;
		public string prizePictureFileName;
		public string prizeText;
		public int picWidth, picHeight;
		public Rect tapLocation;
		public int tapMinLayer, tapMaxLayer;
		public int gridX, gridY;
	}

	public struct PreScreen
	{
		public string text;
		public string pictureFileName;
	}
	
	public class QuestionManager : MonoBehaviour
    {
		private JsonData _questions;
		private JsonData _quizzes;
		private int _numberOfQuestions;
		private int _numberOfQuizzes;
		private string _selectedLevel;
		private QuizTypes _selectedQuizType;

		public enum QuizTypes 
		{
			quizType1,
			quizType2,
			quizType3,
			quizType4,
			quizType5,
			quizType6,
			quizType7,
			Jigsaw
		}

		private string[] _quizTypeNames = new string[] {"quizType1", "quizType2", "quizType3", "quizType4", "quizType5", "quizType6", "quizType7", "jigsaw"};	//"quizType1"};
		private string[] _quizSceneNames = new string[] {"quizType1Quiz", "quizType2Quiz", "quizType3Quiz", "quizType4Quiz", "quizType5Quiz", "quiz flow", "quizType7Quiz", "JigsawQuiz"};

		private void Awake()
		{
			SetSelectedQuizType(QuizTypes.quizType1);		// default
		}

		public void SetSelectedQuizType(QuizTypes type)
		{
			Debug.Log("SetSelectedQuizType "+type);
			_selectedQuizType = type;
		}

		public string GetQuizSceneNameForCurrentMode()
		{
			return _quizSceneNames[(int)_selectedQuizType];
		}

		public static QuestionManager GetQuestionManager()
		{
			// tries to find question manager in the scene
			// if not, then create one and initialise it.
			GameObject questionManagerObject = GameObject.Find ("QuestionManagerPrefab(Clone)");
			if (questionManagerObject)
			{
				// return the existing one
				return questionManagerObject.GetComponent<QuestionManager>();
			}
			else
			{
				// create a new one
				questionManagerObject = Instantiate(Resources.Load("QuestionManagerPrefab")) as GameObject;
				QuestionManager questionManager = questionManagerObject.GetComponent<QuestionManager>();
				DontDestroyOnLoad(questionManager.gameObject);	// make it persist between scenes
				questionManager.LoadQuestions();
				return questionManager;
			}
		}

		public void LoadQuestions()
		{
			Debug.Log("LoadQuestions");
			string streamingAssetsPath = Application.streamingAssetsPath;
			string jsonFileName = "jigsaw_levels.json";
			string jsonFilePath = streamingAssetsPath+"/"+jsonFileName;

			string json_file_text = System.IO.File.ReadAllText(jsonFilePath);
			
			_quizzes = JsonMapper.ToObject(json_file_text);
			_numberOfQuizzes = _quizzes["quizzes"].Count;
			SetQuiz(0);
		}


		public void RandomiseQuestions()
		{
			// start at the end of the array and work back
			int n = 0;
			int a = (int)(_numberOfQuestions/2)-1;	// don't swap the last few with the first few to avoid repeated questions when we reshuffle at the end of the set.
			a = Mathf.Clamp (a,0,2);
			a = _numberOfQuestions-1-a;
			while (n < _numberOfQuestions-1)
			{
				// swap this question with any before it (including potentially itself, to allow a non-swap as an option)
				int swapPartner = UnityEngine.Random.Range(n,a+1);
				a++;
				if (a >= _numberOfQuestions) a=_numberOfQuestions-1;
				SwapQuestions(n, swapPartner);
				n++;
			}
		}

		public void SwapQuestions(int a, int b)
		{
			if ((a != b) && (a >= 0 && a < _numberOfQuestions) && (b>=0 && b < _numberOfQuestions))
			{
				var temp = _questions["questions"][a];
				_questions["questions"][a] = _questions["questions"][b];
				_questions["questions"][b] = temp;
			}
		}
			
		public string[] AllPictureNames()
		{
			List<string> l = new List<string>();

			// get all the question pictures
			int index = 0;
			for (int i=0; i<_numberOfQuestions; i++)
			{
				if (_questions["questions"][i]["picture"] != null)
					l.Add((string)_questions["questions"][i]["picture"]);
				if (_questions["questions"][i]["picture2"] != null)
					l.Add((string)_questions["questions"][i]["picture2"]);
				if (_questions["questions"][i]["prizepicture"] != null)
					l.Add((string)_questions["questions"][i]["prizepicture"]);
				if (_questions["questions"][i]["pictures"] != null)
				{
					int numberOfPictures = _questions["questions"][i]["pictures"].Count;
					for (int j=0; j<numberOfPictures; j++)
					{
						l.Add((string)_questions["questions"][i]["pictures"][j]);
					}
				}
			}

			// add the prescreen picture if there is one.
			if (IsPreScreen())
			{
				PreScreen ps = GetPreScreen();
				l.Add(ps.pictureFileName);
			}

			return l.ToArray();
		}

		public string[] AllQuizNamesInCurrentMode()
		{
			string modeName = _quizTypeNames[(int)_selectedQuizType];

			// count the quizzes in this mode
			int count = 0;
			for (int i=0; i<_numberOfQuizzes; i++)
			{
				string mode = (string)_quizzes["quizzes"][i]["mode"];
				if (mode.ToLower() == modeName.ToLower())
					count++;
			}

			string[] names = new string[count];
			int index = 0;
			for (int i=0; i<_numberOfQuizzes; i++)
			{
				string mode = (string)_quizzes["quizzes"][i]["mode"];
				if (mode.ToLower() == modeName.ToLower())
					names[index++] = (string)_quizzes["quizzes"][i]["name"];
			}
			return names;
		}

		public string PictureSubDirectory()
		{
			return (string)_questions["subdirectory"];
		}

		public int GetNumberOfQuestions()
		{
			return _numberOfQuestions;
		}

		public int GetNumberOfQuizzes()
		{
			return _numberOfQuizzes;
		}

		public string GetGroupName()
		{
			if (_questions["group"] != null)
			{
				return (string)_questions["group"];
			}
			else
			{
				return "no group";
			}
		}

		public string GetQuizName()
		{
			return (string)_questions["name"];
		}

		public Question GetQuestion(int questionNumber)
		{
			Question q = new Question();
			q.questionText = (string)_questions["questions"][questionNumber]["questionText"];
			if (_questions["questions"][questionNumber]["picture"] != null)
				q.pictureFileName = (string)_questions["questions"][questionNumber]["picture"];
			else
				q.pictureFileName = null;

			if (_questions["questions"][questionNumber]["picture2"] != null)
				q.pictureFileName2 = (string)_questions["questions"][questionNumber]["picture2"];
			else
				q.pictureFileName2 = null;

			if (_questions["questions"][questionNumber]["prizepicture"] != null)
				q.prizePictureFileName = (string)_questions["questions"][questionNumber]["prizepicture"];
			else
				q.prizePictureFileName = null;
			
			if (_questions["questions"][questionNumber]["prizetext"] != null)
				q.prizeText = (string)_questions["questions"][questionNumber]["prizetext"];
			else
				q.prizeText = null;
			
			if (_questions["questions"][questionNumber]["pictures"] != null)
			{
				int numberOfPictures = _questions["questions"][questionNumber]["pictures"].Count;
				q.pictures = new string[numberOfPictures];
				for (int i=0; i<numberOfPictures; i++)
				{
					q.pictures[i] = (string)_questions["questions"][questionNumber]["pictures"][i];
				}
			}


			q.picWidth = 0;
			q.picHeight = 0;
			if (_questions["questions"][questionNumber]["picWidth"] != null)
				q.picWidth = (int)_questions["questions"][questionNumber]["picWidth"];
			if (_questions["questions"][questionNumber]["picHeight"] != null)
				q.picHeight = (int)_questions["questions"][questionNumber]["picHeight"];

			if (_questions["questions"][questionNumber]["correctAnswer"] != null)
			{
				q.correctAnswer = (int)_questions["questions"][questionNumber]["correctAnswer"];
			}

			// mode specific data. Really I should have different question type classes for each game mode.

			if (_selectedQuizType == QuizTypes.quizType1 || 
			    _selectedQuizType == QuizTypes.quizType2 || 
			    _selectedQuizType == QuizTypes.quizType4 || 
			    _selectedQuizType == QuizTypes.quizType5 || 
			    _selectedQuizType == QuizTypes.quizType6)
			{
				Debug.Log("Question number "+questionNumber+", quizType = "+_selectedQuizType);
				int numberOfAnswers = _questions["questions"][questionNumber]["answers"].Count;
				q.answers = new string[numberOfAnswers];
				for (int i=0; i<numberOfAnswers; i++)
				{
					q.answers[i] = (string)_questions["questions"][questionNumber]["answers"][i];
				}
			}

			if (_selectedQuizType == QuizTypes.quizType3 ||
			    _selectedQuizType == QuizTypes.quizType7 ||
			    _selectedQuizType == QuizTypes.Jigsaw
			    )
			{
				int count = _questions["questions"][questionNumber].Count;
				bool contains = (_questions["questions"][questionNumber]["boxBRX"] != null);
				bool contains2 = (_questions["questions"][questionNumber]["boxTLX"] != null);
				bool contains3 = (_questions["questions"][questionNumber]["boxTLY"] != null);

				int tlX = 0, tlY = 0, brX = 0, brY = 0;
				if (_questions["questions"][questionNumber]["boxTLX"] != null)
				{
					tlX = (int)_questions["questions"][questionNumber]["boxTLX"];
					tlY = (int)_questions["questions"][questionNumber]["boxTLY"];

					// in case the bottom right is not specified.
					brX = tlX;
					brY = tlY;
					if (_questions["questions"][questionNumber]["boxBRX"] != null)
					{
						brX = (int)_questions["questions"][questionNumber]["boxBRX"];
						brY = (int)_questions["questions"][questionNumber]["boxBRY"];
					}
				}

				q.tapLocation = new Rect(tlX, tlY, brX-tlX, brY-tlY);
				q.tapMinLayer = -1;
				q.tapMaxLayer = -1;
				if (_questions["questions"][questionNumber]["boxMinLayer"] != null)
					q.tapMinLayer = (int)_questions["questions"][questionNumber]["boxMinLayer"];
				if (_questions["questions"][questionNumber]["boxMaxLayer"] != null)
					q.tapMaxLayer = (int)_questions["questions"][questionNumber]["boxMaxLayer"];
			}

			if (_selectedQuizType == QuizTypes.quizType4 ||
			    _selectedQuizType == QuizTypes.Jigsaw)
			{
				q.gridX = (int)_questions["questions"][questionNumber]["gridX"];
				q.gridY = (int)_questions["questions"][questionNumber]["gridY"];
			}

			return q;
		}


		public bool IsPreScreen()
		{
			return (_questions["prescreen"] != null);
		}

		public PreScreen GetPreScreen()
		{
			PreScreen ps = new PreScreen();
			ps.text = (string)_questions["prescreen"]["questionText"];
			ps.pictureFileName = (string)_questions["prescreen"]["picture"];

			return ps;
		}

		public void SetQuiz(int quizIndex)
		{
			_questions = _quizzes["quizzes"][quizIndex];
			_numberOfQuestions = _questions["questions"].Count;
		}

		public void SetQuiz(string quizName)
		{
			_selectedLevel = quizName;
			int quizIndex = 0;

			// find the quiz name
			for (int i=0; i<_numberOfQuizzes; i++)
			{
				string n = (string)_quizzes["quizzes"][i]["name"];
				if (n == quizName)
				{
					Debug.Log ("found quiz "+quizName+", at "+i);
					quizIndex = i;
					break;
				}
			}

			SetQuiz (quizIndex);
		}

		
		public void AdjustSettings(GameplaySettings settings)
		{
			if (_questions["settings"] == null)
				return;

			ICollection keys = (_questions["settings"] as IDictionary).Keys;
			ICollection values = (_questions["settings"] as IDictionary).Values;

			foreach(string key in keys)
			{
				var value = _questions["settings"][key];

				Type type = settings.GetType();
				FieldInfo prop = type.GetField(key);
				if (value.IsBoolean)
					prop.SetValue (settings, (bool)value);
				else if (value.IsString)
					prop.SetValue (settings, (string)value);
				else if (value.IsInt)
				{
					prop.SetValue (settings, (int)value);
				}
				else if (value.IsDouble)
				{
					prop.SetValue (settings, (float)((double)value));
				}
			}
		}
    }
}
