using UnityEngine;

namespace SnapToMapTD.Towers
{
    [CreateAssetMenu(fileName = "TowerData", menuName = "SnapToMapTD/Tower Data")]
    public class TowerData : ScriptableObject
    {
        public string towerName;
        public GameObject prefab;
        public Sprite icon;
        public int cost;
        [TextArea(2, 4)] public string description = "";
        [TextArea(2, 4)] public string skillDescription = "";
    }
}
