using FoodMatch.Level.Mechanics.Items;
using UnityEngine;

namespace FoodMatch.Level.Mechanics.ItemCollection
{
    public class CollectionArea : MonoBehaviour
    {
        public BoardItem Item { get; set; }

        public void SetItem(BoardItem item)
        {
            Item = item;
        }

        public void RemoveItem(BoardItem item)
        {
            if (item == Item)
            {
                Item = null;
            }
        }

        public bool IsOccupied()
        {
            return Item != null;
        }
    }
}