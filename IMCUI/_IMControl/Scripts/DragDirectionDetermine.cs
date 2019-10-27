using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDirectionDetermine : MonoBehaviour
{
    public enum Direction
    {
        None,
        X,
        Y
    }
    Direction nowDirection = Direction.None;

    public List<ScrollRect> verticalScrollRect;//垂直
    public List<ScrollRect> horizontalScrollRect;//水平

    bool changeFlag = true;

    void Update()
    {
        Determine();
    }
    void Determine()
    {
        if (Input.GetMouseButton(0) && changeFlag)
        {
            if (Mathf.Abs(Input.GetAxis("Mouse Y")) > Mathf.Abs(Input.GetAxis("Mouse X")))
            {
                nowDirection = Direction.Y;
                changeFlag = false;
            }
            else if (Mathf.Abs(Input.GetAxis("Mouse Y")) < Mathf.Abs(Input.GetAxis("Mouse X")))
            {
                nowDirection = Direction.X;
                changeFlag = false;
            }
            DirectionSwitch();
        }
        if (Input.GetMouseButtonUp(0))
        {
            nowDirection = Direction.None;
            changeFlag = true;
            DirectionSwitch();
        }
    }
    void DirectionSwitch()
    {
        switch (nowDirection)
        {
            case Direction.None:
                SetSwitch(true, true);
                break;
            case Direction.X:
                SetSwitch(true, false);
                break;
            case Direction.Y:
                SetSwitch(false, true);
                break;
        }
    }
    void SetSwitch(bool x, bool y)
    {
        for (int i = 0; i < verticalScrollRect.Count; i++)
        {
            if (verticalScrollRect[i])
                verticalScrollRect[i].enabled = y;
        }
        for (int i = 0; i < horizontalScrollRect.Count; i++)
        {
            if (horizontalScrollRect[i])
                horizontalScrollRect[i].enabled = x;
        }
    }
}
