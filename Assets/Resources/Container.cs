using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Container : MonoBehaviour
{
    [SerializeField] GameObject plane;
    [SerializeField] public GameObject controller;
    public static Dictionary<GameObject, (string Name, int Id)> container = new Dictionary<GameObject, (string Name, int Id)>();
    public static Dictionary<Vector3, int> originalContainer = new Dictionary<Vector3, int>();
    public static List<GameObject> poyos = new List<GameObject>();
    public static int cpt = 0;
    public int id;
    private GameObject grabed;
    private Vector3 initPos;
    private static int deadChicken = 0;

    // Start is called before the first frame update
    void Start()
    {
        cpt++;
        id = cpt;
        string objectName = gameObject.name.Split(char.Parse(" "))[0];
        if(objectName != "Toon"){
            container.Add(gameObject, (Name:objectName, Id:id));
            originalContainer.Add(gameObject.transform.position, id);
        }
        else{
            poyos.Add(gameObject);
        }
    }

    public static void Reset(){
        foreach (KeyValuePair<GameObject, (string Name, int Id)> entry2 in container)
        {
            foreach (KeyValuePair<Vector3, int> entry in originalContainer)
            {
                if(entry.Value == entry2.Value.Id){
                    entry2.Key.transform.position = entry.Key;
                }
            }
            if(entry2.Value.Name == "MobSpawner"){
                entry2.Key.GetComponent<ChickenSpawner>().Restart();
            }
            else if(entry2.Value.Name == "FinishPoint"){
                entry2.Key.GetComponent<FinishLine>().Restart();
            }

        }
        foreach(GameObject poyo in poyos){
            Destroy(poyo);
        }
        poyos = new List<GameObject>();

        StaticSetOffPlates();

    }
    public static void addDead(){
        deadChicken += 1;
    }

    public void OnRayEnter()
    {
        if (gameObject.GetComponent<Outline>() != null)
        {
            gameObject.GetComponent<Outline>().enabled = true;
        }
    }

    public void OnRayExit()
    {
        if (gameObject.GetComponent<Outline>() != null)
        {
            gameObject.GetComponent<Outline>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool verif2 = true;
        bool verif3 = true;
        GameObject finish = new GameObject();
        int max = 0;
        foreach (KeyValuePair<GameObject, (string Name, int Id)> entry2 in container)
        {
            if(entry2.Value.Name == "MobSpawner"){
                max = entry2.Key.GetComponent<ChickenSpawner>().getMax();
                if(entry2.Key.GetComponent<ChickenSpawner>().getUnspawned() != 0) verif2 = false;
            }
            else if(entry2.Value.Name == "FinishPoint"){
                finish = entry2.Key;
                if(entry2.Key.GetComponent<FinishLine>().getToEnd() <= 0) verif3 = false;
            }

        }
        if(deadChicken == max && verif2 && verif3){
            finish.GetComponent<FinishLine>().setDefeat();
        }
    }

    public static void StaticSetOffPlates(){
        foreach(KeyValuePair<GameObject, (string Name, int Id)> entry2 in container){
            if( entry2.Value.Name == "Plate"){
                entry2.Key.GetComponent<Plate>().setOff();
            }
        }
    }

   
    public void SetOffPlates(){
        foreach(KeyValuePair<GameObject, (string Name, int Id)> entry2 in container){
            if( entry2.Value.Name == "Plate"){
                entry2.Key.GetComponent<Plate>().setOff();
            }
        }
    }

    public void getHit()
    {
        Vector3 pos = new Vector3(0,0,0);
        Vector3 nor = new Vector3(0,0,0);
        Int32 il = 0;
        bool tar = false;
        controller.GetComponent<XRRayInteractor>().TryGetHitInfo(out pos,out nor,out il,out tar);
        if(pos != null)
        {
            if(pos.y < 3 && pos.y > -2 && pos.x >= 0 && pos.x <= 9 && pos.z >= 0 && pos.z <= 9)
            {
                foreach(KeyValuePair<GameObject, (string Name, int Id)> entry2 in container){
                    if(entry2.Key.transform.position.z == (int)Math.Round(pos.z) && entry2.Key.transform.position.x == (int)Math.Round(pos.x) && entry2.Key.layer == LayerMask.NameToLayer("Grabable Items")){
                        grabed = entry2.Key;
                        initPos = entry2.Key.transform.position;
                        break;
                    }
                }
            }
        }
    }

    public void moveAtHit()
    {
        Vector3 pos = new Vector3(0,0,0);
        Vector3 nor = new Vector3(0,0,0);
        Int32 il = 0;
        bool tar = false;
        controller.GetComponent<XRRayInteractor>().TryGetHitInfo(out pos, out nor,out il,out tar);
        if(pos.y < 3 && pos.y > -2 && pos.x >= 0 && pos.x <= 9 && pos.z >= 0 && pos.z <= 9){
            grabed.transform.position = new Vector3(pos.x, pos.y + grabed.transform.localScale.y/2f, pos.z);
        }
    }

    public void setPos()
    {
        Vector3 pos = new Vector3(0,0,0);
        Vector3 nor = new Vector3(0,0,0);
        Int32 il = 0;
        bool tar = false;
        controller.GetComponent<XRRayInteractor>().TryGetHitInfo(out pos, out nor,out il,out tar);
        if(pos.y < 3 && pos.y > -2 && pos.x >= 0 && pos.x <= 9 && pos.z >= 0 && pos.z <= 9){
            grabed.transform.position = new Vector3((int)Math.Round(pos.x), pos.y + grabed.transform.localScale.y/2f, (int)Math.Round(pos.z));
        }
        else
        {
            grabed.transform.position = initPos;
        }
    }
}
