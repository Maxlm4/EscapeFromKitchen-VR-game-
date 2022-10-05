using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ChickenMove : MonoBehaviour
{
    Animator animator;
    public AudioSource audioData;
    public AudioSource audioData2;
    private float movementSpeed = 2f;
    [SerializeField] private Vector3 direction;
    private Dictionary<GameObject, (string Name, int Id)> container;
    private Renderer rend;
    [SerializeField] private bool isFlying;//si le poulet à décollé après avoir marché sur une spatule par exemple
    private float phase;//permet de s'assurer qu'un saut dure une demi seconde
    private int step;//règle la hauteur de départ du saut
    private int hasTarget;//l'id de la cible du poulet s'il en a une
    private int yOrientation; //orientation en y du poulet pour éviter les bugs
    private float cooldown; //cooldown at death before object destruction
    private bool death; //chicken is dead
    private float honk; //chicken honking

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        hasTarget = -1;
        rend = transform.GetChild(4).GetComponent<Renderer>();
        isFlying = false;
        phase = 0f;
        step = 0;
        yOrientation = 0;
        audioData = GetComponents<AudioSource>()[0];
        audioData2 = GetComponents<AudioSource>()[1];
        cooldown = 0f;
        death = false;
        animator.SetBool("Walk", true);
        honk = UnityEngine.Random.Range(4f,20f);
    }

    public void Init(float rotation, Vector3 direction, Vector3 position){
        transform.position = position;
        transform.Rotate(0,rotation,0);
        this.direction = direction;
    }

    void OnCollisionEnter(Collision collision)
    {
        isFlying = false;
        if (collision.contacts[0].normal.y != 1)
        {
            transform.Rotate(0,180,0);
            yOrientation = (yOrientation + 180) % 360;
            direction*=-1;
            hasTarget = -1;
        }
    }

    private void Deactivate()
    {
        death = true;
        cooldown = 1;
        audioData.Play(0);
        gameObject.GetComponent<BoxCollider>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(death){
            cooldown -= Time.deltaTime;
            if(cooldown <= 0){
                Destroy(gameObject);
            }
            else if(cooldown <= 0.5f){
                gameObject.transform.GetChild(4).GetComponent<SkinnedMeshRenderer>().enabled = false;
            }
        }
        else
        {
            container = Container.container;
            honk -= Time.deltaTime;
            if(honk <= 0f)
            {
                honk = UnityEngine.Random.Range(4f,10f);
                audioData2.Play(0);
            }

            //change position
            if(direction.z !=0){
                transform.position = new Vector3((int)Math.Round(transform.position.x), transform.position.y, transform.position.z) + direction * movementSpeed * Time.deltaTime;
                if(transform.position.z >= 9){
                    transform.position = new Vector3(transform.position.x, transform.position.y, 9);
                    transform.Rotate(0,180,0);
                    yOrientation = (yOrientation + 180) % 360;
                    direction*=-1;
                }
                else if(transform.position.z <= 0){
                    transform.position = transform.position = new Vector3(transform.position.x, transform.position.y, 0);
                    transform.Rotate(0,180,0);
                    yOrientation = (yOrientation + 180) % 360;
                    direction*=-1;
                }
            }
            else if(direction.x !=0){
                transform.position = new Vector3(transform.position.x, transform.position.y, (int)Math.Round(transform.position.z)) + direction * movementSpeed * Time.deltaTime;
                if(transform.position.x >= 9){
                    transform.position = new Vector3(9, transform.position.y, transform.position.z);
                    transform.Rotate(0,180,0);
                    yOrientation = (yOrientation + 180) % 360;
                    direction*=-1;
                }
                else if(transform.position.x <= 0){
                    transform.position = transform.position = new Vector3(0, transform.position.y, transform.position.z);
                    transform.Rotate(0,180,0);
                    yOrientation = (yOrientation + 180) % 360;
                    direction*=-1;
                }
            }
            if (isFlying)
            {
                float now = Time.time;
                transform.position =  new Vector3(transform.position.x, step + 1.5f * (float)Math.Sin((now  - phase) * Math.PI / 0.5f), transform.position.z);
            }

            if (transform.position.y < 0){
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            }

            transform.eulerAngles = new Vector3(0, yOrientation, 0);

            foreach(KeyValuePair<GameObject, (string Name, int Id)> entry in container)
            {
                if(Math.Abs(transform.position.z - entry.Key.transform.position.z) <= 0.03 && Math.Abs(transform.position.x - entry.Key.transform.position.x) <= 0.03 && Math.Abs(transform.position.y - entry.Key.transform.position.y) <= 0.3){
                    if(hasTarget == entry.Value.Id){
                        hasTarget = -1;
                    }
                    else if (entry.Value.Name == "Plate")
                    {
                        
                        if (entry.Key.GetComponent<Plate>().getOn()){
                            Container.addDead();
                            Destroy(gameObject);
                        }
                    }
                    else if(entry.Value.Name == "BoilingOil"){
                        //rend.material = Resources.Load("Fired", typeof(Material)) as Material;
                        bool verif = false;
                        foreach(KeyValuePair<GameObject, (string Name, int Id)> entry2 in container){
                            if(entry2.Key.transform.position.z - entry.Key.transform.position.z == 0 && entry2.Key.transform.position.x - entry.Key.transform.position.x == 0 && Math.Abs(transform.position.y - entry.Key.transform.position.y) <= 0.3 && entry2.Value.Name == "CuttingBoard"){
                                verif = true;
                                break;
                            }
                        }
                        if(!verif){
                            Container.addDead();
                            Destroy(gameObject);
                        }
                    }
                    else if (entry.Value.Name == "Spatule")
                    {
                        isFlying = true;
                        phase = Time.time;
                        step = (int)Math.Round(entry.Key.transform.position.y);
                        entry.Key.GetComponent<AudioSource>().Play(0);
                    }
                }
            }

            if(hasTarget == -1){
                if(direction.z == 1 && Math.Abs(transform.position.z - (int)Math.Round(transform.position.z)) <= 0.05){
                    foreach(KeyValuePair<GameObject, (string Name, int Id)> entry in container)
                    {
                        if(entry.Key.transform.position.z == (int)Math.Round(transform.position.z) && entry.Value.Name == "Grains"){
                            if(transform.position.x < entry.Key.transform.position.x){
                                transform.position = new Vector3(transform.position.x, transform.position.y, (int)Math.Round(transform.position.z));
                                transform.Rotate(0,90,0);
                                yOrientation = (yOrientation + 90) % 360;
                                direction=new Vector3(1,0,0);
                                hasTarget = entry.Value.Id;
                                break;
                            }
                            else if(transform.position.x > entry.Key.transform.position.x){
                                transform.position = new Vector3(transform.position.x, transform.position.y, (int)Math.Round(transform.position.z));
                                transform.Rotate(0,-90,0);
                                yOrientation = (yOrientation - 90) % 360;
                                direction=new Vector3(-1,0,0);
                                hasTarget = entry.Value.Id;
                                break;
                            }   
                        }
                    }
                }
                else if(direction.z == -1 && Math.Abs(transform.position.z - (int)Math.Round(transform.position.z)) <= 0.05){
                    foreach(KeyValuePair<GameObject, (string Name, int Id)> entry in container)
                    {
                        if(entry.Key.transform.position.z == (int)Math.Round(transform.position.z) && entry.Value.Name == "Grains"){
                            if(transform.position.x < entry.Key.transform.position.x){
                                transform.position = new Vector3(transform.position.x, transform.position.y, (int)Math.Round(transform.position.z));
                                transform.Rotate(0,-90,0);
                                yOrientation = (yOrientation - 90) % 360;
                                direction=new Vector3(1,0,0);
                                hasTarget = entry.Value.Id;
                                break;
                            }
                            else if(transform.position.x > entry.Key.transform.position.x){
                                transform.position = new Vector3(transform.position.x, transform.position.y, (int)Math.Round(transform.position.z));
                                transform.Rotate(0,90,0);
                                yOrientation = (yOrientation + 90) % 360;
                                direction=new Vector3(-1,0,0);
                                hasTarget = entry.Value.Id;
                                break;
                            }
                        }
                    }
                }
                else if(direction.x == 1 && Math.Abs(transform.position.x - (int)Math.Round(transform.position.x)) <= 0.05){
                    container = Container.container;
                    foreach(KeyValuePair<GameObject, (string Name, int Id)> entry in container)
                    {
                        if(entry.Key.transform.position.x == (int)Math.Round(transform.position.x) && entry.Value.Name == "Grains"){
                            if(transform.position.z < entry.Key.transform.position.z){
                                transform.position = new Vector3((int)Math.Round(transform.position.x), transform.position.y, transform.position.z);
                                transform.Rotate(0,-90,0);
                                yOrientation = (yOrientation - 90) % 360;
                                direction=new Vector3(0,0,1);
                                hasTarget = entry.Value.Id;
                                break;
                            }
                            else if(transform.position.z > entry.Key.transform.position.z){
                                transform.position = new Vector3((int)Math.Round(transform.position.x), transform.position.y, transform.position.z);
                                transform.Rotate(0,90,0);
                                yOrientation = (yOrientation + 90) % 360;
                                direction=new Vector3(0,0,-1);
                                hasTarget = entry.Value.Id;
                                break;
                            }
                        }
                    }
                }
                else if(direction.x == -1 && Math.Abs(transform.position.x - (int)Math.Round(transform.position.x)) <= 0.05){
                    container = Container.container;
                    foreach(KeyValuePair<GameObject, (string Name, int Id)> entry in container)
                    {
                        if(entry.Key.transform.position.x == (int)Math.Round(transform.position.x) && entry.Value.Name == "Grains"){
                            if(transform.position.z < entry.Key.transform.position.z){
                                transform.position = new Vector3((int)Math.Round(transform.position.x), transform.position.y, transform.position.z);
                                transform.Rotate(0,90,0);
                                yOrientation = (yOrientation + 90) % 360;
                                direction=new Vector3(0,0,1);
                                hasTarget = entry.Value.Id;
                                break;
                            }
                            else if(transform.position.z > entry.Key.transform.position.z){
                                transform.position = new Vector3((int)Math.Round(transform.position.x), transform.position.y, transform.position.z);
                                transform.Rotate(0,-90,0);
                                yOrientation = (yOrientation - 90) % 360;
                                direction=new Vector3(0,0,-1);
                                hasTarget = entry.Value.Id;
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}

