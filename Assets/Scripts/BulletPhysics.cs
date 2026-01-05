using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPhysics : MonoBehaviour
{
    [Header("Settings")]
    public bool isWindy = false;
    public Vector3 windForce = new Vector3(20f, 0, 0);

    [Header("VFX")]
    public GameObject hitEffectPrefab;

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

    // LOGIC TABRAKAN & EFEK
    void OnCollisionEnter(Collision collision)
    {
        // 1. Munculkan Efek Debu/Ledakan (BARU)
        if (hitEffectPrefab != null)
        {
            // ContactPoint adalah titik pas peluru nyentuh tembok
            ContactPoint contact = collision.contacts[0];
            Quaternion rot = Quaternion.LookRotation(contact.normal);

            GameObject vfx = Instantiate(hitEffectPrefab, contact.point, rot);
            Destroy(vfx, 1f); // Hapus sisa efek setelah 1 detik
        }

        // 2. Hancurkan Peluru
        Destroy(gameObject);
    }

    // Fungsi bawaan Unity yang dipanggil SAAT OBJEK HANCUR
    // Baik hancur karena nabrak, atau hancur karena waktu habis
    void OnDestroy()
    {
        // Lapor ke GameManager "Peluru Sudah Hilang"
        if (gm != null) gm.UnregisterBullet();
    }
}
