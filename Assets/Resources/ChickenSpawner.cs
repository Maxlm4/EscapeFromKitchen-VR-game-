using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenSpawner : MonoBehaviour
{
    [SerializeField] public int chickenCount;
    [SerializeField] public GameObject chicken;
    [SerializeField] public float rotation;
    [SerializeField] public Vector3 position;
    [SerializeField] public Vector3 direction;
    private int chickenMax;
    float spawnTime = 2;
    private float movementSpeed = 2f;
    
    

    void Start()
    {
        float timeleft = spawnTime;
        chickenMax = chickenCount;
        
    }
    
    public void Restart()
    {
        spawnTime = 2;
        movementSpeed = 2f;
        chickenCount = 5;
    }

    public int getUnspawned(){
        return chickenCount;
    }
    public int getMax(){
        return chickenMax;
    }

    // Update is called once per frame
    void Update()
    {
        if (chickenCount > 0){

            spawnTime -= Time.deltaTime;
            if (spawnTime <= 0f){
                
                GameObject chick = Instantiate(chicken);
                chick.GetComponent<ChickenMove>().Init(rotation,direction,position);
                
                chickenCount--;
                spawnTime = 2;
            }
        }
    }
}
