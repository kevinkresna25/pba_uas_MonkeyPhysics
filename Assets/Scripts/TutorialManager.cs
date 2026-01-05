using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text instructionText;
    public GameObject tutorialPanel;

    [Header("Objects")]
    public GameObject dummyTarget;  // Target latihan

    private int currentStep = 0;    // Tahapan tutorial
    private float timer = 0f;       // Timer untuk jeda

    // Start is called before the first frame update
    void Start()
    {
        currentStep = 0;
        ShowStep(0);

        if (dummyTarget != null) dummyTarget.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // LOGIKA SETIAP TAHAP
        switch (currentStep)
        {
            case 0: // WELCOME
                timer += Time.deltaTime;
                if (timer > 5f) NextStep(); // Lanjut setelah 2 detik
                break;

            case 1: // MOUSE LOOK
                // Cek kalau mouse gerak signifikan
                if (Mathf.Abs(Input.GetAxis("Mouse X")) > 0.5f || Mathf.Abs(Input.GetAxis("Mouse Y")) > 0.5f)
                {
                    NextStep();
                }
                break;

            case 2: // MOVEMENT
                // Cek tekan A atau D
                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
                {
                    NextStep();
                }
                break;

            case 3: // SCOPE
                if (Input.GetMouseButtonDown(1)) // Klik Kanan
                {
                    NextStep();
                }
                break;

            case 4: // SPAWN TARGET
                if (dummyTarget != null) dummyTarget.SetActive(true);
                NextStep(); // Langsung lanjut ke instruksi nembak
                break;

            case 5: // SHOOT
                // Cek kalau target sudah hancur (null)
                if (dummyTarget == null)
                {
                    NextStep();
                }
                break;

            case 6: // SELESAI
                timer += Time.deltaTime;
                if (timer > 5f)
                {
                    SceneManager.LoadScene("MainMenu"); // Balik ke menu
                }
                break;
        }
    }

    void NextStep()
    {
        currentStep++;
        timer = 0;
        ShowStep(currentStep);
    }

    void ShowStep(int step)
    {
        switch (step)
        {
            case 0:
                instructionText.text = "WELCOME TO SNIPER TRAINING";
                break;
            case 1:
                instructionText.text = "MOVE THE MOUSE TO LOOK AROUND";
                break;
            case 2:
                instructionText.text = "PRESS 'A' OR 'D' TO MOVE SIDEWAYS";
                break;
            case 3:
                instructionText.text = "RIGHT CLICK TO USE THE SCOPE";
                break;
            case 4:
                instructionText.text = "TARGET ACQUIRED!";
                break;
            case 5:
                instructionText.text = "AIM AND LEFT CLICK TO SHOOT THE TARGET";
                break;
            case 6:
                instructionText.text = "TRAINING COMPLETE. RETURNING TO MENU...";
                break;
        }
    }
}
