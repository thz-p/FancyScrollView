/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

using UnityEngine;

namespace FancyScrollView
{
    /// <summary>
    /// 用于实现 <see cref="FancyScrollView{TItemData, TContext}"/> 的单元格的抽象基类.
    /// 如果不需要 <see cref="FancyCell{TItemData, TContext}.Context"/>，则使用 <see cref="FancyCell{TItemData}"/> 代替.
    /// </summary>
    /// <typeparam name="TItemData">项目数据的类型.</typeparam>
    /// <typeparam name="TContext"><see cref="Context"/> 的类型.</typeparam>
    public abstract class FancyCell<TItemData, TContext> : MonoBehaviour where TContext : class, new()
    {
        /// <summary>
        /// 此单元格显示的数据的索引.
        /// </summary>
        public int Index { get; set; } = -1;

        /// <summary>
        /// 此单元格的可见状态.
        /// </summary>
        public virtual bool IsVisible => gameObject.activeSelf;

        /// <summary>
        /// <see cref="FancyScrollView{TItemData, TContext}.Context"/> 的引用.
        /// 在单元格和滚动视图之间共享相同的实例. 用于传递信息和保持状态.
        /// </summary>
        protected TContext Context { get; private set; }

        /// <summary>
        /// 设置 <see cref="Context"/>.
        /// </summary>
        /// <param name="context">上下文.</param>
        public virtual void SetContext(TContext context) => Context = context;

        /// <summary>
        /// 执行初始化.
        /// </summary>
        public virtual void Initialize() { }

        /// <summary>
        /// 设置此单元格的可见状态.
        /// </summary>
        /// <param name="visible"><c>true</c> 表示可见状态，<c>false</c> 表示不可见状态.</param>
        public virtual void SetVisible(bool visible) => gameObject.SetActive(visible);

        /// <summary>
        /// 根据项目数据更新此单元格的显示内容.
        /// </summary>
        /// <param name="itemData">项目数据.</param>
        public abstract void UpdateContent(TItemData itemData);

        /// <summary>
        /// 根据 <c>0.0f</c> ~ <c>1.0f</c> 的值更新此单元格的滚动位置.
        /// </summary>
        /// <param name="position">规范化的滚动位置，位于视口范围内.</param>
        public abstract void UpdatePosition(float position);
    }

    /// <summary>
    /// 用于实现 <see cref="FancyScrollView{TItemData}"/> 的单元格的抽象基类.
    /// </summary>
    /// <typeparam name="TItemData">项目数据的类型.</typeparam>
    /// <seealso cref="FancyCell{TItemData, TContext}"/>
    public abstract class FancyCell<TItemData> : FancyCell<TItemData, NullContext>
    {
        /// <inheritdoc/>
        public sealed override void SetContext(NullContext context) => base.SetContext(context);
    }
}
