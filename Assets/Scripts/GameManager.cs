using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Setting Target")]
    public GameObject targetPrefab;
    public Transform spawnCenter;   // Titik tengah panggung (patokan spawn)
    public float areaLebar = 5f;    // Seberapa lebar target bisa muncul (Kiri-Kanan)

    [Header("Status Game")]
    public int targetCount = 0; // Skor saat ini
    public int targetWin = 5;

    // Start is called before the first frame update
    void Start()
    {
        SpawnTarget();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnTarget()
    {
        // Tentukan posisi acak (Random X)
        // Rumus: Posisi Tengah + (Acak antara -Lebar sampai +Lebar)
        float randomX = Random.Range(-areaLebar, areaLebar);

        // Kita kunci Y dan Z sesuai titik spawnCenter, cuma X yang berubah
        Vector3 randomPos = new Vector3(
            spawnCenter.position.x + randomX,
            spawnCenter.position.y,
            spawnCenter.position.z
        );

        // Munculkan Target Baru
        Instantiate(targetPrefab, randomPos, Quaternion.identity);
    }

    public void TargetEliminated()
    {
        // Fungsi ini dipanggil kalau Target kena tembak
        targetCount++; // Skor nambah 1

        if (targetCount >= targetWin)
        {
            Debug.Log("MENANG! Lanjut Level 2");
            SceneManager.LoadScene("Level2");
        }
        else
        {
            // Kalau belum menang, munculkan target baru lagi
            SpawnTarget();
        }
    }
}
