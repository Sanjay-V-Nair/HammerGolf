using System;
using UnityEngine;
using UnityEngine.UI;

namespace HammerGolf.Gameplay {
    public class GameplayManager : MonoBehaviour{
        
        [SerializeField] private GameResultPage gameResultPage;
        [SerializeField] private StartScreenView startScreenView;
        [SerializeField] private TutorialScreen tutorialScreen;
        [SerializeField] private Button quitButton;

        public static bool IsGameActive { get; private set; }

        private void Start() {
            IsGameActive = false;
            GoalView.OnGameWon += OnGameWon;
            StartScreenView.OnStartScreenComplete += HandleStartScreenComplete;
            TutorialScreen.OnTutorialComplete += HandleTutorialComplete;
            BallThrowController.OnBallTimeout += HandleBallTimeout;
            GameResultPage.OnTryAgain += HandleTryAgain;

            SetButton();
            
            if (tutorialScreen != null) tutorialScreen.gameObject.SetActive(false);
            if (gameResultPage != null) gameResultPage.gameObject.SetActive(false);
        }

        private void SetButton() {
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(() => Application.Quit());
        }

        private void OnDestroy() {
            GoalView.OnGameWon -= OnGameWon;
            StartScreenView.OnStartScreenComplete -= HandleStartScreenComplete;
            TutorialScreen.OnTutorialComplete -= HandleTutorialComplete;
            BallThrowController.OnBallTimeout -= HandleBallTimeout;
            GameResultPage.OnTryAgain -= HandleTryAgain;
        }

        private void HandleStartScreenComplete() {
            if (tutorialScreen != null) tutorialScreen.Render();
        }

        private void HandleTutorialComplete() {
            IsGameActive = true;
            if (startScreenView != null) startScreenView.gameObject.SetActive(false);
            if (tutorialScreen != null) tutorialScreen.gameObject.SetActive(false);
            if (gameResultPage != null) gameResultPage.gameObject.SetActive(false);
        }

        private void HandleBallTimeout() {
            IsGameActive = false;
            if (gameResultPage != null) {
                gameResultPage.Render(GameResultPage.ResultType.Loss);
            }
        }

        private void HandleTryAgain() {
            IsGameActive = true;
            var ballController = FindObjectOfType<BallThrowController>();
            if (ballController != null) {
                ballController.ReturnToLastShotSafe();
            }
        }

        private void OnGameWon() {
            IsGameActive = false;
            gameResultPage.Render(GameResultPage.ResultType.Win);
        }
    }
}
