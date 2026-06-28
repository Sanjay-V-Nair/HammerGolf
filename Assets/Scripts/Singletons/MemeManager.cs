using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using HammerGolf.Gameplay;

namespace HammerGolf {
    public class MemeManager : MonoBehaviour {
        public static MemeManager Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private Image memeImage;
        [SerializeField] private List<Sprite> memeSprites;

        private List<Sprite> availableMemes = new List<Sprite>();
        private bool isGameActive = false;

        private void Awake() {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            } else {
                Destroy(gameObject);
                return;
            }

            if (memeImage != null) {
                memeImage.gameObject.SetActive(false);
            }

            RefillMemes();
        }

        private void Start() {
            BallThrowController.OnBallAlmostTimeout += HandleAlmostTimeout;
            BallThrowController.OnBallTimeout += HandleTimeout;
            GoalView.OnGameWon += HandleGameOver;
            TutorialScreen.OnTutorialComplete += HandleGameStart;
            GameResultPage.OnTryAgain += HandleGameStart;
        }

        private void OnDestroy() {
            BallThrowController.OnBallAlmostTimeout -= HandleAlmostTimeout;
            BallThrowController.OnBallTimeout -= HandleTimeout;
            GoalView.OnGameWon -= HandleGameOver;
            TutorialScreen.OnTutorialComplete -= HandleGameStart;
            GameResultPage.OnTryAgain -= HandleGameStart;
        }

        private void HandleGameStart() {
            isGameActive = true;
            if (memeImage != null) {
                memeImage.gameObject.SetActive(false);
            }
        }

        private void HandleGameOver() {
            isGameActive = false;
            if (memeImage != null) {
                memeImage.gameObject.SetActive(false);
            }
        }

        private void HandleTimeout() {
            if (memeImage != null) {
                memeImage.gameObject.SetActive(false);
            }
        }

        private void HandleAlmostTimeout() {
            if (!isGameActive || memeImage == null) return;
            ShowRandomMeme();
        }

        private void ShowRandomMeme() {
            if (memeSprites == null || memeSprites.Count == 0) return;

            if (availableMemes.Count == 0) {
                RefillMemes();
            }

            int randomIndex = Random.Range(0, availableMemes.Count);
            Sprite spriteToDisplay = availableMemes[randomIndex];
            availableMemes.RemoveAt(randomIndex);

            if (spriteToDisplay != null) {
                memeImage.sprite = spriteToDisplay;
                memeImage.gameObject.SetActive(true);

                if (AudioManager.Instance != null) {
                    AudioManager.Instance.PlayBruhSound();
                }

                PlayRandomTween();
            }
        }

        private void PlayRandomTween() {
            RectTransform rect = memeImage.rectTransform;
            rect.DOKill(); // Stop any currently playing tweens
            memeImage.DOKill();
            
            // Reset to defaults
            rect.anchoredPosition = Vector2.zero;
            rect.localScale = Vector3.one;
            rect.localRotation = Quaternion.identity;
            memeImage.color = Color.white;

            Sequence seq = DOTween.Sequence();

            int tweenType = Random.Range(0, 5);
            switch (tweenType) {
                case 0:
                    // Scale and Spin
                    rect.localScale = Vector3.zero;
                    seq.Join(rect.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack));
                    seq.Join(rect.DORotate(new Vector3(0, 0, 720), 0.5f, RotateMode.FastBeyond360).SetEase(Ease.OutBack));
                    break;
                case 1:
                    // Shake
                    seq.Join(rect.DOShakeAnchorPos(1.5f, 150f, 20, 90f));
                    break;
                case 2:
                    // Punch Scale
                    seq.Join(rect.DOPunchScale(new Vector3(0.5f, 0.5f, 0f), 0.5f, 10, 1f));
                    break;
                case 3:
                    // Slide in from bottom
                    rect.anchoredPosition = new Vector2(0, -1500);
                    seq.Join(rect.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutBounce));
                    break;
                case 4:
                    // Fade in while scaling down
                    memeImage.color = new Color(1, 1, 1, 0);
                    rect.localScale = Vector3.one * 3f;
                    seq.Join(memeImage.DOFade(1f, 0.3f));
                    seq.Join(rect.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutCubic));
                    break;
            }

            // Keep the meme on screen for an extra second, then hide it
            seq.AppendInterval(1f);
            seq.OnComplete(() => {
                if (memeImage != null) {
                    memeImage.gameObject.SetActive(false);
                }
            });
        }

        private void RefillMemes() {
            if (memeSprites == null) return;
            availableMemes.Clear();
            availableMemes.AddRange(memeSprites);
        }
    }
}
