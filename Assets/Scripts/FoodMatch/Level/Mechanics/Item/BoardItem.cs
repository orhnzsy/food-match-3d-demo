using System;
using System.Collections;
using FoodMatch.Level.Data;
using FoodMatch.Level.Mechanics.ItemCollection;
using UnityEngine;
using UnityEngine.Serialization;

namespace FoodMatch.Level.Mechanics.Items
{
    public class BoardItem : MonoBehaviour
    {
        public ItemData ItemData { get; private set; }
        private GameObject VisualObject { get; set; }
        public BoardItemState State { get; private set; } = BoardItemState.InBoard;

        [Range(0.1f, 10f)]
        [SerializeField] private float _moveSpeed = 2f;
        [Range(0.5f, 5f)]
        [SerializeField] private float _arcHeight = 2f; // Controls the height of the arc

        //Moving specific variables
        private Vector3 StartPoint { get; set; }
        private Vector3 ControlPoint { get; set; } //Control point for bezier movement
        private Vector3 EndPoint { get; set; }
        private Vector3 EndRotation { get; set; }
        private Vector3 EndScale { get; set; }
        private bool IsMoving { get; set; }
        private float CurrentPositionOnCurve { get; set; }


        private Coroutine _moveCoroutine;
        private Coroutine _switchCollectionAreaCoroutine;

        public CollectionArea TargetCollectionArea { get; private set; }

        public void Populate(ItemData itemData, GameObject visualObject)
        {
            ItemData = itemData;
            VisualObject = visualObject;
            VisualObject.transform.localScale = itemData.DefaultScale;
        }

        public void MoveToCollectionArea(CollectionArea collectionArea, Action<BoardItem> onAnimationCompleted)
        {
            TargetCollectionArea = collectionArea;
            StartPoint = transform.position;
            EndPoint = collectionArea.transform.position + ItemData.CollectedPositionOffset;
            EndRotation = ItemData.CollectedRotation;
            EndScale = ItemData.CollectedScale;
            CurrentPositionOnCurve = 0;
            CalculateControlPoint();
            BeginMovement(onAnimationCompleted);
        }

        //In the case of target change (It can happen when you spam collect things)
        public void UpdateEndPoint(Vector3 newEnd)
        {
            if (!IsMoving) return;

            if (Vector3.Distance(EndPoint, newEnd) > 0.01f)
            {
                EndPoint = newEnd;
            }
        }

        //Finding to make it look like the object being picked instead of just moving to target
        private void CalculateControlPoint()
        {
            var offset = new Vector3(0f, 0f, 3f);
            var midPoint = (StartPoint + EndPoint) / 2f + offset;
            midPoint.z = StartPoint.z;
            ControlPoint = midPoint + Vector3.up * _arcHeight;
        }

        private void BeginMovement(Action<BoardItem> onAnimationCompleted)
        {
            if (IsMoving)
            {
                StopCoroutine(_moveCoroutine);
            }

            _moveCoroutine = StartCoroutine(MoveToCollectionArea(onAnimationCompleted));
        }

        //Fancy way of moving object along bezier to collection area
        private IEnumerator MoveToCollectionArea(Action<BoardItem> onAnimationCompleted)
        {
            State = BoardItemState.CollectAnimationPlaying;
            IsMoving = true;

            var initialScale = transform.localScale;
            var initialRotation = transform.eulerAngles;

            while (CurrentPositionOnCurve < 1)
            {
                CurrentPositionOnCurve += Time.deltaTime * _moveSpeed;
                CurrentPositionOnCurve = Mathf.Clamp01(CurrentPositionOnCurve);

                if (CurrentPositionOnCurve < 0.99f)
                {
                    transform.position = CalculateQuadraticBezierPoint(CurrentPositionOnCurve, StartPoint, ControlPoint, EndPoint);
                    transform.localScale = Vector3.Lerp(initialScale, EndScale, CurrentPositionOnCurve);
                    transform.eulerAngles = Vector3.Lerp(initialRotation, EndRotation, CurrentPositionOnCurve);
                }

                yield return null;
            }

            transform.localScale = EndScale;
            transform.eulerAngles = EndRotation;
            transform.position = EndPoint;
            IsMoving = false;

            State = BoardItemState.InCollectionArea;

            onAnimationCompleted?.Invoke(this);
        }

        // Quadratic Bezier curve formula. I took it from AI.
        private Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;

            Vector3 point = uu * p0; // (1-t)²P₀
            point += 2 * u * t * p1; // 2(1-t)tP₁
            point += tt * p2; // t²P₂

            return point;
        }

        //Moving objects between collection areas because of ordering
        public void SwitchToAnotherCollectionArea(Vector3 targetAreaPosition)
        {
            //If the object we are trying to move still in the animation of moving to collection area, we are gonna change the target of it
            if (State == BoardItemState.CollectAnimationPlaying)
            {
                UpdateEndPoint(targetAreaPosition);
            }
            else if (State == BoardItemState.InCollectionArea)
            {
                if (_switchCollectionAreaCoroutine != null)
                {
                    StopCoroutine(_switchCollectionAreaCoroutine);
                    _switchCollectionAreaCoroutine = null;
                }

                _switchCollectionAreaCoroutine = StartCoroutine(SwitchToAnotherCollectionAreaCoroutine(targetAreaPosition));
            }
        }

        private IEnumerator SwitchToAnotherCollectionAreaCoroutine(Vector3 targetPosition)
        {
            State = BoardItemState.SwitchingPlacesInCollectionArea;
            IsMoving = true;
            while (Vector3.Distance(targetPosition, transform.position) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, _moveSpeed * Time.deltaTime * 5f);
                yield return null;
            }

            IsMoving = false;
            transform.position = targetPosition;
            State = BoardItemState.InCollectionArea;
        }

        public void OnItemMatched()
        {
            State = BoardItemState.Matched;
        }

        public void PrepareForCollection()
        {
            //Making rigidbody kinematic so it can move as transform object
            GetComponent<Rigidbody>().isKinematic = true;

            //Disabling all collider interactions
            GetComponentInChildren<Collider>().enabled = false;
        }
    }

    public enum BoardItemState
    {
        InBoard,
        CollectAnimationPlaying,
        InCollectionArea,
        SwitchingPlacesInCollectionArea,
        Matched,
    }
}