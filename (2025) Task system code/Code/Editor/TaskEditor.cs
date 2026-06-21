#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

[CustomEditor(typeof(Taskr))]
public class TaskEditor : Editor
{
    private static readonly Type[] conditionTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(asm => asm.GetTypes())
            .Where(t => t.IsSubclassOf(typeof(TaskCondition)) && !t.IsAbstract)
            .ToArray();

    private string[] conditionTypeNames;

    private void OnEnable()
    {
        conditionTypeNames = conditionTypes
    .Select(t => t.Name)
    .ToArray();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawPropertiesExcluding(serializedObject, new[] { "conditions" });


        SerializedProperty conditionsProp = serializedObject.FindProperty("conditions");
        Vector2 btnSize = new(200, 40);

        Color standardColor = GUI.backgroundColor;

        GUI.backgroundColor = Color.green;
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();


        // Add Button
        if (GUILayout.Button("+ Add Condition", GUILayout.Width(btnSize.x), GUILayout.Height(btnSize.y)))
        {
            GenericMenu menu = new GenericMenu();
            for (int i = 0; i < conditionTypes.Length; i++)
            {
                int index = i;//remove this and you get index out of range. Lambda remembers variable i and the menu gets drawn after the loop is done (which is when i is out of range).
                menu.AddItem(new GUIContent(conditionTypeNames[i]), false, () =>
                {
                    var newCondition = Activator.CreateInstance(conditionTypes[index]);
                    conditionsProp.arraySize++;
                    var newElement = conditionsProp.GetArrayElementAtIndex(conditionsProp.arraySize - 1);
                    newElement.managedReferenceValue = newCondition;
                    serializedObject.ApplyModifiedProperties();
                });
            }
            menu.ShowAsContext();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUI.backgroundColor = standardColor;

        EditorGUILayout.Space();

        // Show conditions
        for (int i = 0; i < conditionsProp.arraySize; i++)
        {
            SerializedProperty element = conditionsProp.GetArrayElementAtIndex(i);
            if (element.managedReferenceValue == null)
            {
                EditorGUILayout.HelpBox($"Element {i} is null.", MessageType.Warning);
                continue;
            }

            EditorGUILayout.BeginVertical("box");

            // Show type name
            EditorGUILayout.LabelField($"Condition {i}: {element.managedReferenceValue.GetType().Name}", EditorStyles.boldLabel);

            // Draw fields of the condition
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(element, true);
            EditorGUI.indentLevel--;

            GUI.backgroundColor = Color.red;
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            // Remove button
            if (GUILayout.Button("- Remove Condition", GUILayout.Width(btnSize.x), GUILayout.Height(btnSize.y)))
            {
                conditionsProp.DeleteArrayElementAtIndex(i);
                break; // Required to avoid errors while iterating
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
            GUI.backgroundColor = standardColor;
        }

        serializedObject.ApplyModifiedProperties();
    }
}

#endif