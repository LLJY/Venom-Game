using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.UI
{
    public class MainMenuController: MonoBehaviour
    {
        public Text[] saveSlotInfo;
        public Text[] emptySaveSlotText;
        public Button[] saveButtons;
        public Canvas saveScreen;
        public Canvas playScreen;
        public Button backButton;
        public Button playButton;
        public Button quitButton;
        private List<GameData> saves=new List<GameData>();

        private void Awake()
        {
            GetSaves();
            Debug.Log(saves);
            SetupUI();
            
            backButton.onClick.AddListener(ShowHomeScreen);
            playButton.onClick.AddListener(ShowSavesScreen);
            quitButton.onClick.AddListener(Application.Quit);
        }

        /// <summary>
        /// Gets all the available saves
        /// it really should be async, but i am lazy
        /// </summary>
        private void GetSaves()
        {
            var files = Directory.GetFiles(Application.persistentDataPath);
            foreach (var file in files)
            {
                if (!file.Contains(".dat")) continue;
                var saveSlot = int.Parse(file[file.Length-5].ToString());
                saves.Add(GameData.LoadGameData(saveSlot));
            }
        }

        /// <summary>
        /// Sets up the user interface with save info
        /// </summary>
        private void SetupUI() 
        {
            for (int i = 0; i < 3; i++)
            {
                var i1 = i;
                saveButtons[i].onClick.AddListener(() =>
                {
                    LoadGame(i1);
                });
                if (i < saves.Count)
                {
                    saveSlotInfo[i].enabled = true;
                    emptySaveSlotText[i].enabled = false;
                    saveSlotInfo[i].text = $"Save Slot {i+1}\n {saves[i].PlayerLevel} XP \n {saves[i].DemonsKilled} Demons Killed";
                }
                else
                {
                    saveSlotInfo[i].enabled = false;
                    emptySaveSlotText[i].enabled = true;
                }
            }
        }

        /// <summary>
        /// Sets the game data and then loads the GameScene
        /// </summary>
        /// <param name="saveSlot">the save slot chosen</param>
        private void LoadGame(int saveSlot)
        {
            Debug.Log($"loading save {saveSlot}");
            /*
             * If the save slot has nothing in it, create a new game data
             * and then set the saveslot to the defined slot
             */
            if (saveSlot < saves.Count-1)
            {
                Debug.Log("loading game data...");
                GameCache.GameData = saves[saveSlot];
            }
            else
            {
                GameCache.GameData = new GameData
                {
                    SaveSlot = saveSlot
                };
            }

            SceneManager.LoadScene("StupidGameScene");
        }

        public void ShowHomeScreen()
        {
            saveScreen.enabled = false;
            playScreen.enabled = true;
        }

        public void ShowSavesScreen()
        {
            saveScreen.enabled = true;
            playScreen.enabled = false;
        }

    }
}