using UnityEngine;
using UnityEngine.UI;

namespace SnapToMapTD.UI
{
    public class DebugButtonBorder : MonoBehaviour
    {
        [SerializeField] private Color color = Color.red;
        [SerializeField] private float thickness = 3f;

        private void Awake()
        {
            string[] sides = { "Top", "Bottom", "Left", "Right" };
            foreach (var side in sides)
            {
                var go = new GameObject("Border_" + side);
                go.transform.SetParent(transform, false);

                var img = go.AddComponent<Image>();
                img.color = color;
                img.raycastTarget = false;

                var rt = go.GetComponent<RectTransform>();
                switch (side)
                {
                    case "Top":
                        rt.anchorMin = new Vector2(0, 1);
                        rt.anchorMax = new Vector2(1, 1);
                        rt.offsetMin = new Vector2(0, -thickness);
                        rt.offsetMax = Vector2.zero;
                        break;
                    case "Bottom":
                        rt.anchorMin = new Vector2(0, 0);
                        rt.anchorMax = new Vector2(1, 0);
                        rt.offsetMin = Vector2.zero;
                        rt.offsetMax = new Vector2(0, thickness);
                        break;
                    case "Left":
                        rt.anchorMin = new Vector2(0, 0);
                        rt.anchorMax = new Vector2(0, 1);
                        rt.offsetMin = Vector2.zero;
                        rt.offsetMax = new Vector2(thickness, 0);
                        break;
                    case "Right":
                        rt.anchorMin = new Vector2(1, 0);
                        rt.anchorMax = new Vector2(1, 1);
                        rt.offsetMin = new Vector2(-thickness, 0);
                        rt.offsetMax = Vector2.zero;
                        break;
                }
            }
        }
    }
}
