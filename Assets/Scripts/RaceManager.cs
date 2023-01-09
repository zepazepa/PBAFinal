using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    public static RaceManager instance;
    
    public CheckPoints[] allCheckPoint;

    public void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < allCheckPoint.Length; i++)
        {
            allCheckPoint[i].checkpointNumber = i;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
