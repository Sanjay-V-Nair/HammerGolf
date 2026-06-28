using System.Collections;
using UnityEngine;

namespace HammerGolf
{
    public class StartScreenView : MonoBehaviour {

        public static event System.Action OnStartScreenComplete;

        private void OnEnable() {
            StartCoroutine(WaitAndComplete());
        }

        private IEnumerator WaitAndComplete() {
            yield return new WaitForSeconds(2f);
            OnStartScreenComplete?.Invoke();
            Hide();
        }

        public void Hide() {
            gameObject.SetActive(false);
        }
        
    }
}
