using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.UI
{
    public class PauseMenuController : MonoBehaviour
    {
        private bool _isPaused = false;
        public Canvas pauseCanvas;
        public Button resumeButton;
        public Button saveGameButton;
        public Button mainMenuButton;
        public Button quitGameButton;
        public Button pauseGameButton;

        private void Awake()
        {
            resumeButton.onClick.AddListener(Resume);
            saveGameButton.onClick.AddListener(SaveGame);
            mainMenuButton.onClick.AddListener(MainMenu);
            quitGameButton.onClick.AddListener(QuitToDesktop);
            pauseGameButton.onClick.AddListener(Pause);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Pause();
            }
        }

        public void Pause()
        {
            Debug.Log("pause");
            Time.timeScale = 0;
            pauseCanvas.gameObject.SetActive(true);
            _isPaused = true;
        }

        public void Resume()
        {
            Debug.Log("Resume");
            Time.timeScale = 1;
            pauseCanvas.gameObject.SetActive(false);
            _isPaused = false;
        }

        public void MainMenu()
        {
            Time.timeScale = 1;
            // SceneManager.LoadScene();
        }

        public void QuitToDesktop()
        {
            Application.Quit();
        }

        public static void SaveGame()
        {
            var gameData = GameCache.GameData;
            var path = $"{Application.persistentDataPath}/save-{gameData.SaveSlot}.dat";
            Debug.Log($"Save file saved to {path}");
            using var stream = new FileStream(path, FileMode.OpenOrCreate);
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, gameData);
        }
    }
}