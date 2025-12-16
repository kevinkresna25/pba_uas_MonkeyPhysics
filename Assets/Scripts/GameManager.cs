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

    [Header("Target Spawn")]
    public GameObject targetPrefab;
    public Transform targetSpawnCenter; // Titik spawn Target (Jauh)
    public float targetAreaWidth = 8f;  // Lebar area spawn Target

    [Header("Obstacle Spawn")]
    public GameObject obstaclePrefab;
    public Transform obstacleSpawnCenter;
    public float obstacleWidth = 3f;  // Lebar (Kiri-Kanan / X)
    public float obstacleDepth = 5f; // Panjang (Maju-Mundur / Z) - BARU
    public int minObstacles = 4;
    public int maxObstacles = 8;
    public float obstacleCheckRadius = 1.5f; // Jarak aman antar obstacle

    [Header("UI References")]
    public TMP_Text scoreText;
    public TMP_Text ammoText;
    public GameObject gameOverPanel; // Panel Kalah
    public GameObject winPanel;      // Panel Menang

    [HideInInspector]
    public int activeBullets = 0; // Menghitung peluru yang masih terbang
    private bool isGameEnded = false; // Biar panel gak muncul dobel

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

    // --- LOGIC: REGISTER PELURU ---
    public void RegisterBullet()
    {
        activeBullets++; // Ada peluru baru terbang
    }

    public void UnregisterBullet()
    {
        activeBullets--; // Peluru sudah hancur/hilang
        // Cek status game SETELAH peluru hilang
        CheckGameStatus();
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
        if (isGameEnded) return; // Kalau udah game over/win, abaikan skor susulan
        AddScore(points);

        // JANGAN langsung spawn. Mulai hitung mundur dulu.
        StartCoroutine(RespawnTargetRoutine());
    }

    // Ini fungsi penunda waktu (Coroutine)
    IEnumerator RespawnTargetRoutine()
    {
        yield return new WaitForSeconds(2f);
        if (!isGameEnded) SpawnTarget();
    }

    public void ObstacleDestroyed(int penalty)
    {
        if (isGameEnded) return;
        AddScore(penalty);
        currentObstacleCount--;
        if (!isGameEnded) SpawnObstacleSafe();
    }

    public bool CanShoot() => currentAmmo > 0 && !isGameEnded;

    public void UseAmmo()
    {
        currentAmmo--;
        UpdateUI();
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        UpdateUI();
        CheckGameStatus();
    }

    void CheckGameStatus()
    {
        if (isGameEnded) return; // Jangan cek lagi kalau sudah selesai

        // 1. Cek MENANG (Prioritas Utama)
        if (currentScore >= targetScoreWin)
        {
            WinGame();
        }
        // 2. Cek KALAH
        // Kalah cuma kalau: Ammo Habis DAN Tidak ada peluru di udara
        else if (currentAmmo <= 0 && activeBullets <= 0)
        {
            LoseGame();
        }
    }

    void WinGame()
    {
        isGameEnded = true;
        Debug.Log("MENANG!");
        if (winPanel != null)
        {
            winPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void LoseGame()
    {
        isGameEnded = true;
        Debug.Log("GAME OVER");
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
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
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings) SceneManager.LoadScene(nextSceneIndex);
        else SceneManager.LoadScene(0);
    }
}
