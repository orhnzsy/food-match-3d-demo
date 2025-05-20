using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FoodMatch.Level.Data
{
    [CreateAssetMenu(menuName = "Food Match/Item Data Repo")]
    public class ItemDataRepo : ScriptableObject
    {
        [SerializeField] private List<ItemData> _itemData;

        public ItemData GetItemDataByType(string type)
        {
            return _itemData.FirstOrDefault(x => x.ItemType == type);
        }
    }
}