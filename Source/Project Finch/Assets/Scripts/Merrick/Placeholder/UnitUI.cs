using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatView {

    public class UnitUI : MonoBehaviour {

        public ActionUnit target = null;
        public Vector3 offset = new Vector3(0, 0.5f, 0);
        private RectTransform canvasRect;
        new private RectTransform transform;
        private RectTransform Healthbar;

        private void Awake() {
            canvasRect = CanvasRefs.canvasRefs.GetComponent<Canvas>().transform as RectTransform;
            transform = GetComponent<RectTransform>();
            Healthbar = transform.Find("Healthbar").GetComponent<RectTransform>();
        }

        private void Update() {
            if (target != null) {
                Vector2 pos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvasRect,
                    Camera.main.WorldToScreenPoint(target.transform.position + offset),
                    Camera.main,
                    out pos);
                transform.localPosition = pos;
                Healthbar.sizeDelta = new Vector2(Mathf.Clamp01((float)target.currentHealth / target.maxHealth) * 100, Healthbar.sizeDelta.y);
            }
            else Destroy(gameObject);
        }
    }
}