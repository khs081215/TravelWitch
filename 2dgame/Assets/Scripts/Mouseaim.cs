using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
/**
 * 에임 UI를 마우스의 위치와 동기화합니다.
 */
public class Mouseaim : MonoBehaviour
{
    private Vector2 mouseLocation;
    
    // Update is called once per frame
    void Update()
    {
        mouseLocation= Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector2((float)Math.Round(mouseLocation.x,2),(float)Math.Round(mouseLocation.y, 2));
    }
}
