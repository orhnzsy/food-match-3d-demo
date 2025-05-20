using System.Collections.Generic;
using System.Linq;
using FoodMatch.Game.Level;
using FoodMatch.Level.Controller;
using FoodMatch.Level.Data;
using FoodMatch.Level.Mechanics.Items;
using UnityEngine;

namespace FoodMatch.Level.Mechanics.Board
{
    public class BoardManager : InLevelManager
    {
        [SerializeField] private Transform _boardParent;
        [SerializeField] private BoardItem _boardItemRootPrefab;
        [SerializeField] private Transform _leftBottomCorner;
        [SerializeField] private Transform _rightTopCorner;

        [Header("Grid Settings")]
        [SerializeField] private float randomOffsetRange = 0.3f;
        [SerializeField] private Vector2Int maxGridDimensions;
        [SerializeField] private float layerHeight = 1f;
        [SerializeField] private float randomHeightOffset = 0.1f;

        private List<BoardItem> _boardItems = new List<BoardItem>();

        public override void PrepareLevel(LevelData levelData, ItemDataRepo itemDataRepo)
        {
            base.PrepareLevel(levelData, itemDataRepo);
            PlaceObjectsInGrid(_rightTopCorner.position, _leftBottomCorner.position, levelData.Items.ToList());
        }

        private void PlaceObjectsInGrid(Vector3 rightTopCorner, Vector3 leftBottomCorner, List<string> itemTypes)
        {
            var areaSize = rightTopCorner - leftBottomCorner;

            int numberOfObjects = itemTypes.Count;

            var gridDimensions = CalculateOptimalGridDimensions(numberOfObjects, new Vector2(areaSize.x, areaSize.z));

            gridDimensions.x = Mathf.Min(gridDimensions.x, maxGridDimensions.x);
            gridDimensions.y = Mathf.Min(gridDimensions.y, maxGridDimensions.y);

            // Calculate how many objects fit in a single layer
            int objectsPerLayer = gridDimensions.x * gridDimensions.y;

            // Calculate total number of layers needed
            int totalLayers = Mathf.CeilToInt((float)numberOfObjects / objectsPerLayer);

            var cellSize = new Vector2(areaSize.x / gridDimensions.x, areaSize.z / gridDimensions.y);

            int objectsPlaced = 0;

            for (int layer = 0; layer < totalLayers && objectsPlaced < numberOfObjects; layer++)
            {
                float layerBaseHeight = leftBottomCorner.y + (layer * layerHeight);

                for (int y = 0; y < gridDimensions.y && objectsPlaced < numberOfObjects; y++)
                {
                    for (int x = 0; x < gridDimensions.x && objectsPlaced < numberOfObjects; x++)
                    {
                        var basePosition = new Vector3(leftBottomCorner.x + cellSize.x * (x + 0.5f), layerBaseHeight, leftBottomCorner.z + cellSize.y * (y + 0.5f));
                        var randomOffset = new Vector3(Random.Range(-randomOffsetRange, randomOffsetRange) * cellSize.x, Random.Range(0, randomHeightOffset), Random.Range(-randomOffsetRange, randomOffsetRange) * cellSize.y);
                        var finalPosition = basePosition + randomOffset;
                        finalPosition.x = Mathf.Clamp(finalPosition.x, leftBottomCorner.x, rightTopCorner.x);
                        finalPosition.z = Mathf.Clamp(finalPosition.z, leftBottomCorner.z, rightTopCorner.z);
                        var boardItem = Instantiate(_boardItemRootPrefab, _boardParent);
                        int randomIndex = Random.Range(0, itemTypes.Count);
                        string itemType = itemTypes[randomIndex];
                        itemTypes.RemoveAt(randomIndex);
                        var itemData = ItemDataRepo.GetItemDataByType(itemType);
                        var visualPrefab = Instantiate(itemData.ItemVisualPrefab, boardItem.transform);
                        boardItem.Populate(itemData, visualPrefab);
                        boardItem.transform.position = finalPosition;
                        boardItem.transform.rotation = Random.rotation;
                        _boardItems.Add(boardItem);
                        objectsPlaced++;
                    }
                }
            }
        }

        private Vector2Int CalculateOptimalGridDimensions(int objectCount, Vector2 area)
        {
            var targetRatio = area.x / area.y;
            var bestX = 1;
            var bestY = objectCount;
            var bestRatioDifference = Mathf.Abs(targetRatio - (float)bestX / bestY);

            for (int x = 1; x <= objectCount; x++)
            {
                var y = Mathf.CeilToInt((float)objectCount / x);

                if (x > maxGridDimensions.x || y > maxGridDimensions.y)
                {
                    continue;
                }

                var ratio = (float)x / y;
                var ratioDifference = Mathf.Abs(targetRatio - ratio);

                if (ratioDifference < bestRatioDifference)
                {
                    bestRatioDifference = ratioDifference;
                    bestX = x;
                    bestY = y;
                }
            }

            if (bestX > maxGridDimensions.x || bestY > maxGridDimensions.y)
            {
                bestX = maxGridDimensions.x;
                bestY = maxGridDimensions.y;
            }

            return new Vector2Int(bestX, bestY);
        }

        public override void Cleanup()
        {
            base.Cleanup();
            foreach (var item in _boardItems)
            {
                if (item != null && item.gameObject != null)
                {
                    Destroy(item.gameObject);
                }
            }

            _boardItems.Clear();
        }
    }
}