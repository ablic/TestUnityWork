using AxGrid;
using AxGrid.Base;
using AxGrid.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace TestTaskSolution
{
    public class SlotReel : MonoBehaviourExtBind
    {
        [Header("Setup")]
        [SerializeField] private Image itemPrefab;
        [SerializeField] private Sprite[] itemSprites;
        [SerializeField] private Transform itemContainer;
        [SerializeField] private float gapBetweenItems = 15f;

        [Header("Moving")]
        [Tooltip("Reel speed, measured in cells per second. May be negative")]
        [SerializeField] 
        private float maxSpeed = 20f;

        [Min(0f)]
        [SerializeField] 
        private float accelerationDuration = 3f;

        [Min(0f)]
        [SerializeField] 
        private float brakingDuration = 3f;

        [Min(0f)]
        [SerializeField]
        private float alignDuration = 2f;

        [SerializeField] private AnimationCurve accelerationSpeedCurve;
        [SerializeField] private AnimationCurve brakingSpeedCurve;
        [SerializeField] private AnimationCurve alignItemPosCurve;

        private LinkedList<RectTransform> items = new();
        private float cellSize;
        private float halfReelHeight;

        private float currentOffset;
        private float speed;
        private Coroutine reelSpeedChangingRoutine;
        private Coroutine reelAlignRoutine;

        [OnStart]
        private void Init()
        {
            for (int i = 0; i < itemSprites.Length; i++)
            {
                var reelItem = Instantiate(itemPrefab, itemContainer);
                reelItem.sprite = itemSprites[i];
                items.AddLast(reelItem.rectTransform);
            }

            cellSize = itemPrefab.rectTransform.rect.height + gapBetweenItems;
            int count = items.Count;
            halfReelHeight = (count % 2 != 0 ? count : count - 1) * cellSize * .5f;

            RebuildReel();
        }

        [Bind("StartSpin")]
        private void StartReel()
        {
            if (reelSpeedChangingRoutine != null)
                StopCoroutine(reelSpeedChangingRoutine);

            StartCoroutine(
                ChangeReelSpeed(maxSpeed, accelerationDuration, accelerationSpeedCurve, "ReelStarted")
                );
        }

        [Bind("StopSpin")]
        private void StopReel()
        {
            if (reelSpeedChangingRoutine != null)
                StopCoroutine(reelSpeedChangingRoutine);

            StartCoroutine(
                ChangeReelSpeed(0f, brakingDuration, brakingSpeedCurve, "ReelStopped")
                );
        }

        [Bind("Align")]
        private void Align()
        {
            if (reelAlignRoutine != null)
                StopCoroutine(reelAlignRoutine);

            float offset = items.ElementAt((items.Count - 1) / 2).anchoredPosition.y;

            reelAlignRoutine = StartCoroutine(
                AlignReel(offset)
                );
        }

        [OnUpdate]
        private void MoveItems()
        {
            float delta = speed * cellSize * Time.deltaTime;

            currentOffset += delta;

            foreach (var item in items)
            {
                item.anchoredPosition -= new Vector2(0, delta);
            }

            if (speed > 0)
            {
                if (currentOffset > cellSize)
                {
                    currentOffset = 0f;
                    items.AddLast(items.First.Value);
                    items.RemoveFirst();
                    RebuildReel();
                }
            }
            else
            {
                if (currentOffset < -cellSize)
                {
                    currentOffset = 0f;
                    items.AddFirst(items.Last.Value);
                    items.RemoveLast();
                    RebuildReel();
                }
            }
        }

        private void RebuildReel()
        {
            int i = 0;
            foreach (var item in items)
            {
                item.anchoredPosition = new Vector2(0f, cellSize * i - halfReelHeight + cellSize * .5f);
                i++;
            }
        }

        private IEnumerator ChangeReelSpeed(float targetSpeed, float duration, AnimationCurve curve, string onCompleteFsmActionName)
        {
            float time = 0f;
            float initialSpeed = speed;

            while (time < duration)
            {
                time += Time.deltaTime;
                float t = Mathf.Clamp01(time / duration);
                speed = Mathf.LerpUnclamped(initialSpeed, targetSpeed, curve.Evaluate(t));
                yield return null;
            }

            speed = targetSpeed;
            reelSpeedChangingRoutine = null;

            Settings.Fsm.Invoke(onCompleteFsmActionName);
        }

        private IEnumerator AlignReel(float offset)
        {
            Vector2[] startPositions = new Vector2[items.Count];

            {
                int i = 0;
                foreach (var item in items)
                    startPositions[i++] = item.anchoredPosition;
            }
            
            float time = 0f;

            while (time < alignDuration)
            {
                time += Time.deltaTime;

                float t = Mathf.Clamp01(time / alignDuration);
                float currentOffset = Mathf.Lerp(0f, offset, alignItemPosCurve.Evaluate(t));

                int i = 0;
                foreach (var item in items)
                    item.anchoredPosition = startPositions[i++] + Vector2.down * currentOffset;

                yield return null;
            }

            RebuildReel();
            currentOffset = 0f;
            reelAlignRoutine = null;

            Settings.Fsm.Invoke("ReelAligned");
        }
    }
}