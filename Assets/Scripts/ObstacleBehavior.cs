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
        if (collision.gameObject.name.Contains("Bullet") && !isHit)
        {
            isHit = true;
            GameManager gm = FindObjectOfType<GameManager>();
            if (gm != null)
            {
                gm.ObstacleDestroyed(penaltyPoint);
            }

            Destroy(collision.gameObject); // Hapus peluru

            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;

            // Langsung hapus Obstacle detik itu juga (tanpa delay)
            Destroy(gameObject);
        }
    }
}
