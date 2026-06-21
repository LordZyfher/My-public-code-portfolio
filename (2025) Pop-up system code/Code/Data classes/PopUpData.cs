using System;
using UnityEngine;

[Serializable]
public class PopUpData
{
    [Header("Base data")]
    public string Title;
    public string Content;
    [Tooltip("\nSprite image order in the UI depends on it's index in this array.")]
    public Sprite[] sprites;
    [Tooltip("Sprites will be sorted by index from left to right and then continue to the next row.")]
    [Min(1)] public int SpriteColumnCount = 1;

    [Header("Sprite layout padding")]
    public RectOffset padding;

    [Header("Sprite layout spacing")]
    public int HorizontalSpacing = 0;
    public int VerticalSpacing = 0;

    [SerializeField, HideInInspector] private string guid = Guid.NewGuid().ToString();
    public string GUID => guid;
}
