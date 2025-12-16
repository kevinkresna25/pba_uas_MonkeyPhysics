using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetBehavior : MonoBehaviour
{
    private bool isHit = false; // Biar nggak kena hit berkali-kali dalam 1 detik

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        // Cek apakah yang nabrak adalah Peluru?
        // Kita cek nama objectnya mengandung kata "Bullet" atau punya script BulletPhysics
        if (collision.gameObject.name.Contains("Bullet") && !isHit)
        {
            isHit = true; // Tandai sudah kena

            // 1. Panggil GameManager untuk Lapor "Saya Mati"
            // FindObjectOfType mencari objek yang punya script GameManager
            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.TargetEliminated();
            }

            // 2. Hancurkan Peluru (Biar nggak nembus)
            Destroy(collision.gameObject);

            // 3. Hancurkan Diri Sendiri (Target)
            // Kasih delay 1-2 detik biar pemain sempat lihat efek targetnya MENTAL dulu (Momentum)
            Destroy(gameObject, 1.5f);
        }
    }
}
