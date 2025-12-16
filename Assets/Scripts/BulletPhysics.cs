using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPhysics : MonoBehaviour
{
    [Header("Wind Settings (Level 2)")]
    public bool isWindy = false; // Centang di Level 2
    public Vector3 windForce = new Vector3(20f, 0, 0); // Angin dorong ke kanan
    private Rigidbody rb;
    private GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Cari GameManager dan Lapor "Ada Peluru Baru!"
        gm = FindObjectOfType<GameManager>();
        if (gm != null)
        {
            gm.RegisterBullet();
        }

        // Hancur otomatis setalah 3 detik (kalau meleset ke angkasa)
        Destroy(gameObject, 3f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        // Jika mode Angin aktif (Level 2)
        if (isWindy)
        {
            // Tambahkan gaya angin terus menerus ke peluru
            rb.AddForce(windForce, ForceMode.Force);
        }
    }

    // Fungsi bawaan Unity yang dipanggil SAAT OBJEK HANCUR
    // Baik hancur karena nabrak, atau hancur karena waktu habis
    void OnDestroy()
    {
        // Lapor ke GameManager "Peluru Sudah Hilang"
        if (gm != null)
        {
            gm.UnregisterBullet();
        }
    }
}
