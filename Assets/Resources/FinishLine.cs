using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinishLine : MonoBehaviour
{
    public int pouletsNecessaires;
    public Text text;
    // Start is called before the first frame update
    void Start()
    {
        text.text = pouletsNecessaires.ToString();
    }

    public void Restart(){
        pouletsNecessaires = 1;
        text.text = pouletsNecessaires.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setDefeat(){
        text.text = "Perdu";
    }

    public int getToEnd(){
        return pouletsNecessaires;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Chicken")
        {
            Destroy(collision.gameObject);
            pouletsNecessaires--;
            text.text = pouletsNecessaires.ToString();
            if(pouletsNecessaires<=0)
            {
                text.text = "Gagné";
            }
        }
    }
}
