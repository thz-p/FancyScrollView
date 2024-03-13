﻿/*
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

        public override void OnInspectorGUI()
        {
            SetAnimBools(false);

            serializedObject.Update();
            EditorGUILayout.PropertyField(viewport);
            EditorGUILayout.PropertyField(scrollDirection);
            EditorGUILayout.PropertyField(movementType);
            DrawMovementTypeRelatedValue();
            EditorGUILayout.PropertyField(scrollSensitivity);
            EditorGUILayout.PropertyField(inertia);
            DrawInertiaRelatedValues();
            EditorGUILayout.PropertyField(draggable);
            EditorGUILayout.PropertyField(scrollbar);
            serializedObject.ApplyModifiedProperties();
        }

        void DrawMovementTypeRelatedValue()
        {
            using (var group = new EditorGUILayout.FadeGroupScope(showElasticity.faded))
            {
                if (!group.visible)
                {
                    return;
                }

                using (new EditorGUI.IndentLevelScope())
                {
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
