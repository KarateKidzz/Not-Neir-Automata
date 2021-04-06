using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using Object = UnityEngine.Object;

[CustomPropertyDrawer(typeof(UniqueIDAttribute))]
public class UniqueIDAttributeDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;

        Object targetObject = property.serializedObject.targetObject;

        string assetPath;

        if (targetObject is MonoBehaviour component)
        {
            if (PrefabUtility.IsPartOfPrefabAsset(component))
            {
                assetPath = component.transform.parent == null ? AssetDatabase.GetAssetPath(targetObject) : null;
            }
            else
            {
                PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                
                if (component.gameObject == prefabStage.prefabContentsRoot)
                {
                    assetPath = prefabStage.prefabAssetPath;
                }
                else if (PrefabUtility.IsAnyPrefabInstanceRoot(component.gameObject))
                {
                    assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(component);
                    if (property.prefabOverride)
                        PrefabUtility.RevertPropertyOverride(property, InteractionMode.AutomatedAction);
                }
                else
                {
                    assetPath = null;
                }
            }
        }
        else // is ScriptableObject
        {
            assetPath = AssetDatabase.GetAssetPath(targetObject);
        }

        if (!string.IsNullOrEmpty(assetPath))
        {
            string assetGUID = AssetDatabase.AssetPathToGUID(assetPath);

            if (!string.IsNullOrEmpty(assetGUID))
            {
                if (AssetIDs.Instance.Set(assetGUID, targetObject))
                {
                    AssetIDs.SaveAssetIDs();
                }

                if (assetGUID != property.stringValue)
                {
                    property.stringValue = assetGUID;
                }
            }
        }

        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}
