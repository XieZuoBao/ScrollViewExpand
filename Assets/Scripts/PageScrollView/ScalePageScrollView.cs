using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 分页滚动的拓展-----近大远小
/// </summary>
public class ScalePageScrollView : PageScrollView
{
    #region 字段
    //所有页的游戏对象---需要对其进行近大远小缩放
    protected GameObject[] items;

    public float currentScale = 1.0f;
    public float otherScale = 0.6f;

    /// <summary>
    /// 左滑缩小页/右滑放大页
    /// </summary>
    public int lastPage;
    /// <summary>
    /// 左滑放大页/右滑缩小页
    /// </summary>
    public int nextPage;
    #endregion

    #region Unity回调
    protected override void Start()
    {
        base.Start();
        //初始化所有的页游戏对象
        items = new GameObject[pagesCount];
        for (int i = 0; i < pagesCount; i++)
        {
            items[i] = content.GetChild(i).gameObject;
        }
    }

    protected override void Update()
    {
        base.Update();
        switch (pageScrollType)
        {
            case PageScrollType.HORIZONTAL:
                ScaleHorizontal();
                break;
            case PageScrollType.VERTICAL:
                ScaleVettical();
                //Invoke("ScaleVettical", 0.01f);
                //Debug.Log(rect.verticalNormalizedPosition);
                break;
        }
    }
    #endregion

    #region 方法
    /// <summary>
    /// 水平缩放
    /// </summary>
    private void ScaleHorizontal()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i] <= rect.horizontalNormalizedPosition)
            {
                lastPage = i;
            }
        }
        nextPage = lastPage + 1 < pages.Length ? lastPage + 1 : pages.Length - 1;
        /*
        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i] > rect.horizontalNormalizedPosition)
            {
                nextPage = i;
                break;
            }
        }
        */
        Debug.LogError(lastPage + "======" + nextPage);
        if (lastPage == nextPage)
            return;
        float percent = (rect.horizontalNormalizedPosition - pages[lastPage]) / (pages[nextPage] - pages[lastPage]);
        items[lastPage].transform.localScale = Vector3.Lerp(Vector3.one * currentScale, Vector3.one * otherScale, percent);
        items[nextPage].transform.localScale = Vector3.Lerp(Vector3.one * currentScale, Vector3.one * otherScale, 1 - percent);
        //其他页
        for (int i = 0; i < items.Length; i++)
        {
            if (i != lastPage && i != nextPage)
            {
                items[i].transform.localScale = Vector3.one * otherScale;
            }
        }
    }

    /// <summary>
    /// 纵向缩放
    /// </summary>
    private void ScaleVettical()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i] <= rect.verticalNormalizedPosition)
            {
                lastPage = i;
                break;
            }
        }
        /*
        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i] > rect.verticalNormalizedPosition)
            {
                nextPage = i;
                //break;
            }
        }
        */
        nextPage = lastPage + 1 < pages.Length ? lastPage + 1 : pages.Length - 1;
        Debug.LogError(lastPage + "======" + nextPage);
        if (lastPage == nextPage)
            return;
        float percent = (rect.verticalNormalizedPosition - pages[lastPage]) / (pages[nextPage] - pages[lastPage]);
        items[lastPage].transform.localScale = Vector3.Lerp(Vector3.one * currentScale, Vector3.one * otherScale, percent);
        items[nextPage].transform.localScale = Vector3.Lerp(Vector3.one * currentScale, Vector3.one * otherScale, 1 - percent);
        //其他页
        for (int i = 0; i < items.Length; i++)
        {
            if (i != lastPage && i != nextPage)
            {
                items[i].transform.localScale = Vector3.one * otherScale;
            }
        }
    }
    #endregion
}