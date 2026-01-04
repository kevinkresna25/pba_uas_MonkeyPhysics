using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 50f; // Kecepatan gerak (Gaya Dorong)
    public Rigidbody rb;

    [Header("Shooting")]
    public GameObject bulletPrefab;     // Peluru (Prefab)
    public Transform firePoint;         // Titik keluar peluru
    public float shootForce = 50f;      // Kekuatan tembakan
    public float timeBetweenShots = 2f; // Jeda 2 detik tiap tembakan
    private float nextTimeToFire = 0f;  // Kapan boleh nembak lagi?

    [Header("Recoil")]
    // Camera Recoil
    public float verticalRecoil = 5f;    // Seberapa tinggi kamera mental
    public float recoilSnappiness = 10f; // Seberapa CEPAT kamera naik (Hentakan)
    public float recoilReturnSpeed = 2f; // Seberapa LAMBAT kamera turun (Balik ke semula)

    // Weapon Recoil
    public float gunKickBack = 0.2f;
    public float weaponReturnSpeed = 5f;
    public float weaponSnappiness = 20f;

    [Header("Aiming & Scope")]
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    public Camera mainCamera;       // Referensi Kamera untuk di-Zoom
    public GameObject weaponModel;  // Referensi Senjata untuk disembunyikan
    public GameObject scopeOverlay; // Referensi Gambar Scope di UI

    public float defaultFOV = 60f;  // Jarak pandang normal
    public float scopedFOV = 15f;   // Jarak pandang saat zoom (makin kecil makin dekat)

    private float xRotation = 0f;
    private bool isScoped = false;  // Status apakah sedang ngeker atau tidak

    // Internal Variables untuk Smooth Recoil
    private float currentRecoilXPos; // Posisi recoil saat ini
    private float targetRecoilXPos;  // Target recoil (puncak hentakan)

    // Internal Variables untuk Weapon Recoil
    private Vector3 originalWeaponPos;
    private Vector3 currentWeaponPos;
    private Vector3 targetWeaponPos;

    // Referensi ke GameManager
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        // Mengunci kursor mouse di tengah layar biar enak mainnya
        Cursor.lockState = CursorLockMode.Locked;
        if (rb == null) rb = GetComponent<Rigidbody>();

        // Cari GameManager otomatis
        gameManager = FindObjectOfType<GameManager>();

        // scope mati
        if (scopeOverlay != null) scopeOverlay.SetActive(false);

        // Pastikan Senjata Muncul & Kamera Normal
        if (weaponModel != null)
        {
            weaponModel.SetActive(true);
            // Simpan posisi awal senjata untuk dikembalikan setelah recoil
            originalWeaponPos = weaponModel.transform.localPosition;
        }
        if (mainCamera != null) mainCamera.fieldOfView = defaultFOV;
    }

    // Update is called once per frame
    void Update()
    {
        // AIMING (Mouse Look)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // LOGIC SMOOTH CAMERA RECOIL
        // Recoil pelan-pelan turun ke 0 (Recovery)
        targetRecoilXPos = Mathf.Lerp(targetRecoilXPos, 0, Time.deltaTime * recoilReturnSpeed);

        // Posisi Recoil saat ini mengejar Target Recoil (Hentakan)
        currentRecoilXPos = Mathf.Lerp(currentRecoilXPos, targetRecoilXPos, Time.deltaTime * recoilSnappiness);

        // Terapkan Rotasi Kamera: (Mouse Input + Recoil Input)
        // Recoil cuma nambahin offset sementara, tidak mengubah xRotation asli
        Camera.main.transform.localRotation = Quaternion.Euler(xRotation - currentRecoilXPos, 0f, 0f);

        // Putar Badan Player (Kiri-Kanan)
        playerBody.Rotate(Vector3.up * mouseX);

        // SCOPE INPUT (Klik Kanan) 
        if (Input.GetMouseButtonDown(1)) // 1 = Klik Kanan
        {
            isScoped = !isScoped; // Switch status (True jadi False, False jadi True)

            if (isScoped) StartScope();
            else EndScope();
        }

        // LOGIC SHOOTING (DENGAN JEDA)
        if (Input.GetMouseButtonDown(0))
        {
            // Syarat Nembak:
            // 1. Peluru di GameManager ada
            // 2. Waktu sekarang (Time.time) sudah melewati Waktu Jeda (nextTimeToFire)
            if (gameManager != null && gameManager.CanShoot() && Time.time >= nextTimeToFire)
            {
                // Set waktu tembak berikutnya = Waktu sekarang + Jeda
                nextTimeToFire = Time.time + timeBetweenShots;

                Shoot();
                gameManager.UseAmmo();
            }
            else if (Time.time < nextTimeToFire)
            {
                Debug.Log("Sedang Kokang...");
            }
        }

        // WEAPON RECOIL ANIMATION
        if (weaponModel != null && !isScoped)
        {
            targetWeaponPos = Vector3.Lerp(targetWeaponPos, Vector3.zero, Time.deltaTime * weaponReturnSpeed);
            currentWeaponPos = Vector3.Lerp(currentWeaponPos, targetWeaponPos, Time.deltaTime * weaponSnappiness);
            weaponModel.transform.localPosition = originalWeaponPos + currentWeaponPos;
        }
    }

    void FixedUpdate()
    {
        // MOVEMENT (Gerak Fisika) 
        // Input Horizontal: Tombol A (-1) dan D (1)
        float x = Input.GetAxis("Horizontal");

        // Menentukan arah gerak (Samping Kanan/Kiri relatif terhadap arah hadap player)
        Vector3 moveDir = transform.right * x;

        // PENTING: Menggunakan AddForce (Fisika Murni)
        // ForceMode.Force = Mendorong terus menerus seperti mesin mobil
        rb.AddForce(moveDir * moveSpeed, ForceMode.Force);
    }

    void StartScope()
    {
        if (scopeOverlay) scopeOverlay.SetActive(true); // Munculkan Overlay Hitam
        if (weaponModel) weaponModel.SetActive(false);  // Sembunyikan Senjata (biar gak ngalangin)
        if (mainCamera) mainCamera.fieldOfView = scopedFOV; // Zoom Kamera

        // Kurangi sensitivitas mouse pas lagi ngeker biar stabil
        mouseSensitivity = 20f;
    }

    void EndScope()
    {
        if (scopeOverlay) scopeOverlay.SetActive(false); // Sembunyikan Overlay
        if (weaponModel) weaponModel.SetActive(true);    // Munculkan Senjata Lagi
        if (mainCamera) mainCamera.fieldOfView = defaultFOV; // Reset Kamera

        // Kembalikan sensitivitas mouse normal
        mouseSensitivity = 100f;
    }

    void Shoot()
    {
        // APPLY RECOIL
        // Kita tambah target recoil, nanti di Update() dia bakal di-smooth-kan
        targetRecoilXPos += verticalRecoil;

        // Weapon Kickback
        targetWeaponPos.z -= gunKickBack;

        if (bulletPrefab != null && firePoint != null && mainCamera != null)
        {
            Vector3 spawnPosition;
            Quaternion spawnRotation;
            Vector3 forceDirection;

            // --- LOGIKA HYBRID ---
            if (isScoped)
            {
                // MODE SCOPE: Peluru keluar dari "Mata" (Kamera)
                // Ini membuat peluru keluar PERSIS di titik tengah crosshair

                // Spawn sedikit di depan kamera (0.5 meter) biar gak nabrak muka sendiri
                spawnPosition = mainCamera.transform.position + (mainCamera.transform.forward * 0.5f);

                // Arahnya lurus mengikuti arah pandang kamera
                spawnRotation = mainCamera.transform.rotation;
                forceDirection = mainCamera.transform.forward;
            }
            else
            {
                // MODE NORMAL: Peluru keluar dari Ujung Senjata
                // Kita pakai teknik Raycast biar tetap akurat ke tengah walaupun dari samping

                Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                RaycastHit hit;
                Vector3 targetPoint;

                // Cek tabrakan di tengah layar
                if (Physics.Raycast(ray, out hit))
                    targetPoint = hit.point;
                else
                    targetPoint = ray.GetPoint(1000); // Tembak ke langit

                spawnPosition = firePoint.position;

                // Hitung arah dari senjata ke titik tengah layar
                Vector3 direction = targetPoint - firePoint.position;
                spawnRotation = Quaternion.LookRotation(direction);
                forceDirection = direction.normalized;
            }

            // --- EKSEKUSI TEMBAK ---
            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, spawnRotation);

            // BUG TERPENTAL: IGNORE COLLISION
            Collider bulletCollider = bullet.GetComponent<Collider>();
            Collider playerCollider = GetComponent<Collider>();

            if (bulletCollider != null && playerCollider != null)
            {
                // Peluru ini jangan nabrak Player
                Physics.IgnoreCollision(bulletCollider, playerCollider);
            }
            
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

            if (bulletRb != null)
            {
                // Reset kecepatan peluru biar stabil
                bulletRb.velocity = Vector3.zero;
                bulletRb.angularVelocity = Vector3.zero;

                // Dorong!
                bulletRb.AddForce(forceDirection * shootForce, ForceMode.Impulse);
            }
        }
    }
}
