﻿using System;

using UnityEngine;
using UnityEditor;

/*
 * VRSuya Suyasuya Facial Editor
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace com.vrsuya.suyasuyafacial {

    [CustomEditor(typeof(SuyasuyaFacial))]
    public class SuyasuyaFacialEditor : Editor {

        SerializedProperty SerializedAvatarGameObject;
        SerializedProperty SerializedAvatarHeadSkinnedMeshRenderer;
        SerializedProperty SerializedTargetAnimationClips;
        SerializedProperty SerializedTargetBlendShapes;
		SerializedProperty SerializedStatusCode;

		public static int LanguageIndex = 0;
        public readonly string[] LanguageType = new[] { "English", "한국어", "日本語" };

		void OnEnable() {
            SerializedAvatarGameObject = serializedObject.FindProperty("AvatarGameObject");
			SerializedAvatarHeadSkinnedMeshRenderer = serializedObject.FindProperty("AvatarHeadSkinnedMeshRenderer");
			SerializedTargetAnimationClips = serializedObject.FindProperty("TargetAnimationClips");
			SerializedTargetBlendShapes = serializedObject.FindProperty("TargetBlendShapes");
			SerializedStatusCode = serializedObject.FindProperty("StatusCode");
		}

        public override void OnInspectorGUI() {
            serializedObject.Update();
			LanguageIndex = EditorGUILayout.Popup(LanguageHelper.GetContextString("String_Language"), LanguageIndex, LanguageType);
			EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
            EditorGUILayout.PropertyField(SerializedAvatarGameObject, new GUIContent(LanguageHelper.GetContextString("String_TargetAvatar")));
			EditorGUILayout.PropertyField(SerializedAvatarHeadSkinnedMeshRenderer, new GUIContent(LanguageHelper.GetContextString("String_TargetMesh")));
			EditorGUILayout.PropertyField(SerializedTargetAnimationClips, new GUIContent(LanguageHelper.GetContextString("String_TargetAnimations")));
			EditorGUILayout.LabelField(LanguageHelper.GetContextString("String_TargetBlendShape"), EditorStyles.boldLabel);
			EditorGUI.indentLevel++;
			if (SerializedTargetBlendShapes.arraySize > 0) {
				for (int Index = 0; Index < SerializedTargetBlendShapes.arraySize; Index++) {
					SerializedProperty BlendShapeProperty = SerializedTargetBlendShapes.GetArrayElementAtIndex(Index);
					SerializedProperty ActiveValueProperty = BlendShapeProperty.FindPropertyRelative("ActiveValue");
					SerializedProperty BlendShapeNameProperty = BlendShapeProperty.FindPropertyRelative("BlendShapeName");
					EditorGUILayout.BeginHorizontal();
					ActiveValueProperty.boolValue = EditorGUILayout.ToggleLeft(BlendShapeNameProperty.stringValue, ActiveValueProperty.boolValue);
					EditorGUILayout.EndHorizontal();
				}
			} else {
				EditorGUILayout.HelpBox(LanguageHelper.GetContextString("NO_SHAPEKEY"), MessageType.Info);
			}
			EditorGUI.indentLevel--;
			if (GUILayout.Button(LanguageHelper.GetContextString("String_Reload"))) {
				(target as SuyasuyaFacial).ReloadVariable();
				Repaint();
			}
			EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
			if (!string.IsNullOrEmpty(SerializedStatusCode.stringValue)) {
				EditorGUILayout.HelpBox(LanguageHelper.GetContextString(SerializedStatusCode.stringValue), MessageType.Warning);
            }
			serializedObject.ApplyModifiedProperties();
            if (GUILayout.Button(LanguageHelper.GetContextString("String_UpdateAnimations"))) {
                (target as SuyasuyaFacial).UpdateAnimationClips();
				Repaint();
			}
		}
	}
}
