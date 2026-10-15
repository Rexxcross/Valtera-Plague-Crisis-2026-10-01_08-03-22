using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerSweep : MonoBehaviour
{
    [Header("Progress UI")]
    [SerializeField] private Slider progressBar;

    private SweepableArea currentArea;
    private SweepableArea[] allAreas;

    private int totalAreas;

    private void Start()
    {
        // Ambil semua area kotor yang aktif di scene.
        allAreas = FindObjectsByType<SweepableArea>(
            FindObjectsSortMode.None
        );

        totalAreas = allAreas.Length;

        // Progress bar menggunakan nilai 0 sampai 1.
        if (progressBar != null)
        {
            progressBar.minValue = 0f;
            progressBar.maxValue = 1f;
            progressBar.value = 0f;
        }

        if (totalAreas == 0)
        {
            Debug.LogWarning(
                "Tidak ditemukan objek dengan script SweepableArea."
            );
        }
    }

    private void Update()
    {
        // Pemain harus berada di area yang bisa dibersihkan.
        if (currentArea != null &&
            !currentArea.IsCleaned &&
            Keyboard.current != null &&
            Keyboard.current.eKey.isPressed)
        {
            bool justCleaned = currentArea.Sweep(Time.deltaTime);

            if (justCleaned)
            {
                Debug.Log(
                    currentArea.name + " berhasil dibersihkan!"
                );
            }
        }

        UpdateProgress();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        SweepableArea area =
            other.GetComponentInParent<SweepableArea>();

        if (area != null && !area.IsCleaned)
        {
            currentArea = area;

            Debug.Log("Tahan E untuk menyapu area ini.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        SweepableArea area =
            other.GetComponentInParent<SweepableArea>();

        if (area != null && currentArea == area)
        {
            currentArea = null;
        }
    }

    private void UpdateProgress()
    {
        if (totalAreas == 0)
            return;

        float totalProgress = 0f;

        foreach (SweepableArea area in allAreas)
        {
            totalProgress += area.Progress01;
        }

        float progress = totalProgress / totalAreas;

        if (progressBar != null)
        {
            progressBar.value = progress;
        }
    }
}