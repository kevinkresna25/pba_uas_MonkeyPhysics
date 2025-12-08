using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 50f; // Kecepatan gerak (Gaya Dorong)
    public Rigidbody rb;

    [Header("Look Settings")]
    public float mouseSensitivity = 100f; // Kecepatan putar mouse
    public Transform playerBody;          // Badan Player untuk diputar kiri-kanan
    float xRotation = 0f;

    [Header("Shooting Settings")]
    public GameObject bulletPrefab; // Peluru (Prefab)
    public Transform firePoint;     // Titik keluar peluru
    public float shootForce = 50f;  // Kekuatan tembakan

    // Start is called before the first frame update
    void Start()
    {
        // Mengunci kursor mouse di tengah layar biar enak mainnya
        Cursor.lockState = CursorLockMode.Locked;

        if (rb == null) rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // --- 1. MOUSE LOOK (Nengok) ---
        // Mengambil input gerakan mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Hitungan matematika untuk nengok atas-bawah (biar gak bablas muter kepala)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Memutar Kamera (Atas-Bawah)
        Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Memutar Badan Player (Kiri-Kanan)
        playerBody.Rotate(Vector3.up * mouseX);

        // --- 2. SHOOTING (Menembak) ---
        // Jika Klik Kiri (Button 0) ditekan
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void FixedUpdate()
    {
        // --- 3. MOVEMENT (Gerak Fisika) ---
        // Input Horizontal: Tombol A (-1) dan D (1)
        float x = Input.GetAxis("Horizontal");

        // Menentukan arah gerak (Samping Kanan/Kiri relatif terhadap arah hadap player)
        Vector3 moveDirection = transform.right * x;

        // PENTING: Menggunakan AddForce (Fisika Murni)
        // ForceMode.Force = Mendorong terus menerus seperti mesin mobil
        rb.AddForce(moveDirection * moveSpeed, ForceMode.Force);
    }

    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            // 1. Munculkan Peluru dari FirePoint
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            // 2. Dorong Peluru dengan Fisika (Impulse)
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                // ForceMode.Impulse = Gaya ledak instan (cocok untuk peluru/lompat)
                bulletRb.AddForce(firePoint.forward * shootForce, ForceMode.Impulse);
            }
        }
    }
}
