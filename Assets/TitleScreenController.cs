using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityStandardAssets._2D
{
    public class TitleScreenController : MonoBehaviour
    {
        private QuestionManager m_questionManager;
        private AudioManager m_audioManager;
        private SettingsManager m_settingsManager;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void StartButtonPressed()
        {
            Debug.Log("pressed start button");
            m_questionManager = QuestionManager.GetQuestionManager();
            m_audioManager = AudioManager.GetAudioManager();
            m_settingsManager = SettingsManager.GetSettingsManager();
            m_settingsManager.ResetSettings();

            PlayerProgress playerProgress = PlayerProgress.GetPlayerProgress();
            if (playerProgress.TotalPlayerScore() == 0)
            {
                // player is completely new. Go straight into the game with the first quiz.
                QuestionManager qm = QuestionManager.GetQuestionManager();
                qm.SetQuiz(0, 0);
                string sceneName = qm.GetQuizSceneNameForCurrentMode();
                Debug.Log("scene name : "+sceneName);

                SceneManager.LoadScene(sceneName);
            }
            else
            {
                // player has played the game before. Go into level select.
                SceneManager.LoadScene("jigsaw Level Select");
            }
        }
    }
}
