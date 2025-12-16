using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBehavior : MonoBehaviour
{
    public int penaltyPoint = -10; // Nilai hukuman (bisa diubah di Inspector tiap prefab beda)
    private bool isHit = false;

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
        // Cek kena Peluru
        if (collision.gameObject.name.Contains("Bullet") && !isHit)
        {
            isHit = true;

            // Lapor ke GameManager
            GameManager gm = FindObjectOfType<GameManager>();
            if (gm != null)
            {
                gm.AddScore(penaltyPoint); // Kurangi poin
            }

            Destroy(collision.gameObject); // Hapus peluru
            Destroy(gameObject); // Hapus rintangan
        }
    }
}
