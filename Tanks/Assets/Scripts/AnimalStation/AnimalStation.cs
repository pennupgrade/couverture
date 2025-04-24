using UnityEngine;

public class AnimalStation : MonoBehaviour
{
    [SerializeField] public SaveStateManager.CharacterOption characterToUnlock;

    private bool inRange;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            inRange = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            inRange = false;
        }
    }

    private void Update() {
        // Prevent opening again if already open
        if (UIManager.Instance.Cat_Selection_Panel.activeInHierarchy) return;

        if (inRange && Input.GetKeyDown(KeyCode.E)) {
            SaveStateManagerGameObject.UnlockCharacter(characterToUnlock);
            UIManager.Instance.Update_CatSelectionPanel_DuringGame();
            UIManager.Instance.Open_CatSelectionPanel_DuringGame();
        }
    }
}