/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

using UnityEditor;
using UnityEditor.AnimatedValues;

// For manteinance, every new [SerializeField] variable in Scroller must be declared here

namespace FancyScrollView
{
    [CustomEditor(typeof(Scroller))]
    [CanEditMultipleObjects]
    public class ScrollerEditor : Editor
    {
        SerializedProperty viewport;
        SerializedProperty scrollDirection;
        SerializedProperty movementType;
        SerializedProperty elasticity;
        SerializedProperty scrollSensitivity;
        SerializedProperty inertia;
        SerializedProperty decelerationRate;
        SerializedProperty snap;
        SerializedProperty draggable;
        SerializedProperty scrollbar;

        AnimBool showElasticity;
        AnimBool showInertiaRelatedValues;

        // 当 Unity Inspector 中启用脚本时调用
        void OnEnable()
        {
            // 使用 serializedObject 找到对应属性
            viewport = serializedObject.FindProperty("viewport");
            scrollDirection = serializedObject.FindProperty("scrollDirection");
            movementType = serializedObject.FindProperty("movementType");
            elasticity = serializedObject.FindProperty("elasticity");
            scrollSensitivity = serializedObject.FindProperty("scrollSensitivity");
            inertia = serializedObject.FindProperty("inertia");
            decelerationRate = serializedObject.FindProperty("decelerationRate");
            snap = serializedObject.FindProperty("snap");
            draggable = serializedObject.FindProperty("draggable");
            scrollbar = serializedObject.FindProperty("scrollbar");

            // 实例化 AnimBool 变量，并传入一个委托（Repaint），用于在动画发生改变时重绘界面
            showElasticity = new AnimBool(Repaint);
            showInertiaRelatedValues = new AnimBool(Repaint);

            // 设置初始动画状态
            SetAnimBools(true);
        }

        // 当 Unity Inspector 中禁用脚本时调用
        void OnDisable()
        {
            // 移除动画变量的监听器，以避免在禁用脚本时触发不必要的重绘操作
            showElasticity.valueChanged.RemoveListener(Repaint);
            showInertiaRelatedValues.valueChanged.RemoveListener(Repaint);
        }

        // 设置动画布尔值
        void SetAnimBools(bool instant)
        {
            // 设置 showElasticity 的动画布尔值
            // 如果 movementType 不具有多个不同的值，并且 movementType 的枚举值索引等于 MovementType.Elastic 的整数值
            SetAnimBool(showElasticity, !movementType.hasMultipleDifferentValues && movementType.enumValueIndex == (int)MovementType.Elastic, instant);
            
            // 设置 showInertiaRelatedValues 的动画布尔值
            // 如果 inertia 不具有多个不同的值，并且 inertia 的布尔值为真
            SetAnimBool(showInertiaRelatedValues, !inertia.hasMultipleDifferentValues && inertia.boolValue, instant);
        }

        void SetAnimBool(AnimBool a, bool value, bool instant)
        {
            if (instant)
            {
                a.value = value;
            }
            else
            {
                a.target = value;
            }
        }

        // 覆盖基类的 OnInspectorGUI 函数
        public override void OnInspectorGUI()
        {
            // 调用 SetAnimBools 函数，并传入 false 作为参数
            SetAnimBools(false);

            // 更新序列化对象
            serializedObject.Update();

            // 在编辑器中绘制视口属性字段
            EditorGUILayout.PropertyField(viewport);
            
            // 在编辑器中绘制滚动方向属性字段
            EditorGUILayout.PropertyField(scrollDirection);
            
            // 在编辑器中绘制运动类型属性字段
            EditorGUILayout.PropertyField(movementType);
            
            // 绘制与运动类型相关的值
            DrawMovementTypeRelatedValue();
            
            // 在编辑器中绘制滚动灵敏度属性字段
            EditorGUILayout.PropertyField(scrollSensitivity);
            
            // 在编辑器中绘制惯性属性字段
            EditorGUILayout.PropertyField(inertia);
            
            // 绘制与惯性相关的值
            DrawInertiaRelatedValues();
            
            // 在编辑器中绘制可拖动属性字段
            EditorGUILayout.PropertyField(draggable);
            
            // 在编辑器中绘制滚动条属性字段
            EditorGUILayout.PropertyField(scrollbar);
            
            // 应用修改后的序列化对象
            serializedObject.ApplyModifiedProperties();
        }

        // 绘制与运动类型相关的值
        void DrawMovementTypeRelatedValue()
        {
            // 使用 FadeGroupScope 来控制元素的淡入淡出效果，使用 showElasticity.faded 来控制其可见性
            using (var group = new EditorGUILayout.FadeGroupScope(showElasticity.faded))
            {
                // 如果组不可见，则直接返回，不进行绘制
                if (!group.visible)
                {
                    return;
                }

                // 使用 IndentLevelScope 来增加缩进级别，使字段在界面上更加清晰
                using (new EditorGUI.IndentLevelScope())
                {
                    // 在缩进的级别上绘制弹性字段
                    EditorGUILayout.PropertyField(elasticity);
                }
            }
        }

        void DrawInertiaRelatedValues()
        {
            using (var group = new EditorGUILayout.FadeGroupScope(showInertiaRelatedValues.faded))
            {
                if (!group.visible)
                {
                    return;
                }

                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(decelerationRate);
                    EditorGUILayout.PropertyField(snap);
                }
            }
        }
    }
}
