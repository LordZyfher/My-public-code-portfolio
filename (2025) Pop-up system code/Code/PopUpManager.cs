using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpManager : MonoBehaviour
{
    public PopUpGeneralSettings GeneralSettings;

    private PopUpUIController[] popUpSlots;

    private Queue<PopUpData> dataQueue = new();

    private List<string> closeQueueGUID = new();

    private RectTransform rectTransform;

    private void Awake()
    {
        List<PopUpUIController> controllers = new();

        //Only want to look at direct child objects, no children of children.
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            PopUpUIController controller = child.GetComponent<PopUpUIController>();
            if (controller != null)
            {
                controller.Disable();
                controller.OnClosed.AddListener(delegate { NotifyClosed(controller); });
                controllers.Add(controller);
            }
        }

        popUpSlots = controllers.ToArray();
        if (popUpSlots.Length > 1) Debug.LogWarning("Pop-up: Multiple pop-ups detected under a manager, this is currently bugged with 2 main issues: no sorting, vertical layout not updating correctly.");

        rectTransform = GetComponent<RectTransform>();
    }

    public void AddDataToQueue(PopUpData data)
    {
        if (closeQueueGUID.Contains(data.GUID)) return;

       // Debug.Log($"Adding Data To Queue: {data.Title} - {data.GUID}");

        if (!TryDisplay(data))
        {
            dataQueue.Enqueue(data);
        }
    }

    public void AddDataProfileToQueue(PopUpDataProfile dataProfile)
    {
        AddDataToQueue(dataProfile.PopUpData);
    }

    private bool TryDisplay(PopUpData data)
    {
        foreach (var slot in popUpSlots)
        {
            if (!slot.gameObject.activeSelf)
            {
                CallShow(slot, data);
                return true;
            }
        }

        return false;
    }

    public void TryCloseDisplay(PopUpDataProfile data)
    {
        foreach (var slot in popUpSlots)
        {
            if (slot.gameObject.activeSelf)
            {
                if (!slot.Close(data.PopUpData, true)) StartCoroutine(AddToCloseQueue(data.PopUpData));
                return;
            }
        }
    }

    private IEnumerator AddToCloseQueue(PopUpData data)
    {
        closeQueueGUID.Add(data.GUID);
        //Debug.Log($"{closeQueueGUID.Count} Could not find the data on an active pop-up: {data.Title} - {data.GUID}, storing it in the closing queue for {GeneralSettings.SaveInCloseQueueDuration} sec");

        yield return new WaitForSeconds(GeneralSettings.SaveInCloseQueueDuration);
        closeQueueGUID.Remove(data.GUID);
      //  Debug.Log($"{closeQueueGUID.Count} Removing data from the close queue: {data.Title}");
    }

    /// <summary>
    /// Closes a specific message matching the given data and relocates the unused pop-up child as last child.
    /// </summary>
    public void NotifyClosed(PopUpUIController slot)
    {
        if (dataQueue.Count > 0)
        {
            var nextData = dataQueue.Dequeue();
            CallShow(slot, nextData);
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }
    }

    private void CallShow(PopUpUIController slot, PopUpData data)
    {
        if (closeQueueGUID.Contains(data.GUID))
        {
            slot.CanceledShow();
            return;
        }
        slot.Show(data, GeneralSettings, rectTransform);
    }
}
