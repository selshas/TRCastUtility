using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR

using UnityEditor;
using UnityEditorInternal;

#endif


[System.Serializable]
public class Roster
{
    [SerializeField]
    public string name;

    [SerializeField]
    public eFaction faction;

    [SerializeField]
    public Texture2D tex_icon;

    [SerializeField]
    public Texture2D tex_ria;

    [SerializeField]
    public bool isEntered = false;

    [SerializeField]
    public string info;
}

[System.Serializable]
public class LoadoutSlot
{
    [SerializeField]
    public List<Roster> units = new List<Roster>();

    [SerializeField]
    string label;
}

[System.Serializable]
public class SourceDB_Faction : MonoBehaviour
{
    [SerializeField]
    public Texture2D tex2D_banner;
    
    [SerializeField]
    public Texture2D tex2D_loadoutFrame;
    [SerializeField]
    public Texture2D tex2D_loadoutFrame_bg;

    [SerializeField]
    public List<LoadoutSlot> slots = new List<LoadoutSlot>();
}


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(LoadoutSlot))]
class PropDrawer_LoadoutSlot : PropertyDrawer
{
    public Dictionary<string, ReorderableList> dict_reorderableList_units = new Dictionary<string, ReorderableList>();
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty prop_units = property.FindPropertyRelative("units");
        ReorderableList unitList;
        if (!dict_reorderableList_units.TryGetValue(property.propertyPath, out unitList))
        {
            unitList = new ReorderableList(prop_units.serializedObject, prop_units);
            dict_reorderableList_units.Add(property.propertyPath, unitList);

            unitList.elementHeightCallback = (index) =>
            {
                if (prop_units.arraySize == 0) return 0;
                int height = 18 + 18 + 18 + 64;
                SerializedProperty element = prop_units.GetArrayElementAtIndex(index);
                Texture2D tex2D_icon, tex2D_ria;
                tex2D_icon = (Texture2D)element.FindPropertyRelative("tex_icon").objectReferenceValue;
                tex2D_ria = (Texture2D)element.FindPropertyRelative("tex_ria").objectReferenceValue;

                if (tex2D_icon != null || tex2D_ria != null)
                {
                    height += 128;
                }

                return height;
            };
            unitList.drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, "Units");
            };
            unitList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                SerializedProperty element = prop_units.GetArrayElementAtIndex(index);
                Rect fieldRect = rect;
                fieldRect.height = 18;
                element.FindPropertyRelative("name").stringValue = EditorGUI.TextField(fieldRect, "Name", element.FindPropertyRelative("name").stringValue);
                fieldRect.y += 18;

                EditorGUI.ObjectField(fieldRect, element.FindPropertyRelative("tex_icon"));
                fieldRect.y += 18;

                EditorGUI.ObjectField(fieldRect, element.FindPropertyRelative("tex_ria"));
                fieldRect.y += 18;

                fieldRect.x = rect.width - 64;
                fieldRect.width = 128;
                fieldRect.height = 128;
                Texture2D tex2D_icon, tex2D_ria;
                tex2D_icon = (Texture2D)element.FindPropertyRelative("tex_icon").objectReferenceValue;
                tex2D_ria = (Texture2D)element.FindPropertyRelative("tex_ria").objectReferenceValue;
                if (tex2D_icon != null)
                {
                    EditorGUI.DrawTextureTransparent(fieldRect, tex2D_icon, ScaleMode.StretchToFill);
                }
                fieldRect.x -= 128;
                if (tex2D_ria != null)
                {
                    EditorGUI.DrawTextureTransparent(fieldRect, tex2D_ria, ScaleMode.StretchToFill);
                }
                if (tex2D_icon != null || tex2D_ria != null)
                {
                    fieldRect.y += 128;
                }

                fieldRect.x = rect.x;
                fieldRect.width = rect.width;
                fieldRect.height = 64;
                element.FindPropertyRelative("info").stringValue = EditorGUI.TextArea(fieldRect, element.FindPropertyRelative("info").stringValue);
            };
        }

        Editor_SourceDB_Faction.dict_elementHeights[property.propertyPath] = unitList.GetHeight();

        unitList.DoList(
            new Rect(
                position.position, 
                position.size
            )
        );
    }
}

[CustomEditor(typeof(SourceDB_Faction), true), CanEditMultipleObjects]
class Editor_SourceDB_Faction : Editor
{
    public static Dictionary<string, float> dict_elementHeights = new Dictionary<string, float>();
    public static Dictionary<string, bool> dict_folding = new Dictionary<string, bool>();

    ReorderableList reorderableList_slots;
    private void OnEnable()
    {
        SerializedProperty property = serializedObject.FindProperty("slots");

        reorderableList_slots = new ReorderableList(serializedObject, property, true, true, true, true);
        reorderableList_slots.drawHeaderCallback = (rect) =>
        {
            EditorGUI.LabelField(rect, $"Slots");
        };

        reorderableList_slots.elementHeightCallback = (index) => {
            if (property.arraySize == 0) return 0;
            float height = 18;
            SerializedProperty element = property.GetArrayElementAtIndex(index);

            if (!dict_folding.ContainsKey(element.propertyPath))
                dict_folding[element.propertyPath] = false;

            if (dict_folding[element.propertyPath])
            {
                height += (dict_elementHeights.ContainsKey(element.propertyPath)) ? dict_elementHeights[element.propertyPath] : 0;
            }

            return height;
        };
        reorderableList_slots.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            SerializedProperty element = property.GetArrayElementAtIndex(index);

            Rect fieldRect = rect;
            fieldRect.x += 18;
            fieldRect.height = 18;
            if (!dict_folding.ContainsKey(element.propertyPath))
                dict_folding[element.propertyPath] = false;

            dict_folding[element.propertyPath] = EditorGUI.Foldout(fieldRect, dict_folding[element.propertyPath], $"Slot {index}");

            if (dict_folding[element.propertyPath])
            {
                fieldRect = rect;
                fieldRect.y += 18;
                EditorGUI.PropertyField(fieldRect, element);
            }
        };
        reorderableList_slots.onAddCallback = (list) =>
        {
            int index = property.arraySize;
            property.InsertArrayElementAtIndex(property.arraySize);
            SerializedProperty element = property.GetArrayElementAtIndex(index);
            if (!dict_folding.ContainsKey(element.propertyPath))
                dict_folding.Add(element.propertyPath, true);
        };
    }
    public override void OnInspectorGUI()
    {
        EditorGUILayout.ObjectField(serializedObject.FindProperty("tex2D_banner"));
        EditorGUILayout.ObjectField(serializedObject.FindProperty("tex2D_loadoutFrame"));
        EditorGUILayout.ObjectField(serializedObject.FindProperty("tex2D_loadoutFrame_bg"));

        reorderableList_slots.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }
}

#endif