using FoodMatch.Level.Data;
using TMPro;
using UnityEngine;

namespace FoodMatch.Level.Mechanics.Orders
{
    public class OrderView : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _remainingAmountText;
        [SerializeField] private Transform _objectCreationPoint;
        private OrderModel Order { get; set; }
        private GameObject OrderObject { get; set; }

        public void Populate(OrderModel orderModel, GameObject orderObject, ItemData itemData)
        {
            Order = orderModel;
            PopulateOrderObject(orderObject, itemData);
            UpdateRemainingAmountText();
        }

        private void PopulateOrderObject(GameObject orderObject, ItemData itemData)
        {
            OrderObject = orderObject;
            OrderObject.transform.SetParent(_objectCreationPoint);
            OrderObject.transform.localPosition = itemData.OrderPositionOffset;
            OrderObject.transform.localRotation = Quaternion.Euler(itemData.OrderRotation);
            OrderObject.transform.localScale = itemData.OrderScale;
            OrderObject.GetComponentInChildren<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        public void UpdateRemainingAmountText()
        {
            if (_remainingAmountText != null)
            {
                _remainingAmountText.text = $"{Order.TargetAmount - Order.CurrentAmount}";
            }
        }
    }
}