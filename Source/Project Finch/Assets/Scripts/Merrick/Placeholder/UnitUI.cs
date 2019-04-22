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
        private List<UnityEngine.UI.Image> Arrows = new List<UnityEngine.UI.Image>();
        private UnityEngine.UI.Image ArrowPrefab = null;
        private int LitArrows = 0;
        private Color initColor;

        private void Awake() {
            canvasRect = CanvasRefs.canvasRefs.GetComponent<Canvas>().transform as RectTransform;
            transform = GetComponent<RectTransform>();
            Healthbar = transform.Find("Healthbar").GetComponent<RectTransform>();
        }

        private void Start() {
            ArrowPrefab = Resources.Load<UnityEngine.UI.Image>("Prefabs/RightArrow");
            initColor = Healthbar.GetComponent<UnityEngine.UI.Image>().color;
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

                for (int i = Arrows.Count; i < target.actionsPerTurn; i++) {
                    Arrows.Add(Instantiate(ArrowPrefab, transform));
                    Arrows[i].GetComponent<RectTransform>().anchoredPosition += new Vector2(i * 20, 0);
                    Arrows[i].color = initColor;
                    LitArrows++;
                }
                for (int i = Arrows.Count; i > target.actionsPerTurn; i++) {
                    Destroy(Arrows[i - 1]);
                    Arrows.RemoveAt(i - 1);
                }

                while (LitArrows < target.numActions) {
                    Arrows[LitArrows].color = new Color(Arrows[LitArrows].color.r, Arrows[LitArrows].color.g, Arrows[LitArrows].color.b, 0.375f);
                    LitArrows++;
                }
                while (LitArrows > target.numActions && LitArrows > 0) {
                    LitArrows--;
                    Arrows[LitArrows].color = new Color(Arrows[LitArrows].color.r, Arrows[LitArrows].color.g, Arrows[LitArrows].color.b, 1);
                }
            }
            else Destroy(gameObject);
        }
    }
}