using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// Controls the in game ui, WIP, stuff is all over the place right now and this only currently controls the Dialogs.
    /// </summary>
    public class InGameUIController : MonoBehaviour
    {
        public Canvas dialogBox;
        public Text dialogBoxTitle;
        public Text dialogBoxMessage;
        public Button[] dialogBoxButtons;

        public RectTransform greenArrow;
        private RectTransform _greenArrowTarget;
        private IDisposable _greenArrowLerpCoroutine;

        public delegate void DialogCallback(Canvas dialog);
        
        /// <summary>
        /// Shows the dialog box
        /// </summary>
        /// <param name="title">dialog box title</param>
        /// <param name="message">dialog box message</param>
        /// <param name="options">array of 3 option titles (max)</param>
        /// <param name="callbacks">array of 3 callbacks for each option</param>
        public void ShowDialogBox(string title, string message, string[] options = null,
            DialogCallback[] callbacks = null)
        {
            /*
             * no options enabled => show option 1 as a dismiss button
             * 
             * option 1 enabled => show option 1, assume callback is also defined, otherwise the user probably
             * deserves an exception anyway
             *
             * option 2 enabled => show option 1 + 2 and assume both callbacks are defined
             * etc...
             */
            foreach (var button in dialogBoxButtons)
            {
                button.gameObject.SetActive(false);
            }

            if (options == null)
            {
                dialogBoxButtons[2].gameObject.SetActive(true);
                dialogBoxButtons[2].GetComponentInChildren<Text>().text = "Dismiss";
                dialogBoxButtons[2].onClick.AddListener(() => { dialogBox.enabled = false; });
                return;
            }

            for (int i = 0; i < options.Length; i++)
            {
                // the order of the indices are actually reversed (bottom up) as kind of like a pseudo stack layout
                var actualIndex = dialogBoxButtons.Length - 1 - i;
                Debug.Log($"actual index: {actualIndex}");
                var button = dialogBoxButtons[actualIndex];

                button.gameObject.SetActive(true);
                button.GetComponentInChildren<Text>().text = options[i];
                var i1 = i;
                button.onClick.AddListener(() => { callbacks[i1].Invoke(dialogBox); });
            }

            dialogBoxTitle.text = title;
            dialogBoxMessage.text = message;
            dialogBox.enabled = true;
        }

        /// <summary>
        /// Sets the target that the arrow should point to
        /// </summary>
        /// <param name="target"></param>
        public void SetGreenArrowTarget(RectTransform target)
        {
            /*
             * stop the coroutine, set the target position and anchors so that the arrow is pointing at the object
             * then enable the gamobject and start the coroutine
             */
            if (_greenArrowLerpCoroutine != null)
            {
                _greenArrowLerpCoroutine.Dispose();
                _greenArrowLerpCoroutine = null;
            }

            var targetPos = target.anchoredPosition;
            greenArrow.anchorMin = target.anchorMin;
            greenArrow.anchorMax = target.anchorMax;
            var anchorPos = new Vector2(targetPos.x + target.sizeDelta.x/2 + 70f, targetPos.y);
            greenArrow.anchoredPosition = anchorPos;
            greenArrow.gameObject.SetActive(true);

            _greenArrowLerpCoroutine = GreenArrowLerp().ToObservable().Subscribe();
        }

        /// <summary>
        /// stops the arrow coroutine and makes the arrow invisible
        /// </summary>
        public void HideArrow()
        {
            if (_greenArrowLerpCoroutine != null)
            {
                _greenArrowLerpCoroutine.Dispose();
                _greenArrowLerpCoroutine = null;
            }
            greenArrow.gameObject.SetActive(false);
        }

        /// <summary>
        /// Makes the Green Arrow move back and forth
        /// </summary>
        /// <returns></returns>
        private IEnumerator GreenArrowLerp()
        {
            const int lerpPrecision = 20;
            const float lerpUnits = 5f;
            const float lerpTime = 0.3f;
            const float unitPerLoop = lerpUnits / lerpPrecision;
            const float waitTime = lerpTime / lerpPrecision;
            var sign = -1;
            while (greenArrow.gameObject.activeSelf)
            {
                for (int i = 0; i < lerpPrecision; i++)
                {
                    var currentPos = greenArrow.anchoredPosition;
                    var newPos = new Vector2(currentPos.x + i * unitPerLoop * sign, currentPos.y);
                    greenArrow.anchoredPosition = newPos;
                    yield return new WaitForSecondsRealtime(waitTime);
                }

                sign *= -1;
            }
        }

        private void OnDestroy()
        {
            // make sure to stop the coroutine (dispose does that in this context)
            _greenArrowLerpCoroutine?.Dispose();
        }

        /// <summary>
        /// Reloads the game scene, used for respawning
        /// </summary>
        public void ReloadScene()
        {
            GameCache.GameData.FreshRespawn = true;
            SceneManager.LoadScene("StupidGameScene");
        }

        // unused code, just left it here 
        
        // /// <summary>
        // /// Scrolls the dialog using a coroutine
        // /// WARNING, since this method modifies strings many times, you WILL trigger GC more often if you use this method
        // /// PLEASE use textmeshpro if you can!
        // /// </summary>
        // /// <param name="scrollDelay"></param>
        // /// <param name="???"></param>
        // /// <returns></returns>
        // private IEnumerator DialogScroll(float scrollDelay, title)
        // {
        //     
        // }
    }
}