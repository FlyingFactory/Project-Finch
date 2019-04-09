using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatView {

    public class CanvasRefs : MonoBehaviour {

        public static CanvasRefs canvasRefs = null;
        
#pragma warning disable 649
        public GameObject fireUI;
        public GameObject EnemyTurnIndicator;
        public GameObject EndTurnButton;
        public TMPro.TextMeshProUGUI hitChanceText;
        public TMPro.TextMeshProUGUI coverText;
        public Color lowCoverTextColor = new Color32(255, 255, 0, 255);
        public Color highCoverTextColor = new Color32(100, 100, 255, 255);
#pragma warning restore 649

        private void Awake() {
            if (canvasRefs != null) Destroy(canvasRefs);
            canvasRefs = this;
        }
    }
}