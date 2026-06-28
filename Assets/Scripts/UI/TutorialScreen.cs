using UnityEngine;
using UnityEngine.UI;

namespace HammerGolf {
    public class TutorialScreen : MonoBehaviour {

        public static event System.Action OnTutorialComplete;

        [SerializeField] private GameObject firstStep;
        [SerializeField] private GameObject secondStep;
        [SerializeField] private GameObject thirdStep;
        [SerializeField] private Button continueButton;
        
        private int currentStep = 0;

        public void Render() {
            gameObject.SetActive(true);
            currentStep = 0;
            ShowStep(currentStep);
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(OnContinueClicked);
        }

        private void OnContinueClicked() {
            currentStep++;
            if (currentStep > 2) {
                OnTutorialComplete?.Invoke();
                Hide();
            } else {
                ShowStep(currentStep);
            }
        }

        private void ShowStep(int step) {
            if (firstStep != null) firstStep.SetActive(step == 0);
            if (secondStep != null) secondStep.SetActive(step == 1);
            if (thirdStep != null) thirdStep.SetActive(step == 2);
        }

        public void Hide() {
            gameObject.SetActive(false);
        }
    }
}
