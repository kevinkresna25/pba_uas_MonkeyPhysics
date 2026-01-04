using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WindManager : MonoBehaviour
{
    [Header("Wind Settings")]
    public float windStrength = 20f;   // Kekuatan angin
    public float changeInterval = 3f;  // Ganti arah tiap 3 detik

    [Header("Visuals")]
    public ParticleSystem snowParticle; // Partikel Salju
    public TMP_Text windIndicatorText;  // UI Teks Arah

    // Variabel Publik yang bisa dibaca Peluru
    [HideInInspector]
    public Vector3 currentWindForce;

    private float timer;
    private bool blowRight = true; // Arah angin saat ini

    // Start is called before the first frame update
    void Start()
    {
        timer = changeInterval;
        UpdateWindDirection(); // Set arah awal
    }

    // Update is called once per frame
    void Update()
    {
        // Hitung mundur waktu
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            // Waktu habis, balik arah!
            blowRight = !blowRight;
            UpdateWindDirection();
            timer = changeInterval; // Reset waktu
        }
    }

    void UpdateWindDirection()
    {
        if (blowRight)
        {
            // Angin ke KANAN (X Positif)
            currentWindForce = new Vector3(windStrength, 0, 0);

            if (windIndicatorText) windIndicatorText.text = "Wind: ->>";
            if (windIndicatorText) windIndicatorText.color = Color.red;

            // Update Rotasi Partikel Salju (Biar miring ke kanan)
            if (snowParticle)
            {
                var shape = snowParticle.shape;
                shape.rotation = new Vector3(0, 90, 0);

                // Atau pakai Force Over Lifetime di Particle System
                var force = snowParticle.forceOverLifetime;
                force.enabled = true;
                force.x = new ParticleSystem.MinMaxCurve(5f); // Tiup ke kanan
            }
        }
        else
        {
            // Angin ke KIRI (X Negatif)
            currentWindForce = new Vector3(-windStrength, 0, 0);

            if (windIndicatorText) windIndicatorText.text = "Wind: <<-";
            if (windIndicatorText) windIndicatorText.color = Color.blue;

            // Update Rotasi Partikel Salju (Biar miring ke kiri)
            if (snowParticle)
            {
                var force = snowParticle.forceOverLifetime;
                force.enabled = true;
                force.x = new ParticleSystem.MinMaxCurve(-5f); // Tiup ke kiri
            }
        }
    }
}
