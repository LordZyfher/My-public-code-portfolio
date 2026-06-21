using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PopUpUIController : MonoBehaviour
{
    public GameObject AnimatedObject;
    public UnityEvent<PopUpData> OnShow;
    public UnityEvent OnClosed;

    private bool allowClose = true;
    private PopUpGeneralSettings popUpGeneralSettings = new();

    private enum PopUpState { Idle, Showing, Closing }
    private PopUpState currentState = PopUpState.Idle;

    private bool isIdle => currentState == PopUpState.Idle;
    private bool isAnimating = false;

    private RectTransform redrawRect;//is usually the parent object of the pop-ups for a clean vertical multi-pop-up view
    private CanvasGroup canvasGroup;
    private PopUpData lastReceivedData;

    private void Awake()
    {
        SetCanvasGroup();
    }

    private void SetCanvasGroup()
    {
        if (canvasGroup != null) return;
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (canvasGroup != null) return;
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void CanceledShow()
    {
        OnClosed?.Invoke();
    }

    /// <summary>
    /// Returns false if it can't be shown
    /// </summary>
    /// <param name="data"></param>
    /// <param name="generalSettings"></param>
    /// <param name="rectToRedraw"></param>
    /// <param name="closeQueue"></param>
    /// <returns></returns>
    public void Show(PopUpData data, PopUpGeneralSettings generalSettings, RectTransform rectToRedraw)
    {
        lastReceivedData = data;
        OnShow?.Invoke(lastReceivedData);

        popUpGeneralSettings = generalSettings;

        redrawRect = rectToRedraw;

        gameObject.SetActive(true);

       // if (data.Title.Length > 0) Debug.Log($"Data loading: {data.Title}");
        StartCoroutine(StartShowing());

    }

    private IEnumerator StartShowing()
    {
        yield return new WaitUntil(() => isIdle == true);

        currentState = PopUpState.Showing;
        gameObject.SetActive(true);
        if (popUpGeneralSettings.SlideIn)
        {
            StartCoroutine(MoveAnimation(true));
        }

        StartCoroutine(WaitForMinDuration(popUpGeneralSettings.minPopUpDurationS));
        if (popUpGeneralSettings.maxPopUpDurationS > 0)
        {
            StartCoroutine(CloseOnMaxDuration(popUpGeneralSettings.maxPopUpDurationS));

            if (popUpGeneralSettings.minPopUpDurationS >= popUpGeneralSettings.maxPopUpDurationS)
                Debug.LogWarning("Pop-Up minimum duration is the same or larger than it's max duration, this will cause unintended behavior.");
        }

        yield return null;
        yield return new WaitForSecondsRealtime(0.1f);
        StartCoroutine(FixLayout());
        if (!popUpGeneralSettings.SlideIn) canvasGroup.alpha = 1;

        yield return new WaitUntil(() => isAnimating == false);

        currentState = PopUpState.Idle;
    }

    private IEnumerator WaitForMinDuration(float minDuration)
    {
        allowClose = false;
        yield return new WaitForSecondsRealtime(minDuration);
        allowClose = true;
    }

    private IEnumerator CloseOnMaxDuration(float maxDuration)
    {
        yield return new WaitForSecondsRealtime(maxDuration);
        Close(lastReceivedData);
    }

    public bool Close(PopUpData data, bool waitForAllowClose = false)
    {
        if (!waitForAllowClose && !allowClose) return true;

        if (data.GUID != lastReceivedData.GUID)
        {
            //  Debug.LogWarning($"Pop-up can't close. Given data does not match pop-up's data. data: {data.Title}, dispData: {currentDispData.Title}");
            return false;
        }

        StartCoroutine(Closing());
        return true;
    }

    public void Disable()
    {
        SetCanvasGroup();
        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }

    private IEnumerator Closing()
    {
        if (lastReceivedData.Title.Length > 0) Debug.Log($"Closing {lastReceivedData.Title}");
        yield return new WaitUntil(() => isIdle == true && allowClose == true);

        currentState = PopUpState.Closing;

        yield return new WaitForSecondsRealtime(popUpGeneralSettings.lingerDuration);

        if (popUpGeneralSettings.SlideOut)
        {
            StartCoroutine(MoveAnimation(false));
        }

        yield return new WaitUntil(() => isAnimating == false);

        Disable();

        currentState = PopUpState.Idle;

        OnClosed?.Invoke();
    }

    /// <summary>
    /// Offset is taken from the general settings for the Pop-up
    /// </summary>
    /// <param name="startWithOffset">If true: moves from Vector3.zero + offset towards it's start point, if false will move towards the offset</param>
    /// <returns></returns>
    private IEnumerator MoveAnimation(bool startWithOffset)
    {
        if (AnimatedObject != null)
        {
            isAnimating = true;

            Vector3 targPos = Vector3.zero;
            Vector3 startPos = Vector3.zero;

            yield return null;
            StartCoroutine(FixLayout());

            if (startWithOffset)
            {
                startPos = popUpGeneralSettings.SlideOffset;
            }
            else
            {
                targPos = popUpGeneralSettings.SlideOffset;
            }
            AnimatedObject.transform.localPosition = startPos;

            yield return new WaitForSecondsRealtime(0.2f);

            StartCoroutine(FixLayout());

            canvasGroup.alpha = 1f;
            Vector3 currentPos = startPos;

            while (Vector3.Distance(currentPos, targPos) > 0.1f)
            {
                AnimatedObject.transform.localPosition = Vector3.Slerp(currentPos, targPos, Time.deltaTime * 10);
                currentPos = AnimatedObject.transform.localPosition;
                yield return null;
            }

            isAnimating = false;
        }
    }

    private IEnumerator FixLayout()
    {
        if (redrawRect != null)
        {
            yield return null;
            LayoutRebuilder.ForceRebuildLayoutImmediate(redrawRect);
        }
    }
}
