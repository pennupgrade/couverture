using UnityEngine;

public class AnimalStation : MonoBehaviour
{
    [SerializeField] public SaveStateManager.CharacterOption characterToUnlock;

    private bool inRange;

    private void Start() { }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            inRange = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player") {
            inRange = false;
        }
    }

    private void Update() {
        if (inRange && Input.GetKeyDown(KeyCode.E)) {
            SaveStateManagerGameObject.UnlockCharacter(characterToUnlock);
            UIManager.Instance.Update_CatSelectionPanel_DuringGame();
            UIManager.Instance.Open_CatSelectionPanel_DuringGame();
        }
    }
}