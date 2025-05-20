using UnityEngine;

namespace FoodMatch.Level.Data
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "ItemData", order = 1)]
    public class ItemData : ScriptableObject
    {
        [SerializeField] private string _itemType;
        [SerializeField] private GameObject _itemVisualPrefab;

        [SerializeField] private Vector3 _defaultScale;

        [SerializeField] private Vector3 _collectedScale;
        [SerializeField] private Vector3 _collectedRotation;
        [SerializeField] private Vector3 _collectedPositionOffset;

        [SerializeField] private Vector3 _orderScale;
        [SerializeField] private Vector3 _orderRotation;
        [SerializeField] private Vector3 _orderPositionOffset;

        public string ItemType => _itemType;

        public GameObject ItemVisualPrefab => _itemVisualPrefab;

        public Vector3 DefaultScale => _defaultScale;

        public Vector3 CollectedScale => _collectedScale;

        public Vector3 OrderScale => _orderScale;

        public Vector3 CollectedRotation => _collectedRotation;
        public Vector3 OrderRotation => _orderRotation;

        public Vector3 CollectedPositionOffset => _collectedPositionOffset;

        public Vector3 OrderPositionOffset => _orderPositionOffset;
    }
}