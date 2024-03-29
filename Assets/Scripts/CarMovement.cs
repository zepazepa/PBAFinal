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
    public int lapSekarang;

    public bool apakahAI;
    public int targetSekarang;
    private Vector3 titikTujuan;
    public float kecepatanAI = 1f, kecepatanBelokAI = 0.8f, rentangTitikAI = 5f, varianPointAI = 3f, kecepatanMaksBelokAI = 15f;
    private float inputKecepatanAI, kecepatanAiAcak;

    public Transform banDepanKiri, banDepanKanan;
    public float kecepatanMaksBelokBan = 25f;

    public ParticleSystem[] debuBan;
    public float emisiMaks = 25f, kecepatanEmisiHilang = 20f;
    private float cepatEmisiHilang;

    public AudioSource suaraMesin;

    public float waktuLap;

    // Start is called before the first frame update
    void Start()
    {
        //agar model mobil saja yang bergerak. bukan keduanya. karena sphere (Rb) adalah child dari player car
        rigidBody.transform.parent = null;

        if (apakahAI)
        {
            titikTujuan = RaceManager.instance.allCheckPoint[targetSekarang].transform.position;
            RandomizeAITarget();
            kecepatanAiAcak = Random.Range(0.7f, 1.2f);
        }

        UIManager.instance.teksLap.text = lapSekarang + "/" + RaceManager.instance.jumlahLap;
    }

    // Update is called once per frame
    void Update()
    {
        waktuLap += Time.deltaTime;

        var timeSpan = System.TimeSpan.FromSeconds(waktuLap);

        UIManager.instance.teksWaktu.text = string.Format("{0:00}m{1:00}.{2:000}s", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);

        if (!apakahAI)
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

            inputBelokan = Input.GetAxis("Horizontal");
        }
        else
        {
            titikTujuan.y = transform.position.y;

            if (Vector3.Distance(transform.position, titikTujuan) < rentangTitikAI)
            {
                SetNextAITarget();
            }

            Vector3 targetDirection = titikTujuan - transform.position;
            float sudut = Vector3.Angle(targetDirection, transform.forward);
            Vector3 posisiLokal = transform.InverseTransformPoint(titikTujuan);
            if (posisiLokal.x < 0f)
            {
                sudut = -sudut;
            }

            inputBelokan = Mathf.Clamp(sudut / kecepatanMaksBelokAI, -1f, 1f);

            if (Mathf.Abs(sudut) < kecepatanMaksBelokAI)
            {
                inputKecepatanAI = Mathf.MoveTowards(inputKecepatanAI, 1f, kecepatanAI);
            }
            else
            {
                inputKecepatanAI = Mathf.MoveTowards(inputKecepatanAI, kecepatanBelokAI, kecepatanAI);
            }

            inputKecepatan = inputKecepatanAI * akselerasiMaju * kecepatanAiAcak;
        }

        //belok ban
        banDepanKiri.localRotation = Quaternion.Euler(banDepanKiri.localRotation.eulerAngles.x, inputBelokan * kecepatanMaksBelokBan, banDepanKiri.localRotation.eulerAngles.z);
        banDepanKanan.localRotation = Quaternion.Euler(banDepanKanan.localRotation.eulerAngles.x, inputBelokan * kecepatanMaksBelokBan, banDepanKanan.localRotation.eulerAngles.z);

        //transform.position = rigidBody.position;

        //mengontrol debu ban
        cepatEmisiHilang = Mathf.MoveTowards(cepatEmisiHilang, 5, kecepatanEmisiHilang * Time.deltaTime);

        if (Mathf.Abs(inputBelokan) > 0.5f || (rigidBody.velocity.magnitude < kecepatanMaksimal * 0.5f && rigidBody.velocity.magnitude != 0f))
        {
            cepatEmisiHilang = emisiMaks;
        }

        if (rigidBody.velocity.magnitude == 0f)
        {
            cepatEmisiHilang = 0f;
        }

        for (int i = 0; i < debuBan.Length; i++)
        {
            var modulEmisi = debuBan[i].emission;

            modulEmisi.rateOverTime = cepatEmisiHilang;
        }

        if (suaraMesin != null)
        {
            suaraMesin.pitch = 1f + ((rigidBody.velocity.magnitude / kecepatanMaksimal) / 2f);
        }
    }

    private void FixedUpdate()
    {
        //agar konsisten walaupun framer ates beda
        // inputKecepatan dikali 1000 karena drag rigidBody yg besar dan agar acceleration 
        // tidak bernilai besar juga
        rigidBody.AddForce(transform.forward * inputKecepatan * 1000f);

        if (rigidBody.velocity.magnitude > kecepatanMaksimal)
        {
            rigidBody.velocity = rigidBody.velocity.normalized * kecepatanMaksimal;
        }

        //posisi player car = posisi rigid body yang berjalan.
        //diletakan di update karena secara visual Update yang dilihat oleh player secara jelas. 
        transform.position = rigidBody.position;
        
        //quaternion
        if (inputKecepatan != 0)
        {
            /*Mathf.Sign(inputKecepatan) agar ketika belok mundur di reverse. jika math sign bernilai positif akan return angka 1 dan sebaliknya akan return angka -1*/
            /*rigidBody.velocity.magnitude = seberapa cepat rigid body bergerak sekarang. kecepatan tsb dibagi dgn max speed agar cocok dilihat secara visual*/
            /*contoh: rb velo sekarang 10f dan max speed juga 10f, makan mobil berbelok dengan maksimal yaitu(1) sedangkan jika rb velo 5f maka mobil berbelok 1 / 2(0.5)*/

            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, (inputBelokan * kekuatanBelokan * Time.deltaTime * Mathf.Sign(inputKecepatan) * (rigidBody.velocity.magnitude / kecepatanMaksimal)), 0f));
        }
    }

    public void CheckPointHit(int cpNumber)
    {
        if (cpNumber == nextCheckpoint)
        {
            nextCheckpoint++;

            if (nextCheckpoint == RaceManager.instance.allCheckPoint.Length)
            {
                nextCheckpoint = 0;
                LapCompleted();
            }
        }

        if (apakahAI)
        {
            if (cpNumber == targetSekarang)
            {
                SetNextAITarget();
            }
        }
    }

    public void RandomizeAITarget()
    {
        titikTujuan += new Vector3(Random.Range(-varianPointAI, varianPointAI), 0f, Random.Range(-varianPointAI, varianPointAI));
    }

    public void LapCompleted()
    {
        lapSekarang++;

        waktuLap = 0f;

        UIManager.instance.teksLap.text = lapSekarang + "/" + RaceManager.instance.jumlahLap;
    }

    public void SetNextAITarget()
    {
        targetSekarang++;

        if (targetSekarang >= RaceManager.instance.allCheckPoint.Length)
        {
            targetSekarang = 0;
        }

        titikTujuan = RaceManager.instance.allCheckPoint[targetSekarang].transform.position;
        RandomizeAITarget();
    }
}
