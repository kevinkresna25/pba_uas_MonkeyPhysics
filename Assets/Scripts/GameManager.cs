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

    [Header("Spawn Settings")]
    public GameObject targetPrefab;
    public GameObject obstaclePrefab;
    public Transform spawnCenter;   // Titik tengah panggung (patokan spawn)
    public float areaLebar = 5f;    // Seberapa lebar target bisa muncul (Kiri-Kanan)

    [Header("UI References (TMP)")]
    public TMP_Text scoreText;
    public TMP_Text ammoText;
    public GameObject gameOverPanel; // Panel Kalah
    public GameObject winPanel;      // Panel Menang

    // Data Internal
    private int currentScore = 0;
    private int currentAmmo;

    // Start is called before the first frame update
    void Start()
    {
        currentAmmo = startingAmmo;
        UpdateUI();
        SpawnRandomObject();

        // Sembunyikan panel di awal
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnRandomObject()
    {
        float randomX = Random.Range(-areaLebar, areaLebar);
        Vector3 spawnPos = new Vector3(spawnCenter.position.x + randomX, spawnCenter.position.y, spawnCenter.position.z);

        int dice = Random.Range(0, 100); 
        if (dice < 70) Instantiate(targetPrefab, spawnPos, Quaternion.identity);
        else Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);
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
        SpawnRandomObject();
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

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

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
