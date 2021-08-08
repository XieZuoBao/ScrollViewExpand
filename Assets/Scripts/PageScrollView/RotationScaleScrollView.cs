using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 分页滚动的拓展-----旋转+缩放
/// </summary>
public class RotationScaleScrollView : ScalePageScrollView
{
    public float rotation;

    protected override void Update()
    {
        base.Update();
        RotationHorizontal();
    }

    /// <summary>
    /// 旋转效果
    /// </summary>
    void RotationHorizontal()
    {
        if (nextPage == lastPage)
            return;

        float percent = (rect.horizontalNormalizedPosition - pages[lastPage]) / (pages[nextPage] - pages[lastPage]);

        items[lastPage].transform.localRotation = Quaternion.Euler(-Vector3.Lerp(Vector3.zero, Vector3.up * rotation, percent));
        items[nextPage].transform.localRotation = Quaternion.Euler(Vector3.Lerp(Vector3.zero, Vector3.up * rotation, 1 - percent));

        for (int i = 0; i < items.Length; i++)
        {
            if (i != nextPage && i != lastPage)
            {
                if (i < currentPage)
                    items[i].transform.localRotation = Quaternion.Euler(-Vector3.up * rotation);
                else if (i > currentPage)
                    items[i].transform.localRotation = Quaternion.Euler(Vector3.up * rotation);
            }
        }
    }
}
