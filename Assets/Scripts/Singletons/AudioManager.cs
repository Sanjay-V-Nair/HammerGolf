using System.Collections.Generic;
using UnityEngine;
using HammerGolf.Gameplay;

namespace HammerGolf {
    public class AudioManager : MonoBehaviour {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource tauntSource;
        [SerializeField] private AudioSource fxSource;

        [Header("Clips")]
        [SerializeField] private AudioClip winClip;
        [SerializeField] private AudioClip bruhClip;
        [SerializeField] private List<AudioClip> tauntClips;

        [Header("Taunt Settings")]
        [SerializeField] private float minTauntInterval = 10f;
        [SerializeField] private float maxTauntInterval = 25f;

        private List<AudioClip> availableTaunts = new List<AudioClip>();
        private bool isGameActive = false;
        private float nextTauntTime = 0f;

        private void Awake() {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            } else {
                Destroy(gameObject);
                return;
            }

            if (tauntSource == null) tauntSource = gameObject.AddComponent<AudioSource>();
            if (fxSource == null) fxSource = gameObject.AddComponent<AudioSource>();
            
            RefillTaunts();
        }

        private void Start() {
            TutorialScreen.OnTutorialComplete += HandleGameStart;
            GameResultPage.OnTryAgain += HandleGameStart;
            
            GoalView.OnGameWon += HandleGameWon;
            BallThrowController.OnBallTimeout += HandleGameOver;
        }

        private void OnDestroy() {
            TutorialScreen.OnTutorialComplete -= HandleGameStart;
            GameResultPage.OnTryAgain -= HandleGameStart;
            
            GoalView.OnGameWon -= HandleGameWon;
            BallThrowController.OnBallTimeout -= HandleGameOver;
        }

        private void Update() {
            if (!isGameActive) return;

            if (Time.time >= nextTauntTime) {
                PlayRandomTaunt();
                ScheduleNextTaunt();
            }
        }

        private void HandleGameStart() {
            isGameActive = true;
            ScheduleNextTaunt();
        }

        private void HandleGameWon() {
            isGameActive = false;
            if (winClip != null) {
                fxSource.PlayOneShot(winClip);
            }
        }

        private void HandleGameOver() {
            isGameActive = false;
            PlayRandomTaunt();
        }

        public void PlayBruhSound() {
            if (bruhClip != null && fxSource != null) {
                fxSource.PlayOneShot(bruhClip);
            }
        }

        private void PlayRandomTaunt() {
            if (tauntClips == null || tauntClips.Count == 0) return;

            if (availableTaunts.Count == 0) {
                RefillTaunts();
            }

            int randomIndex = Random.Range(0, availableTaunts.Count);
            AudioClip clipToPlay = availableTaunts[randomIndex];
            availableTaunts.RemoveAt(randomIndex);

            if (clipToPlay != null) {
                tauntSource.clip = clipToPlay;
                tauntSource.Play();
            }
        }

        private void ScheduleNextTaunt() {
            nextTauntTime = Time.time + Random.Range(minTauntInterval, maxTauntInterval);
        }

        private void RefillTaunts() {
            if (tauntClips == null) return;
            availableTaunts.Clear();
            availableTaunts.AddRange(tauntClips);
        }
    }
}
