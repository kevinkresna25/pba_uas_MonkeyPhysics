using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPhysics : MonoBehaviour
{
    [Header("Wind Settings (Level 2)")]
    public bool isWindy = false; // Centang di Level 2
    public Vector3 windForce = new Vector3(20f, 0, 0); // Angin dorong ke kanan
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Hancurkan peluru ini setelah 3 detik
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

    // Deteksi Tabrakan
    void OnCollisionEnter(Collision collision)
    {
        // Jika nabrak sesuatu, hancurkan peluru langsung
        Destroy(gameObject);
    }
}
