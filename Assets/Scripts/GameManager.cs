using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public int targetScoreWin = 100;
    public int startingAmmo = 15;

    [Header("Target Spawn (Panggung)")]
    public GameObject targetPrefab;
    public Transform targetSpawnCenter; // Titik spawn Target (Jauh)
    public float targetAreaWidth = 8f;  // Lebar area spawn Target

    [Header("Obstacle Spawn (Area Tengah)")]
    public GameObject obstaclePrefab;
    public Transform obstacleSpawnCenter;
    public float obstacleWidth = 3f;  // Lebar (Kiri-Kanan / X)
    public float obstacleDepth = 5f; // Panjang (Maju-Mundur / Z) - BARU
    public int minObstacles = 4;
    public int maxObstacles = 8;
    public float obstacleCheckRadius = 1.5f; // Jarak aman antar obstacle

    [Header("UI References (TMP)")]
    public TMP_Text scoreText;
    public TMP_Text ammoText;
    public GameObject gameOverPanel; // Panel Kalah
    public GameObject winPanel;      // Panel Menang

    // Data Internal
    private int currentScore = 0;
    private int currentAmmo;
    private int currentObstacleCount = 0; // Hitung jumlah obstacle aktif

    // Start is called before the first frame update
    void Start()
    {
        currentAmmo = startingAmmo;
        UpdateUI();

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);

        SpawnTarget();

        // Spawn Obstacle Awal
        int jumlahObstacle = Random.Range(minObstacles, maxObstacles + 1);
        for (int i = 0; i < jumlahObstacle; i++)
        {
            SpawnObstacleSafe();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // --- LOGIC SPAWN TARGET (DELAYED) ---
    public void SpawnTarget()
    {
        float randomX = Random.Range(-targetAreaWidth, targetAreaWidth);
        Vector3 spawnPos = new Vector3(
            targetSpawnCenter.position.x + randomX,
            targetSpawnCenter.position.y,
            targetSpawnCenter.position.z
        );

        Instantiate(targetPrefab, spawnPos, Quaternion.identity);
    }

    // --- LOGIC SPAWN OBSTACLE (ANTI-TABRAKAN) ---
    public void SpawnObstacleSafe()
    {
        Vector3 spawnPos = Vector3.zero;
        bool validPositionFound = false;
        int attempts = 0;

        // Kita coba cari posisi kosong maksimal 10 kali
        // Kalau 10 kali gagal (penuh), ya sudah spawn paksa atau skip
        while (!validPositionFound && attempts < 10)
        {
            // Random X (Lebar) dan Z (Panjang/Kedalaman)
            float randomX = Random.Range(-obstacleWidth, obstacleWidth);
            float randomZ = Random.Range(-obstacleDepth, obstacleDepth);
            float randomY = Random.Range(0f, 2f);

            spawnPos = new Vector3(
                obstacleSpawnCenter.position.x + randomX,
                obstacleSpawnCenter.position.y + randomY,
                obstacleSpawnCenter.position.z + randomZ // Tambahkan variasi Z
            );

            // Cek apakah ada benda (Collider) di posisi itu?
            // Physics.CheckSphere mengembalikan TRUE jika ada benda di radius tertentu
            if (!Physics.CheckSphere(spawnPos, obstacleCheckRadius))
            {
                validPositionFound = true; // Kosong! Aman!
            }
            attempts++;
        }

        if (validPositionFound)
        {
            Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);
            currentObstacleCount++;
        }
    }

    // --- LOGIC KENA HIT ---

    public void TargetDestroyed(int points)
    {
        AddScore(points);
        // JANGAN langsung spawn. Mulai hitung mundur dulu.
        StartCoroutine(RespawnTargetRoutine());
    }

    // Ini fungsi penunda waktu (Coroutine)
    IEnumerator RespawnTargetRoutine()
    {
        // Tunggu 2 detik (karena target lama hancur dalam 1.5 detik)
        yield return new WaitForSeconds(2f);

        SpawnTarget(); // Baru munculkan target baru
    }

    public void ObstacleDestroyed(int penalty)
    {
        AddScore(penalty);
        currentObstacleCount--;
        SpawnObstacleSafe(); // Spawn pengganti (Safe Mode)
    }

    public bool CanShoot() => currentAmmo > 0;

    public void UseAmmo()
    {
        currentAmmo--;
        UpdateUI();
        CheckGameStatus();
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        UpdateUI();
        CheckGameStatus();
    }

    void CheckGameStatus()
    {
        // LOGIC MENANG
        if (currentScore >= targetScoreWin)
        {
            Debug.Log("MENANG!");
            // Munculkan Panel Menang
            if (winPanel != null)
            {
                winPanel.SetActive(true);
                Cursor.lockState = CursorLockMode.None; // Munculkan kursor mouse
                Cursor.visible = true;
            }
        }
        // LOGIC KALAH
        else if (currentAmmo <= 0)
        {
            Debug.Log("GAME OVER");
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }

    void UpdateUI()
    {
        if(scoreText) scoreText.text = "Score: " + currentScore + "/" + targetScoreWin;
        if(ammoText) ammoText.text = "Ammo: " + currentAmmo;
    }

    public void RestartLevel() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    public void BackToMainMenu() => SceneManager.LoadScene(0);

    public void LoadNextLevel()
    {
        // Cek urutan scene di Build Settings
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        // Kalau masih ada level berikutnya, lanjut
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            // Kalau sudah level terakhir, balik ke menu
            Debug.Log("Tamat! Balik ke Menu.");
            SceneManager.LoadScene(0);
        }
    }
}
