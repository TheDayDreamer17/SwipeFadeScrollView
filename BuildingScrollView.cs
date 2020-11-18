using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingScrollView : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public List<Image> buildingImages;
    public int CurrentPageIndex = 0;
    public bool custom = false;
    float t = 0;
    float currentAlpha = 1, previousAlpha = 1, NextAplha = 1;
    Color currentColor;
    Color previousColor;
    Color nextColor;
    bool pageChanging = false;
    void Start()
    {
        // StartCoroutine(FadeImage(0, false));
        // StartCoroutine(checkHorizontalSwipes());
        pageChanged(0);
    }
    public void goToNextPage()
    {
        StartCoroutine(FadeImage(CurrentPageIndex, true));

        if (CurrentPageIndex >= buildingImages.Count - 1)
        {
            CurrentPageIndex = 0;
        }
        else
            CurrentPageIndex++;
        StartCoroutine(FadeImage(CurrentPageIndex, false));


    }
    public void goToPreviousPage()
    {
        StartCoroutine(FadeImage(CurrentPageIndex, true));

        if (CurrentPageIndex <= 0)
        {
            CurrentPageIndex = buildingImages.Count - 1;
        }
        else
            CurrentPageIndex--;
        StartCoroutine(FadeImage(CurrentPageIndex, false));



    }
    void pageChanged(int value)
    {
        if (value > 0 && CurrentPageIndex >= buildingImages.Count - 1)
        {
            CurrentPageIndex = 0;
        }
        else if (value < 0 && CurrentPageIndex <= 0)
        {
            CurrentPageIndex = buildingImages.Count - 1;
        }
        else
            CurrentPageIndex += value;
        // CurrentPageIndex++;
        currentColor = buildingImages[CurrentPageIndex].color;
        previousColor = buildingImages[CurrentPageIndex <= 0 ? buildingImages.Count - 1 : CurrentPageIndex - 1].color;
        nextColor = buildingImages[CurrentPageIndex >= buildingImages.Count - 1 ? 0 : CurrentPageIndex + 1].color;
        pageChanging = false;
    }

    void Update()
    {
        if (!custom)
        {
            t += Time.deltaTime;
            if (t >= 4f)
            {
                t = 0;
                goToNextPage();
            }

        }
        else
        {
            /* XAxisScaleGesture.ZoomSpeed = ZoomSpeedX;
            XAxisScaleGesture.PlatformSpecificView = XAxisTransformDragView; */
            // TransformToScale = (TransformToScale == null ? transform : TransformToScale);
        }
    }

    IEnumerator FadeImage(int index, bool fadeAway)
    {
        // fade from opaque to transparent
        if (fadeAway)
        {
            Color color = buildingImages[index].color;
            // loop over 1 second backwards
            for (float i = 1; i >= 0; i -= Time.deltaTime)
            {
                // set color with i as alpha
                color.a = i;
                buildingImages[index].color = color;
                yield return null;
            }
        }
        // fade from transparent to opaque
        else
        {
            Color color = buildingImages[index].color;
            // loop over 1 second
            for (float i = 0; i <= 1; i += Time.deltaTime)
            {
                // set color with i as alpha
                color.a = i;
                buildingImages[index].color = color;
                yield return null;
            }
        }
    }

    IEnumerator FadeImageFromBetween(int index, bool fadeAway, float currentValue = 0)
    {
        // fade from opaque to transparent
        if (fadeAway)
        {
            Color color = buildingImages[index].color;
            // loop over 1 second backwards
            for (float i = currentValue; i >= 0; i -= Time.deltaTime)
            {
                // set color with i as alpha
                color.a = i;
                buildingImages[index].color = color;
                yield return null;
            }
        }
        // fade from transparent to opaque
        else
        {
            Color color = buildingImages[index].color;
            // loop over 1 second
            for (float i = currentValue; i <= 1; i += Time.deltaTime)
            {
                // set color with i as alpha
                color.a = i;
                buildingImages[index].color = color;
                yield return null;
            }
        }
    }

    private Vector3 panelLocation;
    public float percentThreshold = 0.2f;
    public float easing = 0.5f;
    public int totalPages = 1;
    private int currentPage = 1;

    float percentage;
    bool swiped = true;
    int direction = 0;
    public void OnDrag(PointerEventData data)
    {

        float difference = data.position.x - data.pressPosition.x;
        // transform.position = panelLocation - new Vector3(difference, 0, 0);
        // Debug.Log(data.position);
        percentage = difference / Screen.width;
        float value = percentage;

        if (!pageChanging && value > -percentThreshold && value < percentThreshold && swiped)
        {
            // Debug.Log(percentage);

            if (value > 0)
            {
                direction = 1;
                currentColor.a = Mathf.Clamp01((-value + 1) * 2);
                buildingImages[CurrentPageIndex].color = currentColor;
                nextColor.a = Mathf.Clamp01(value * 2);
                buildingImages[CurrentPageIndex >= buildingImages.Count - 1 ? 0 : CurrentPageIndex + 1].color = nextColor;
                // if (value >= 0.5f)
                // {
                //     // pageChanging = true;
                //     // swiped = false;
                //     // pageChanged(1);
                // }
            }
            else if (value < 0)
            {
                direction = -1;
                currentColor.a = Mathf.Clamp01((value + 1) * 2);
                buildingImages[CurrentPageIndex].color = currentColor;
                previousColor.a = Mathf.Clamp01(-value * 2);
                buildingImages[CurrentPageIndex <= 0 ? buildingImages.Count - 1 : CurrentPageIndex - 1].color = previousColor;
                // if (value <= -0.5f)
                // {
                //     // pageChanging = true;
                //     // swiped = false;
                //     // pageChanged(-1);
                // }
            }
        }

    }
    public void OnEndDrag(PointerEventData data)
    {

        swipeToNearest();
        direction = 0;
        swiped = true;
    }

    private void swipeToNearest()
    {
        if (direction > 0 && buildingImages[CurrentPageIndex >= buildingImages.Count - 1 ? 0 : CurrentPageIndex + 1].color.a > 0.5f)
        {
            StartCoroutine(FadeImageFromBetween(CurrentPageIndex >= buildingImages.Count - 1 ? 0 : CurrentPageIndex + 1, false, buildingImages[CurrentPageIndex >= buildingImages.Count - 1 ? 0 : CurrentPageIndex + 1].color.a));
            StartCoroutine(FadeImageFromBetween(CurrentPageIndex, true, buildingImages[CurrentPageIndex].color.a));
            pageChanged(1);
        }
        else
        {
            StartCoroutine(FadeImageFromBetween(CurrentPageIndex >= buildingImages.Count - 1 ? 0 : CurrentPageIndex + 1, true, buildingImages[CurrentPageIndex >= buildingImages.Count - 1 ? 0 : CurrentPageIndex + 1].color.a));
            StartCoroutine(FadeImageFromBetween(CurrentPageIndex, false, buildingImages[CurrentPageIndex].color.a));
            pageChanged(0);
        }
        if (direction < 0 && buildingImages[CurrentPageIndex <= 0 ? buildingImages.Count - 1 : CurrentPageIndex - 1].color.a > 0.5f)
        {
            Debug.Log(buildingImages[CurrentPageIndex <= 0 ? buildingImages.Count - 1 : CurrentPageIndex - 1].color.a);
            StartCoroutine(FadeImageFromBetween(CurrentPageIndex <= 0 ? buildingImages.Count - 1 : CurrentPageIndex - 1, false, buildingImages[CurrentPageIndex <= 0 ? buildingImages.Count - 1 : CurrentPageIndex - 1].color.a));
            StartCoroutine(FadeImageFromBetween(CurrentPageIndex, true, buildingImages[CurrentPageIndex].color.a));
            pageChanged(-1);
        }
        else
        {
            StartCoroutine(FadeImageFromBetween(CurrentPageIndex <= 0 ? buildingImages.Count - 1 : CurrentPageIndex - 1, true, buildingImages[CurrentPageIndex <= 0 ? buildingImages.Count - 1 : CurrentPageIndex - 1].color.a));
            StartCoroutine(FadeImageFromBetween(CurrentPageIndex, false, buildingImages[CurrentPageIndex].color.a));
            pageChanged(0);
        }
    }
}

