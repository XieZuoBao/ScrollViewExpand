using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 分页滚动样式
/// </summary>
public enum PageScrollType
{
    /// <summary>
    /// 水平布局样式
    /// </summary>
    HORIZONTAL = 0,
    /// <summary>
    /// 竖直布局样式
    /// </summary>
    VERTICAL,
}

/// <summary>
/// 分页滚动
/// </summary>
public class PageScrollView : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    #region 字段
    /// <summary>
    /// 分页滚动样式--默认水平
    /// </summary>
    public PageScrollType pageScrollType = PageScrollType.HORIZONTAL;
    /// <summary>
    /// 移动动画时长
    /// </summary>
    public float moveTime = 0.3f;
    /// <summary>
    /// 自动滚动开关
    /// </summary>
    public bool IsAutoScroll = false;
    /// <summary>
    /// 自动滚动时间间隔
    /// </summary>
    public float AutoScrollTime = 2f;
    /// <summary>
    /// ScrollRect组件
    /// </summary>
    protected ScrollRect rect;
    /// <summary>
    /// Content节点
    /// </summary>
    protected RectTransform content;
    /// <summary>
    /// 页数
    /// </summary>
    protected int pagesCount;
    /// <summary>
    /// 分页的滑动条值
    /// </summary>
    public float[] pages;
    /// <summary>
    /// 移动计时器
    /// </summary>
    private float timer = 0;
    /// <summary>
    /// 开始移动的位置
    /// </summary>
    private float startMovePos;
    /// <summary>
    /// 当前页索引位置
    /// </summary>
    protected int currentPage;
    /// <summary>
    /// 移动开关
    /// </summary>
    private bool isMoving = false;
    /// <summary>
    /// 拖拽开关
    /// </summary>
    private bool isDraging = false;
    /// <summary>
    /// 自动滚动计时器
    /// </summary>
    private float autoScrollTimer = 0;
    #endregion

    #region Unity回调
    protected virtual void Start()
    {
        Init();
    }

    protected virtual void Update()
    {
        //手动拖拽结束后,滑动到最近的分页
        MoveToTargetPage();
        //自动滑页启用时,按设计自动滑页
        AutoScroll();
    }

    /// <summary>
    /// 开始拖拽事件
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        //开始拖拽
        isDraging = true;
    }

    /// <summary>
    /// 手动拖拽结束事件
    /// input事件在update之前执行
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        //停止拖拽
        isDraging = false;
        //手动改拖拽结束时,自动滚动计时器清零,防止拖拽结束后直接自动滚动的不好体验
        autoScrollTimer = 0;
        //拖拽结束时的回调(设置离的最近的分页索引,设置开始移动的位置)
        EndDragCallback(CalulateMinDistancePage());
    }
    #endregion

    #region 方法
    private void Init()
    {
        rect = transform.GetComponent<ScrollRect>();
        if (rect == null)
            Debug.LogError("未查询到ScrollView!!!");
        content = transform.Find("Viewport/Content").GetComponent<RectTransform>();
        //页数=content的子物体数量
        pagesCount = content.childCount;
        if (pagesCount == 1)
            Debug.LogError("只有一页是不用进行分页滚动的!!!");
        pages = new float[pagesCount];
        //给各分页对应的滑动条值进行赋值
        for (int i = 0; i < pages.Length; i++)
        {
            switch (pageScrollType)
            {
                //水平方向,分页滑动条的值时正序排列
                case PageScrollType.HORIZONTAL:
                    pages[i] = i * (1.0f / (pagesCount - 1));
                    break;
                //竖直方向,分页滑动条的值是倒序排列
                case PageScrollType.VERTICAL:
                    pages[i] = 1 - i * (1.0f / (pagesCount - 1));
                    break;
            }
        }
    }

    /// <summary>
    /// 移动到指定页(距离最近的分页)
    /// </summary>
    private void MoveToTargetPage()
    {
        if (isMoving)
        {
            timer += Time.deltaTime * (1 / moveTime);
            //更新滑动到指定分页
            switch (pageScrollType)
            {
                case PageScrollType.HORIZONTAL:
                    rect.horizontalNormalizedPosition = Mathf.Lerp(startMovePos, pages[currentPage], timer);
                    break;
                case PageScrollType.VERTICAL:
                    rect.verticalNormalizedPosition = Mathf.Lerp(startMovePos, pages[currentPage], timer);
                    break;
            }
            if (timer >= 1)
                isMoving = false;
        }
    }

    /// <summary>
    /// 自动分页滚动
    /// </summary>
    private void AutoScroll()
    {
        //手动拖拽时,禁止自动分页滚动
        if (isDraging)
            return;
        //开启自动滚动条件时,开始累计计时,计时满足后,滚动一页
        if (IsAutoScroll)
        {
            autoScrollTimer += Time.deltaTime;
            if (autoScrollTimer >= AutoScrollTime)
            {
                //计时器清零
                autoScrollTimer = 0;
                currentPage++;
                //保证数据不越界,循环滚动
                currentPage %= pagesCount;
                EndDragCallback(currentPage);
            }
        }
    }

    /// <summary>
    /// 拖拽结束时的回调(设置离的最近的分页索引,设置开始移动的位置)
    /// </summary>
    /// <param name="page"></param>
    private void EndDragCallback(int page)
    {
        //开启移动开关
        isMoving = true;
        //开始移动倒计时
        timer = 0;
        //离的最近的分页
        this.currentPage = page;
        //设置开始移动的位置
        switch (pageScrollType)
        {
            case PageScrollType.HORIZONTAL:
                startMovePos = rect.horizontalNormalizedPosition;
                break;
            case PageScrollType.VERTICAL:
                startMovePos = rect.verticalNormalizedPosition;
                break;
        }

        //此处可以向外分发滚动到某页事件
    }

    /// <summary>
    /// 计算手动拖拽结束时距离最近的分页
    /// <para>1.横向拖拽结束时,通过遍历比较分页的滑动条值与rect.horizontalNormalizedPosition的距离关系,确定最近的分页</para>
    /// <para>2.纵向拖拽结束时,通过遍历比较分页的滑动条值与rect.verticalNormalizedPosition的距离关系,确定最近的分页</para>
    /// </summary>
    protected int CalulateMinDistancePage()
    {
        int minPage = 0;
        for (int i = 1; i < pages.Length; i++)
        {
            switch (pageScrollType)
            {
                case PageScrollType.HORIZONTAL:
                    if (Mathf.Abs(pages[i] - rect.horizontalNormalizedPosition) < Mathf.Abs(pages[minPage] - rect.horizontalNormalizedPosition))
                    {
                        minPage = i;
                    }
                    break;
                case PageScrollType.VERTICAL:
                    if (Mathf.Abs(pages[i] - rect.verticalNormalizedPosition) < Mathf.Abs(pages[minPage] - rect.verticalNormalizedPosition))
                    {
                        minPage = i;
                    }
                    break;
            }
        }
        return minPage;
    }
    #endregion
}