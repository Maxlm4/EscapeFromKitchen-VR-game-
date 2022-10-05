using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plate : MonoBehaviour
{
    [SerializeField] private float countdown;
    [SerializeField] private bool isOn;
    // time in seconds
    private const float MAXCOUNTDOWN = 10f;
    private Renderer rend;
    private Material fired;
    private Material off;

    // Start is called before the first frame update
    void Start()
    {
        isOn = false;
        countdown = MAXCOUNTDOWN;
        rend = transform.GetChild(1).GetComponent<Renderer>();
        off = Resources.Load("Off", typeof(Material)) as Material;
        fired = Resources.Load("Fired", typeof(Material)) as Material;
    }

    public bool getOn(){
        return isOn;
    }

    public void setOff(){
        rend.material = off;
        countdown = MAXCOUNTDOWN;
        isOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isOn){
            countdown -= 1 * Time.deltaTime;
            if(countdown <= 0){
                isOn = true;
                rend.material = fired;
            }
        }

        /*if (Input.GetKeyDown("space") && isOn)
        {
            setOff();
        }*/
    }
}
