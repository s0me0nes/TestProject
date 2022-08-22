using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class UIController : MonoBehaviour
    {
        public static Action OnAddScore;
        public static Action OnDeath;

        [Header("References")]
        [SerializeField] private TextMeshProUGUI score;
        [SerializeField] private GameObject endScreen;

        private int _score;

        private const string Score = "Scores:";

        private void Start()
        {
            Time.timeScale = 0;
        }

        private void OnEnable()
        {
            OnAddScore += AddScore;
            OnDeath += ShowEndScreen;
        }

        private void OnDisable()
        {
            OnAddScore -= AddScore;
            OnDeath -= ShowEndScreen;
        }

        private void AddScore()
        {
            _score++;
            score.text = Score + _score;
        }

        private void ShowEndScreen()
        {
            endScreen.gameObject.SetActive(true);
        }

        public void Play()
        {
            Time.timeScale = 1;
        }

        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        }

        public void Exit()
        {
            Application.Quit();
        }
    }
}