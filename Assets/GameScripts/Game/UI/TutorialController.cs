using System;
using UnityEngine;
namespace Game.UI
{
    public class TutorialController: MonoBehaviour
    {
        public RectTransform mediumAttackIcon;
        public RectTransform bigAttackIcon;
        public RectTransform healthBar;
        public RectTransform xpBar;

        private InGameUIController _uiController;
        private void Start()
        {
            // disable the script if all the tutorials have been run before
            var gameData = GameCache.GameData;
            _uiController = GameCache.UIController;
            if (gameData.FirstTimePlaying)
            {
                ShowWelcomeMessage();
            }
            if (gameData.FirstTimePlaying && gameData.FirstTimeSeeingAnxiety && gameData.FirstTimeSeeingHarm &&
                gameData.FirstTimeSeeingSuicide) return;
            enabled = false;
        }
        
        private void ShowWelcomeMessage()
        {
            _uiController.ShowDialogBox("Welcome to Venom!",
                "If this is your first time playing, would you like to go through the tutorial?", 
                new[] {"Yes", "No"},
                new InGameUIController.DialogCallback[]
                {
                    x =>
                    {
                        x.enabled = false;
                        ShowFirstTimeTutorial();
                    },
                    x =>
                    {
                        // dismiss the dialogue and disable all future tutorials
                        x.enabled = false;
                        var gameData = GameCache.GameData;
                        gameData.FirstTimePlaying = false;
                        gameData.FirstTimeSeeingAnxiety = false;
                        gameData.FirstTimeSeeingHarm = false;
                        gameData.FirstTimeSeeingSuicide = false;
                    }
                });
        }

        private void ShowFirstTimeTutorial()
        {
            Time.timeScale = 0;
            ShowMediumAttackTutorial();
        }

        private void ShowMediumAttackTutorial()
        {
            _uiController.SetGreenArrowTarget(mediumAttackIcon);
            _uiController.ShowDialogBox("Sword attack indicator",
                "This is your sword attack indicator, it will show you the timeout between each sword attack. \nTo perform a sword attack, you must equip your sword, you can do this by pressing the F key. You may then perform a sword attack using the left mouse button",
                new []{"Got it!"},
                new InGameUIController.DialogCallback[]
                {
                    x =>
                    {
                        x.enabled = false;
                        ShowBigAttackTutorial();
                    }
                });
        }
        private void ShowBigAttackTutorial()
        {
            _uiController.SetGreenArrowTarget(bigAttackIcon);
            _uiController.ShowDialogBox("Laser attack indicator",
                "This is your laser attack indicator, it will show you the timeout between each laser attack, this is your most powerful attack. \n You may perform a laser attack using the right mouse button",
                new []{"Got it!"},
                new InGameUIController.DialogCallback[]
                {
                    x =>
                    {
                        x.enabled = false;
                        ShowHealthTutorial();
                    }
                });
        }

        private void ShowHealthTutorial()
        {
            _uiController.SetGreenArrowTarget(healthBar);
            _uiController.ShowDialogBox("Health Bar",
                "This is your health bar, it tells you how much health you currently have. If it reaches 0, you die, but don't worry your progress will be saved and you can safely resurrect yourself",
                new []{"Got it!"},
                new InGameUIController.DialogCallback[]
                {
                    x =>
                    {
                        x.enabled = false;
                        ShowXpBarTutorial();
                    }
                });
        }
        
        private void ShowXpBarTutorial()
        {
            _uiController.SetGreenArrowTarget(xpBar);
            _uiController.ShowDialogBox("Dopamine Bar",
                "This is your dopamine bar, it tells you how much dopamine you have earned. You can gain dopamine by killing demons, your character will get stronger as you gain more dopamine.",
                new []{"Got it!"},
                new InGameUIController.DialogCallback[]
                {
                    x =>
                    {
                        x.enabled = false;
                        EndTutorial();
                    }
                });
        }

        private void EndTutorial()
        {
            _uiController.HideArrow();
            _uiController.ShowDialogBox("You're Set!",
                "Alright, you should have understood the basics of the game and I will now unpause the game, good luck and happy fighting!",
                new []{"Got it!"},
                new InGameUIController.DialogCallback[]
                {
                    x =>
                    {
                        x.enabled = false;
                        Time.timeScale = 1;
                        GameCache.GameData.FirstTimePlaying = false;
                    }
                });
        }
        
        public void ShowSuicideMobTutorial(Transform position)
        {
            GameCache.CameraScript.ChangeCameraTarget(position);
            _uiController.ShowDialogBox("Suicide Mob",
                "This the suicide mob, it is the strongest mob in the game. It attacks you by throwing a rock at you. Unless your dopamine level is high, you best avoid this mob...",
                new []{"Oh dear..."},
                new InGameUIController.DialogCallback[]
                {
                    x =>
                    {
                        x.enabled = false;
                        GameCache.CameraScript.ResetCameraTarget();
                        GameCache.GameData.FirstTimeSeeingSuicide = false;
                    }
                });
        }
        public void ShowHarmMobTutorial(Transform position)
        {
            GameCache.CameraScript.ChangeCameraTarget(position);
            _uiController.ShowDialogBox("Harm Mob",
                "This the harm mob, it is relatively slow and cannot chase you quickly. Don't be fooled though, for it can do some heavy damage to you",
                new []{"Oh dear..."},
                new InGameUIController.DialogCallback[]
                {
                    x =>
                    {
                        x.enabled = false;
                        GameCache.CameraScript.ResetCameraTarget();
                        GameCache.GameData.FirstTimeSeeingHarm = false;
                    }
                });
        }
        public void ShowAnxietyMobTutorial(Transform position)
        {
            GameCache.CameraScript.ChangeCameraTarget(position);
            _uiController.ShowDialogBox("Anxiety Mob",
                "This the anxiety mob, it is the weakest mob in the game. It does not attack you, however it gives all nearby mobs a 50% health boost. It is hard to kill as it will run away from you.",
                new []{"Haha, that's cute..."},
                new InGameUIController.DialogCallback[]
                {
                    x =>
                    {
                        x.enabled = false;
                        GameCache.CameraScript.ResetCameraTarget();
                        GameCache.GameData.FirstTimeSeeingAnxiety = false;
                    }
                });
        }


    }
}