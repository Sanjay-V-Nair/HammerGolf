using UnityEngine;
using UnityEngine.UI;

namespace HammerGolf {
    public class GameResultPage : MonoBehaviour {

        public static event System.Action OnTryAgain;

        public enum ResultType {
            Win,
            Loss,
        }

        [SerializeField] private GameObject winState;
        [SerializeField] private GameObject failState;
        [SerializeField] private Button tryAgainButton;

        public void Render(ResultType result) {
            Reset();
            SetState(result);
            gameObject.SetActive(true);
        }

        private void SetState(ResultType result) {
            switch (result) {
                case ResultType.Win:
                    winState.SetActive(true);
                    break;
                case ResultType.Loss:
                    failState.SetActive(true);
                    SetButton();
                    break;
            }
        }

        private void SetButton() {
            tryAgainButton.onClick.RemoveAllListeners();
            tryAgainButton.onClick.AddListener(OnTryAgainClicked);
        }

        private void OnTryAgainClicked() {
            Reset();
            gameObject.SetActive(false);
            OnTryAgain?.Invoke();
        }

        private void Reset() {
            winState.SetActive(false);
            failState.SetActive(false);
        }
    }
}
