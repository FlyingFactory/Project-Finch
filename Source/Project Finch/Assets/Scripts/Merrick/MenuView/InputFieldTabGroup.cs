using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MenuView {

    public class InputFieldTabGroup : MonoBehaviour {

        [SerializeField] private List<UnityEngine.UI.InputField> Fields = new List<UnityEngine.UI.InputField>();
        [SerializeField] private UnityEngine.UI.Button EnterButton = null;

        private void Start() {
            if (Fields.Count == 0) Destroy(this);
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Tab)) {
                for (int i = 0; i < Fields.Count; i++) {
                    if (Fields[i].isFocused) {
                        Fields[i].DeactivateInputField();
                        Fields[(i + 1) % Fields.Count].ActivateInputField();
                        break;
                    }
                }
            }
            if (EnterButton != null && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))) {

                EnterButton.onClick.Invoke();
            }
        }
    }
}