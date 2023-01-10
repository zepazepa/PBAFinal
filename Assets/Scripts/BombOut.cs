using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class BombOut : MonoBehaviour
{
    public GameObject ProjectilePrefab;
    public GameObject SpawnPoint;
    Vector3 targetPos;
    Rigidbody rb;
    public float bombSpeed = 5f;
    bool isClicked = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetButtonDown("Fire1") == true)
        //{
        //    isClicked = true;
        //}
        //else
        //{
        //    isClicked = false;
        //}
        if (Input.GetButtonDown("Fire1"))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //RaycastHit hit;
            //if (Physics.Raycast(ray, out hit))
            //{
            //    targetPos = hit.point;

            GameObject ball = Instantiate(ProjectilePrefab, SpawnPoint.transform.position, SpawnPoint.transform.rotation);
            ball.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0f, 800f * ProjectilePrefab.GetComponent<Rigidbody>().mass, 0f));
        }
    }

    private void FixedUpdate()
    {
       
    }


}

