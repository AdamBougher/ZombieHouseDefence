using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace ZombieHouseDefense
{
        
    public class GameManager : MonoBehaviour
    {
        public static GameTime Time;
        public static UnityAction Pause, Unpause;

        [ShowInInspector]
        public static bool GameOver, GamePaused, CanLevelUp = true;
        public static int Score;
        public static System.Action OnPauseStateChanged;

        //instance variables
        private AudioSource _audioSource;

        //audio clips
        public AudioClip levelUp;

        private void Start()
        {
            //Application.targetFrameRate = 240;

            SceneManager.LoadScene($"Ui", LoadSceneMode.Additive);

            //setup references
            _audioSource = GetComponent<AudioSource>();

            //create and start the game time
            Time = new GameTime();
            StartCoroutine(Time.Time());
            
            GamePaused = false;
            GameOver = false;
        }

        public static void AddToScore(int amt)
        {
            Score += amt;
        }

        /// <summary>
        /// Toggle pause state; only fires events when state actually changes.
        /// </summary>
        private static void PauseGame(string context = "")
        {
            bool wasPaused = GamePaused;
            GamePaused = !GamePaused;

            if (wasPaused == GamePaused) return; // State didn't change, exit early

            // Invoke appropriate event
            if (GamePaused)
            {
                if (Pause != null)
                {
                    foreach (var d in Pause.GetInvocationList())
                    {
                        if (d.Target == null)
                            Pause -= (UnityAction)d;
                    }
                    Pause.Invoke();
                }
            }
            else
            {
                if (Unpause != null)
                {
                    foreach (var d in Unpause.GetInvocationList())
                    {
                        if (d.Target == null)
                            Unpause -= (UnityAction)d;
                    }
                    Unpause.Invoke();
                }
            }

            // Fire idempotent state change event
            OnPauseStateChanged?.Invoke();
            Time.ToggleTimeStopped();
        }

        public void SetupLevelUp()
        {
            PauseGame();
            //play audio cue
            _audioSource.clip = levelUp;
            _audioSource.Play();
        
        
        }

        public static void EndLevelUp()
        {
            PauseGame();
        }  

        private void OnDisable()
        {
            GameManager.Pause -= OnPaused;
            GameManager.Unpause -= OnResume;
        }

        private void OnPaused()
        {

        }

        private void OnResume()
        {
            // Handle resume logic here
        }
    }
}