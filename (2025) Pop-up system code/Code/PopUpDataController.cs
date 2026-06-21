using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpDataController : MonoBehaviour
{
    [SerializeField] PopUpDataProfile profile;

    [SerializeField] TMP_Text titleField;
    [SerializeField] TMP_Text contentField;
    [SerializeField] HorizontalLayoutGroup horizontalSpriteLayout;
    [SerializeField] GameObject columnPrefab;
    [SerializeField] Image image;

    private PopUpData popUpData;

    private List<VerticalLayoutGroup> columns = new();
    private List<Image> images = new();

    public void SetNewData(PopUpData data)
    {
        popUpData = data;
    }

    public void SetNewDataProfile(PopUpDataProfile data)
    {
        SetNewData(data.PopUpData);
    }

    public virtual void LoadDataToUI(bool enableUI)
    {
        if (popUpData == null)
        {
            if (profile == null)
            {
                Debug.LogError("No profile or data assigned!");
                return;
            }
            else popUpData = profile.PopUpData;
        }

        SetTextMeshProText(titleField, popUpData.Title);
        SetTextMeshProText(contentField, popUpData.Content);

        if (horizontalSpriteLayout != null && columnPrefab !=null)
        {
            UpdateImages(popUpData.sprites);
        }
        else Debug.LogError(DebugSpriteLayouts());

        if (enableUI) gameObject.SetActive(true);
    }


    private void SetTextMeshProText(TMP_Text tmp, string text)
    {
        if (tmp != null)
        {
            if (text.Length == 0)
                tmp.gameObject.SetActive(false);
            else
            {
                tmp.gameObject.SetActive(true);
                tmp.text = text;
            }
        }
    }

    /// <summary>
    /// If not enough columns have been created already, it creates more.
    /// If the count is less than created columns, it de-activates the excess (so it doesn't need to be created again later if a count is higher)
    /// </summary>
    /// <param name="columnCount"></param>
    /// <param name="verticalSpacing"></param>
    private void UpdateColumns(int columnCount, int verticalSpacing)
    {
        if (columnPrefab == null || horizontalSpriteLayout == null)
        {
            Debug.LogError(DebugSpriteLayouts());
            return;
        }

        //If by chance anything is destroyed, remove those from the list before anything else.
        CleanupStoredItems(columns);

        for (int i = 0; i < columnCount; i++)
        {
            //in case the column doesn't exist yet, it gets created
            if (columns.Count <= i)
            {
                GameObject newColumn = Instantiate(columnPrefab, horizontalSpriteLayout.transform);
                VerticalLayoutGroup vLayout = newColumn.GetComponent<VerticalLayoutGroup>() ?? newColumn.AddComponent<VerticalLayoutGroup>();
                columns.Add(vLayout);
            }

            VerticalLayoutGroup column = columns[i];
            column.spacing = verticalSpacing;
            column.gameObject.SetActive(true);
        }

        //Any columns that are not needed right now are disabled, NOT destroyed. No errors should occur if it is changed to destroying them.
        for (int i = columnCount; i < columns.Count; i++)
        {
            columns[i].gameObject.SetActive(false);
            //DestroyInactiveColumns(); //used for destruction instead of only setting them inactive.
        }
    }


    /// <summary>
    /// If no reference to an image is set, it will create a new one.
    /// </summary>
    /// <param name="sprites"></param>
    /// <param name="columnCount"></param>
    private void UpdateImages(Sprite[] sprites)
    {
        if (sprites.Length == 0)
        {
            horizontalSpriteLayout.gameObject.SetActive(false);
            return;
        }

        //More columns than there are images is useless, so reduce the count to image count if it exceeds image count.
        int columnCount = popUpData.SpriteColumnCount > popUpData.sprites.Length ? popUpData.sprites.Length : popUpData.SpriteColumnCount;

        //Get the layout ready for the images.
        UpdateColumns(columnCount, popUpData.VerticalSpacing);

        horizontalSpriteLayout.padding = popUpData.padding;
        horizontalSpriteLayout.spacing = popUpData.HorizontalSpacing;

        //set up variable to loop from the left to the right column
        int column = 0;

        //Make sure an image object exists
        if (image == null)
        {
            MakeImageRef(columnPrefab.transform);
        }

        if (images.Count == 0) images.Add(image);

        CleanupStoredItems(images);

        for (int i = 0; i < sprites.Length; i++)
        {
            //If the sprite doesn't exist, continue to the next loop.
            if (sprites[i] == null) continue;

            //makes sure to loop from left to right column
            if (column > columnCount) column = 0;

            //Create new image if needed
            if (images.Count <= i)
            {
                Image newImage = Instantiate(image);
                images.Add(newImage);
            }

            Image img = images[i];
            //always reparent the image to the correct column
            img.transform.SetParent(columns[column].transform);

            img.sprite = sprites[i];

            //prepare for next column
            column++;
        }

        horizontalSpriteLayout.gameObject.SetActive(true);
    }

    private void MakeImageRef(Transform parent)
    {
        GameObject newObj = new GameObject("Image");
        newObj.layer = parent.gameObject.layer;
        newObj.AddComponent<RectTransform>();
        newObj.AddComponent<CanvasRenderer>();
        image = newObj.AddComponent<Image>();
        newObj.AddComponent<SpriteAspectRatioUpdater>();
        newObj.transform.parent = parent;
    }

    public void CleanupStoredItems<T>(List<T> list) where T : class
    {
        list.RemoveAll(c => c == null);
    }

    public void DestroyInactiveColumns()
    {
        foreach (var column in columns)
        {
            //skip this loop if the object is active. Does NOT destroy objects that are active under an inactive parent.
            if (column.gameObject.activeSelf) continue;

            Destroy(column.gameObject);
        }

        //Null references are not needed anymore, so clean up the list.
        CleanupStoredItems(columns);
    }


    public string DebugSpriteLayouts()
    {
        return $"This component needs a Horizontal and Vertical sprite layout (Vertical null = {columnPrefab != null}, Horizontal null = {horizontalSpriteLayout != null})";
    }

}
