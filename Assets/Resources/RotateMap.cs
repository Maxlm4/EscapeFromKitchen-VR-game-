using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateMap : MonoBehaviour
{
    GameObject world;
    Vector3 mouseReference;

    // Start is called before the first frame update
    void Start()
    {
        world = gameObject.transform.parent.parent.parent.gameObject;
    }
    /*
    void OnMouseDown()
    {
        mouseReference = Input.mousePosition;
    }

    void OnMouseDrag()
    {
        Vector3 offset = (Input.mousePosition - mouseReference);
        world.transform.RotateAround(new Vector3(4.5f, 0, 4.5f), Vector3.up, offset.x * 2f * Time.deltaTime);
    }
    */

    void OnMouseEnter()
    {
        if (gameObject.GetComponent<Outline>() != null)
        {
            gameObject.GetComponent<Outline>().enabled = true;
        }
    }

    void OnMouseExit()
    {
        if (gameObject.GetComponent<Outline>() != null)
        {
            gameObject.GetComponent<Outline>().enabled = false;
        }
    }

    public void RotateTheMap(float value)
    {
        world.transform.RotateAround(new Vector3(4.5f, 0, 4.5f), Vector3.up, 45 * value * Time.deltaTime);
    }

    // Update is called once per frame
    void Update()
    {
        /*if(Input.GetKey("q")){
            RotateTheMap(true);
        }
        else if(Input.GetKey("d")){
            RotateTheMap(false);
        }*/
    }
}
