using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FoodMatch.Game.Events;
using FoodMatch.Level.Controller;
using FoodMatch.Level.Mechanics.Items;
using UnityEngine;

namespace FoodMatch.Level.Mechanics.ItemCollection
{
    public class ItemCollectionManager : InLevelManager
    {
        [SerializeField] private Camera _selectionCamera;
        [SerializeField] private CollectionArea[] _collectionAreas;
        [SerializeField] private ParticleSystem _collectParticle;


        public override void OnUpdate()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var ray = _selectionCamera.ScreenPointToRay(Input.mousePosition);

                if (!Physics.Raycast(ray, out var hit))
                {
                    return;
                }

                var item = hit.transform.GetComponent<BoardItem>();
                if (item == null)
                {
                    return;
                }

                OnItemSelected(item);
            }
        }

        public void OnItemSelected(BoardItem item)
        {
            if (!TryFindCollectionAreaIndexForItem(item, out var index))
            {
                return;
            }

            for (int i = _collectionAreas.Length - 2; i >= index; i--)
            {
                var collectionArea = _collectionAreas[i];
                var targetCollectionArea = _collectionAreas[i + 1];
                if (collectionArea.IsOccupied())
                {
                    var targetItem = collectionArea.Item;
                    collectionArea.RemoveItem(targetItem);
                    targetCollectionArea.SetItem(targetItem);
                    targetItem.SwitchToAnotherCollectionArea(targetCollectionArea.transform.position);
                }
            }

            var area = _collectionAreas[index];
            area.SetItem(item);
            item.PrepareForCollection();

            item.MoveToCollectionArea(area, ItemMoveCompleted);
        }

        private void ItemMoveCompleted(BoardItem item)
        {
            CheckMatchAndRearrangeCollectionArea();
            GameEvents.ItemCollected?.Invoke(item.ItemData.ItemType);
        }

        private void CheckMatchAndRearrangeCollectionArea()
        {
            bool matchFound = false;

            for (int i = 0; i < _collectionAreas.Length - 2; i++)
            {
                var collectionAreaList = new List<CollectionArea>();

                collectionAreaList.Add(_collectionAreas[i]);
                collectionAreaList.Add(_collectionAreas[i + 1]);
                collectionAreaList.Add(_collectionAreas[i + 2]);

                if (collectionAreaList.Any(x => !x.IsOccupied()))
                {
                    continue;
                }

                if (collectionAreaList.Any(x => x.Item.State != BoardItemState.InCollectionArea))
                {
                    continue;
                }

                var itemList = new List<BoardItem>();
                collectionAreaList.ForEach(x => itemList.Add(x.Item));

                string itemType = collectionAreaList[0].Item.ItemData.ItemType;

                if (collectionAreaList.All(x => x.Item.ItemData.ItemType == itemType))
                {
                    matchFound = true;
                    StartCoroutine(MatchObjects(itemList, collectionAreaList));
                    break;
                }
            }

            if (!matchFound && _collectionAreas.All(x => x.IsOccupied() && x.Item.State == BoardItemState.InCollectionArea))
            {
                GameEvents.ThereIsNotEnoughSpace?.Invoke();
            }
        }

        private IEnumerator MatchObjects(List<BoardItem> itemList, List<CollectionArea> collectionAreaList)
        {
            var heightOffset = new Vector3(0, 0, 0.2f);
            var leftInitialPosition = itemList[0].transform.position;
            var centerInitialPosition = itemList[1].transform.position;
            var rightInitialPosition = itemList[2].transform.position;
            var centerInitialScale = itemList[1].transform.localScale;
            var leftPosition = collectionAreaList[0].transform.position + heightOffset;
            var centerPosition = collectionAreaList[1].transform.position + heightOffset;
            var rightPosition = collectionAreaList[2].transform.position + heightOffset;
            collectionAreaList.ForEach(x => x.Item.OnItemMatched());
            collectionAreaList.ForEach(x => x.SetItem(null));

            float duration = 0.2f;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;
                elapsedTime += Time.deltaTime;
                itemList[0].transform.position = Vector3.Lerp(leftInitialPosition, leftPosition, t);
                itemList[1].transform.position = Vector3.Lerp(centerInitialPosition, centerPosition, t);
                itemList[2].transform.position = Vector3.Lerp(rightInitialPosition, rightPosition, t);
                itemList[1].transform.localScale = Vector3.Lerp(centerInitialScale, centerInitialScale * 1.2f, t);
                yield return null;
            }

            elapsedTime = 0f;
            duration = 0.15f;

            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;
                elapsedTime += Time.deltaTime;

                itemList[0].transform.position = Vector3.Lerp(leftPosition, centerPosition, t);
                itemList[2].transform.position = Vector3.Lerp(rightPosition, centerPosition, t);
                yield return null;
            }

            itemList.ForEach(x => Destroy(x.gameObject));
            Instantiate(_collectParticle, centerPosition, Quaternion.identity);
            GameEvents.MatchOccured?.Invoke();
            FIllEmptyAreas();
        }

        private void FIllEmptyAreas()
        {
            for (int i = 0; i < _collectionAreas.Length - 1; i++)
            {
                var collectionArea = _collectionAreas[i];
                if (!collectionArea.IsOccupied())
                {
                    for (int j = i + 1; j < _collectionAreas.Length; j++)
                    {
                        var nextCollectionArea = _collectionAreas[j];
                        if (nextCollectionArea.IsOccupied())
                        {
                            var item = nextCollectionArea.Item;
                            nextCollectionArea.RemoveItem(item);
                            collectionArea.SetItem(item);
                            item.SwitchToAnotherCollectionArea(collectionArea.transform.position);
                            break;
                        }
                    }
                }
            }
        }


        public bool TryFindCollectionAreaIndexForItem(BoardItem item, out int index)
        {
            index = -1;
            var itemType = item.ItemData.ItemType;
            bool itemAlreadyExist = false;

            bool isThereAPlace = _collectionAreas.Any(x => !x.IsOccupied());

            if (!isThereAPlace)
            {
                return false;
            }

            for (int i = 0; i < _collectionAreas.Length - 1; i++)
            {
                if (_collectionAreas[i].IsOccupied())
                {
                    var placedItemType = _collectionAreas[i].Item.ItemData.ItemType;
                    if (itemType.Equals(placedItemType))
                    {
                        index = i + 1;
                        itemAlreadyExist = true;
                    }
                }
            }

            if (!itemAlreadyExist)
            {
                index = _collectionAreas.ToList().FindIndex(x => !x.IsOccupied());
            }

            if (index == -1)
            {
                return false;
            }

            return true;
        }

        public override void Cleanup()
        {
            base.Cleanup();
            foreach (var collectionArea in _collectionAreas)
            {
                if (collectionArea != null && collectionArea.Item != null)
                {
                    collectionArea.SetItem(null);
                }
            }
        }
    }
}