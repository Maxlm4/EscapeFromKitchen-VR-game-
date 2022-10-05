using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabable : MonoBehaviour
{
    
    private Vector3 mOffset;
    private float mZCoord;
    void OnMouseDown()
    {
        mZCoord = Camera.main.WorldToScreenPoint(
            gameObject.transform.position).z;
        // Store offset = gameobject world pos - mouse world pos
        mOffset = gameObject.transform.position - GetMouseAsWorldPoint();
    }
    
    
    private Vector3 GetMouseAsWorldPoint()
    {
        /*
        // Pixel coordinates of mouse (x,y)
        Vector3 mousePoint = Input.mousePosition;

        // z coordinate of game object on screen
        mousePoint.z = mZCoord;

        // Convert it to world points
        return Camera.main.ScreenToWorldPoint(mousePoint);*/
        return new Vector3 (0,0,0);
    }
    

    void OnMouseDrag()
    {
        transform.position = new Vector3(Mathf.Round(GetMouseAsWorldPoint().x + mOffset.x), transform.position.y, Mathf.Round(GetMouseAsWorldPoint().z + mOffset.z));
    }
}
