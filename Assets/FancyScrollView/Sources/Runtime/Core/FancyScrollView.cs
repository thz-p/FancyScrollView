﻿/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

using System.Collections.Generic;
using UnityEngine;

namespace FancyScrollView
{
    /// <summary>
    /// 用于实现滚动视图的抽象基类.
    /// 支持无限滚动和捕捉功能.
    /// 如果不需要 <see cref="FancyScrollView{TItemData, TContext}.Context"/>，则使用 <see cref="FancyScrollView{TItemData}"/> 代替.
    /// </summary>
    /// <typeparam name="TItemData">项目数据的类型.</typeparam>
    /// <typeparam name="TContext"><see cref="Context"/> 的类型.</typeparam>
    public abstract class FancyScrollView<TItemData, TContext> : MonoBehaviour where TContext : class, new()
    {
        /// <summary>
        /// 单元格之间的间隔.
        /// </summary>
        [SerializeField, Range(1e-2f, 1f)] protected float cellInterval = 0.2f;

        /// <summary>
        /// 滚动位置的基准.
        /// </summary>
        /// <remarks>
        /// 例如，指定 <c>0.5</c> 时，如果滚动位置为 <c>0</c>，则第一个单元格将位于中心.
        /// </remarks>
        [SerializeField, Range(0f, 1f)] protected float scrollOffset = 0.5f;

        /// <summary>
        /// 是否循环布置单元格.
        /// </summary>
        /// <remarks>
        /// 将其设置为 <c>true</c> 将使最后一个单元格之后出现第一个单元格，第一个单元格之前出现最后一个单元格.
        /// 当实现无限滚动时，请指定 <c>true</c>.
        /// </remarks>
        [SerializeField] protected bool loop = false;

        /// <summary>
        /// 单元格的父级 <c>Transform</c>.
        /// </summary>
        [SerializeField] protected Transform cellContainer = default;

        readonly IList<FancyCell<TItemData, TContext>> pool = new List<FancyCell<TItemData, TContext>>();

        /// <summary>
        /// 表示是否已经完成初始化.
        /// </summary>
        protected bool initialized;

        /// <summary>
        /// 表示当前的滚动位置.
        /// </summary>
        protected float currentPosition;

        /// <summary>
        /// 单元格的预制体 GameObject.
        /// </summary>
        protected abstract GameObject CellPrefab { get; }

        /// <summary>
        /// 项目数据的列表.
        /// </summary>
        protected IList<TItemData> ItemsSource { get; set; } = new List<TItemData>();

        /// <summary>
        /// <typeparamref name="TContext"/> 的实例.
        /// 该实例在单元格和滚动视图之间共享，用于传递信息和保持状态.
        /// </summary>
        protected TContext Context { get; } = new TContext();

        /// <summary>
        /// 初期化を行います.
        /// </summary>
        /// <remarks>
        /// 最初にセルが生成される直前に呼び出されます.
        /// </remarks>
        protected virtual void Initialize() { }

        /// <summary>
        /// 渡されたアイテム一覧に基づいて表示内容を更新します.
        /// </summary>
        /// <param name="itemsSource">アイテム一覧.</param>
        protected virtual void UpdateContents(IList<TItemData> itemsSource)
        {
            ItemsSource = itemsSource;
            Refresh();
        }

        /// <summary>
        /// セルのレイアウトを強制的に更新します.
        /// </summary>
        protected virtual void Relayout() => UpdatePosition(currentPosition, false);

        /// <summary>
        /// セルのレイアウトと表示内容を強制的に更新します.
        /// </summary>
        protected virtual void Refresh() => UpdatePosition(currentPosition, true);

        /// <summary>
        /// スクロール位置を更新します.
        /// </summary>
        /// <param name="position">スクロール位置.</param>
        protected virtual void UpdatePosition(float position) => UpdatePosition(position, false);

        void UpdatePosition(float position, bool forceRefresh)
        {
            if (!initialized)
            {
                Initialize();
                initialized = true;
            }

            currentPosition = position;

            var p = position - scrollOffset / cellInterval;
            var firstIndex = Mathf.CeilToInt(p);
            var firstPosition = (Mathf.Ceil(p) - p) * cellInterval;

            if (firstPosition + pool.Count * cellInterval < 1f)
            {
                ResizePool(firstPosition);
            }

            UpdateCells(firstPosition, firstIndex, forceRefresh);
        }

        void ResizePool(float firstPosition)
        {
            Debug.Assert(CellPrefab != null);
            Debug.Assert(cellContainer != null);

            var addCount = Mathf.CeilToInt((1f - firstPosition) / cellInterval) - pool.Count;
            for (var i = 0; i < addCount; i++)
            {
                var cell = Instantiate(CellPrefab, cellContainer).GetComponent<FancyCell<TItemData, TContext>>();
                if (cell == null)
                {
                    throw new MissingComponentException(string.Format(
                        "FancyCell<{0}, {1}> component not found in {2}.",
                        typeof(TItemData).FullName, typeof(TContext).FullName, CellPrefab.name));
                }

                cell.SetContext(Context);
                cell.Initialize();
                cell.SetVisible(false);
                pool.Add(cell);
            }
        }

        void UpdateCells(float firstPosition, int firstIndex, bool forceRefresh)
        {
            for (var i = 0; i < pool.Count; i++)
            {
                var index = firstIndex + i;
                var position = firstPosition + i * cellInterval;
                var cell = pool[CircularIndex(index, pool.Count)];

                if (loop)
                {
                    index = CircularIndex(index, ItemsSource.Count);
                }

                if (index < 0 || index >= ItemsSource.Count || position > 1f)
                {
                    cell.SetVisible(false);
                    continue;
                }

                if (forceRefresh || cell.Index != index || !cell.IsVisible)
                {
                    cell.Index = index;
                    cell.SetVisible(true);
                    cell.UpdateContent(ItemsSource[index]);
                }

                cell.UpdatePosition(position);
            }
        }

        int CircularIndex(int i, int size) => size < 1 ? 0 : i < 0 ? size - 1 + (i + 1) % size : i % size;

#if UNITY_EDITOR
        bool cachedLoop;
        float cachedCellInterval, cachedScrollOffset;

        void LateUpdate()
        {
            if (cachedLoop != loop ||
                cachedCellInterval != cellInterval ||
                cachedScrollOffset != scrollOffset)
            {
                cachedLoop = loop;
                cachedCellInterval = cellInterval;
                cachedScrollOffset = scrollOffset;

                UpdatePosition(currentPosition);
            }
        }
#endif
    }

    /// <summary>
    /// <see cref="FancyScrollView{TItemData}"/> のコンテキストクラス.
    /// </summary>
    public sealed class NullContext { }

    /// <summary>
    /// スクロールビューを実装するための抽象基底クラス.
    /// 無限スクロールおよびスナップに対応しています.
    /// </summary>
    /// <typeparam name="TItemData"></typeparam>
    /// <seealso cref="FancyScrollView{TItemData, TContext}"/>
    public abstract class FancyScrollView<TItemData> : FancyScrollView<TItemData, NullContext> { }
}
