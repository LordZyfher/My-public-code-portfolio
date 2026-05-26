using System.Reflection;
using System;
using UnityEngine;

[Serializable]
public abstract class FieldCondition : TaskCondition
{
    public MonoBehaviour script;
    [Tooltip("Be careful! It is case sensitive.")]
    public string fieldName;

    protected FieldInfo GetFieldRecursive(Type type, string name)
    {
        while (type != null)
        {
            var field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            if (field != null)
                return field;

            type = type.BaseType;
        }
        return null;
    }

    protected bool ValidScriptAndFieldName()
    {
        if (script == null || string.IsNullOrEmpty(fieldName))
        {
            Debug.LogError($"script = {script}, string = {fieldName}");
            return false;
        }

        return true;
    }

    public override bool IsMet()
    {
        return false;
    }
}

[Serializable]
public class ScriptBoolCondition : FieldCondition
{
    public bool targetState = true;

    public override bool IsMet()
    {
        if (!ValidScriptAndFieldName()) return false;

        var field = GetFieldRecursive(script.GetType(), fieldName);

        if (field != null && field.FieldType == typeof(bool))
            return (bool)field.GetValue(script) == targetState;
        return false;
    }
}

[Serializable]
public class ScriptEnumCondition : FieldCondition
{
    public enum EnumComparisonMode
    {
        Equals,
        NotEquals
    }

    public string targetValue;
    public EnumComparisonMode enumComparison = EnumComparisonMode.Equals;
    public override bool IsMet()
    {
        if (!ValidScriptAndFieldName()) return false;

        var field = GetFieldRecursive(script.GetType(), fieldName);

        if (field == null)
        {
            Debug.LogError($"field = {field}");
            return false;
        }

        var value = field.GetValue(script);
        if (field.FieldType.IsEnum && targetValue != null)
        {
            if (Enum.TryParse(field.FieldType, targetValue, out var parsedEnum))
            {
                bool isEqual = value.Equals(parsedEnum);
                return enumComparison == EnumComparisonMode.Equals ? isEqual : !isEqual;
            }
            else
            {
                Debug.LogError($"Parse failed ({targetValue}), enum ({field.FieldType.Name})");
                return false;
            }
        }
        return false;
    }
}