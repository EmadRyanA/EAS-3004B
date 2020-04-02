using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TileManager : MonoBehaviour
{
    public GameObject[] tilePrefabs;
    private List<GameObject> activeTiles;

    private Transform shiftPlayer;
    private float playerArea = 75.0f;
    private float zCreated = -15.0f;
    private float tileLength = 75.0f;
    public int maxTileCount = 9;
    private int lastPrefab;

    public void init(){
        // initialize variables
        playerArea = 75.0f;
        zCreated = -15.0f;
        tileLength = 75.0f;
        maxTileCount = 9;
        lastPrefab = 0;

        shiftPlayer = GameObject.FindGameObjectWithTag("Player").transform;
        activeTiles = new List<GameObject>();
        for(int n = 0; n < maxTileCount; n++)
        {
            if (n < 2)
            {
                SpawnTile(0);
            }
            else
            {
                SpawnTile();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        init();
    }

    

    // Update is called once per frame
    void Update()
    {
        if(shiftPlayer.position.z - playerArea > (zCreated - maxTileCount * tileLength))
        {

            SpawnTile();
            DeleteTile();
        }
    }

    private void SpawnTile(int prefabIndex = -1)
    {
        GameObject tileIns;
        if(prefabIndex == -1)
        {
            tileIns = Instantiate(tilePrefabs[RandomPrefab()]) as GameObject;
            //tileIns = Instantiate(tilePrefabs[0]) as GameObject;

        }
        else
        {
            tileIns = Instantiate(tilePrefabs[prefabIndex]) as GameObject;
        }
        tileIns.transform.SetParent(transform);
        tileIns.transform.position = Vector3.forward * zCreated;
        zCreated += tileLength;
        activeTiles.Add(tileIns);
    }

    private void DeleteTile()
    {
        Destroy(activeTiles[0]);
        activeTiles.RemoveAt(0);
    }

    private int RandomPrefab()
    {
        if (tilePrefabs.Length <= 1)
        {
            return 0;
        }
        
        System.Random r = new System.Random();
        int randInt = r.Next(0,100);
        int rngIndex = lastPrefab;
        
        if(randInt>= 0 && randInt < 30){
            // generate empty env
            rngIndex = 0;
        }else if(randInt >= 30 && randInt < 80){
            // generate tree env
            rngIndex = 2;
        }else{
            // generate building env
            rngIndex = 1;
        }
        
        
        //while(rngIndex == lastPrefab)
        //{
            //rngIndex = Random.Range(0,tilePrefabs.Length);
        //}
        lastPrefab = rngIndex;
        return rngIndex;
    }
}
