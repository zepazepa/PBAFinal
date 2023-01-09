using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    public Rigidbody rigidBody;
    public float kecepatanMaksimal;

    public float akselerasiMaju = 8f, akselerasiMundur = 4f;
    private float inputKecepatan = 30f;

    public float kekuatanBelokan = 180f;
    private float inputBelokan;

    private int nextCheckpoint;
    public int currentlap;

    public bool isAI;
    public int targetSekarang;
    private Vector3 targetTitik;
    public float kecepatanAI = 1f;
    public float kecepatanAIBelok = 0.8f;
    public float jarakAI = 5f;
    public float aiVariance = 3f;
    public float aiMaksimalBelok = 1f;
    private float inputSpeedAI;

    // Start is called before the first frame update
    void Start()
    {
        //agar model mobil saja yang bergerak. bukan keduanya. karena sphere (Rb) adalah child dari player car
        rigidBody.transform.parent = null;

        if (isAI)
        {
            targetTitik = RaceManager.instance.allCheckPoint[targetSekarang].transform.position;
            RandomAITarget();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAI)
        {
            inputKecepatan = 0f;
            if (Input.GetAxis("Vertical") > 0)
            {
                inputKecepatan = Input.GetAxis("Vertical") * akselerasiMaju;
            }
            else if (Input.GetAxis("Vertical") < 0)
            {
                inputKecepatan = Input.GetAxis("Vertical") * akselerasiMundur;
            }

            //quaternion
            inputBelokan = Input.GetAxis("Horizontal");
            if (Input.GetAxis("Vertical") != 0)
            {
            /*Mathf.Sign(inputKecepatan) agar ketika belok mundur di reverse. jika math sign bernilai positif akan return angka 1 dan sebaliknya akan return angka -1*/
            /*rigidBody.velocity.magnitude = seberapa cepat rigid body bergerak sekarang. kecepatan tsb dibagi dgn max speed agar cocok dilihat secara visual*/
            /*contoh: rb velo sekarang 10f dan max speed juga 10f, makan mobil berbelok dengan maksimal yaitu(1) sedangkan jika rb velo 5f maka mobil berbelok 1 / 2(0.5)*/

                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, inputBelokan * kekuatanBelokan * Time.deltaTime * Mathf.Sign(inputKecepatan)
                    * (rigidBody.velocity.magnitude / kecepatanMaksimal), 0f));
            }

            //posisi player car = posisi rigid body yang berjalan.
            //diletakan di update karena secara visual Update yang dilihat oleh player secara jelas. 
            
        }
        else
        {
            
            targetTitik.y = transform.position.y;

            if (Vector3.Distance(transform.position, targetTitik) < jarakAI)
            {
                targetSekarang++;
                if (targetSekarang >= RaceManager.instance.allCheckPoint.Length)
                {
                    targetSekarang = 0;
                }
                targetTitik = RaceManager.instance.allCheckPoint[targetSekarang].transform.position;
                RandomAITarget();
            }

            Vector3 targetTujuan = targetTitik - transform.position;
            float sudut = Vector3.Angle(targetTujuan, transform.forward);

            Vector3 posisiLokal = transform.InverseTransformPoint(targetTitik);
            if(posisiLokal.x < 0f)
            {
                sudut = -sudut;
            }

            inputBelokan = Mathf.Clamp(sudut / aiMaksimalBelok, -1f, 1f);

            inputSpeedAI = 1f;
            inputKecepatan = inputSpeedAI * akselerasiMaju;
        }
        transform.position = rigidBody.position;
    }

    private void FixedUpdate()
    {

        //agar konsisten walaupun framer ates beda
        // inputKecepatan dikali 1000 karena drag rigidBody yg besar dan agar acceleration 
        // tidak bernilai besar juga
        rigidBody.AddForce(transform.forward * inputKecepatan * 1000f);
        
        if(rigidBody.velocity.magnitude > kecepatanMaksimal)
        {
            rigidBody.velocity = rigidBody.velocity.normalized * kecepatanMaksimal;
        }
    }

    public void CheckPointHit(int cpNumber)
    {
        Debug.Log(cpNumber);
        if (cpNumber == nextCheckpoint)
        {
            nextCheckpoint++;

            if (nextCheckpoint == RaceManager.instance.allCheckPoint.Length)
            {
                nextCheckpoint = 0;
                currentlap++;
            }
        }
    }

    public void RandomAITarget()
    {
        targetTitik += new Vector3(Random.Range(-aiVariance, aiVariance), 0f, Random.Range(-aiVariance, aiVariance));
    }

    public void LapComplete()
    {
         
    }
}
