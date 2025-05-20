using System.Collections.Generic;
using System.Linq;
using FoodMatch.Game.Events;
using FoodMatch.Game.Level;
using FoodMatch.Level.Controller;
using FoodMatch.Level.Data;
using UnityEngine;

namespace FoodMatch.Level.Mechanics.Orders
{
    public class OrderManager : InLevelManager
    {
        [SerializeField] private Transform _orderParent;
        [SerializeField] private Transform _orderStartPoint;
        [SerializeField] private float _orderSpacing = 1.0f;
        [SerializeField] private OrderView _orderViewPrefab;
        private Dictionary<OrderModel, OrderView> Orders { get; set; } = new();

        private void Awake()
        {
            GameEvents.ItemCollected += OnItemCollected;
        }

        public override void PrepareLevel(LevelData levelData, ItemDataRepo itemDataRepo)
        {
            base.PrepareLevel(levelData, itemDataRepo);
            var orderData = levelData.Orders;

            foreach (var order in orderData)
            {
                var orderController = Instantiate(_orderViewPrefab, _orderParent);
                var orderModel = new OrderModel(order.ItemType, order.TargetAmount);
                var itemData = ItemDataRepo.GetItemDataByType(order.ItemType);
                var orderObjectPrefab = itemData?.ItemVisualPrefab;
                var orderObject = Instantiate(orderObjectPrefab);
                orderController.Populate(orderModel, orderObject, itemData);
                Orders.Add(orderModel, orderController);
            }

            SetPositionOfOrders();
        }

        private void OnItemCollected(string itemType)
        {
            ProgressOrder(itemType, 1);
        }

        private void ProgressOrder(string itemType, int amount)
        {
            foreach (var orderController in Orders)
            {
                if (orderController.Key.OrderID == itemType)
                {
                    orderController.Key.IncreaseAmount(amount);
                    orderController.Value.UpdateRemainingAmountText();

                    if (orderController.Key.IsCompleted())
                    {
                        Destroy(orderController.Value.gameObject);
                        Orders.Remove(orderController.Key);
                        SetPositionOfOrders();
                        break;
                    }
                }
            }

            var ordersCompleted = Orders.All(x => x.Key.IsCompleted());
            if (ordersCompleted)
            {
                GameEvents.AllOrdersCompleted?.Invoke();
            }
        }

        private void SetPositionOfOrders()
        {
            int index = 0;
            foreach (var orderController in Orders.Values)
            {
                var position = _orderStartPoint.position + new Vector3(index * _orderSpacing, 0, 0);
                orderController.transform.position = position;
                index++;
            }
        }

        public override void Cleanup()
        {
            base.Cleanup();
            foreach (var orderController in Orders.Values)
            {
                if (orderController != null && orderController.gameObject != null)
                {
                    Destroy(orderController.gameObject);
                }
            }

            Orders.Clear();
        }

        private void OnDestroy()
        {
            GameEvents.ItemCollected -= OnItemCollected;
        }
    }
}