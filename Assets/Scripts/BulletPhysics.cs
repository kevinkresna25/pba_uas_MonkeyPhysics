using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPhysics : MonoBehaviour
{
    [Header("Settings")]
    public bool isWindy = false;

    private Rigidbody rb;
    private GameManager gm;
    private WindManager windManager; // Referensi ke Angin

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        gm = FindObjectOfType<GameManager>();
        if (gm != null) gm.RegisterBullet();

        // Cari Wind Manager
        windManager = FindObjectOfType<WindManager>();

        // Hancur otomatis setalah 3 detik (kalau meleset ke angkasa)
        Destroy(gameObject, 3f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (isWindy && windManager != null)
        {
            // Ambil arah angin TERBARU dari Manager
            rb.AddForce(windManager.currentWindForce, ForceMode.Force);
        }
    }

    // Fungsi bawaan Unity yang dipanggil SAAT OBJEK HANCUR
    // Baik hancur karena nabrak, atau hancur karena waktu habis
    void OnDestroy()
    {
        // Lapor ke GameManager "Peluru Sudah Hilang"
        if (gm != null) gm.UnregisterBullet();
    }
}
