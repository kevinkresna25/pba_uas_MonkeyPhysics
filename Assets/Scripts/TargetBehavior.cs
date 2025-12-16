using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetBehavior : MonoBehaviour
{
    public int rewardPoint = 20; // Poin hadiah
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
        if (collision.gameObject.name.Contains("Bullet") && !isHit)
        {
            isHit = true;
            GameManager gm = FindObjectOfType<GameManager>();
            if (gm != null)
            {
                // Panggil fungsi khusus Target di GM
                gm.TargetDestroyed(rewardPoint);
            }

            Destroy(collision.gameObject);
            Destroy(gameObject, 1.5f); // Efek mental dulu baru hancur
        }
    }
}
