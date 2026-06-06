using UnityEngine;

namespace SnapToMapTD.Towers
{
    public class HolyBoltEffect : MonoBehaviour
    {
        [SerializeField] private float duration = 0.5f;

        private void Start()
        {
            Destroy(gameObject, duration);
        }
    }
}
