using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ContaminationGaugeManager : MonoBehaviour
{
    [Header("UI Gauge Global")]
    [SerializeField] private Slider globalGaugeSlider;
    [SerializeField] private TMP_Text globalGaugeText;

    [Header("UI Gauge Lokal")]
    [SerializeField] private Slider localGaugeSlider;
    [SerializeField] private TMP_Text localGaugeText;

    [Header("Nilai Awal Gauge (0-100)")]
    [SerializeField, Range(0f, 100f)]
    private float globalContamination = 0f;

    [SerializeField, Range(0f, 100f)]
    private float localContamination = 0f;

    [Header("Kenaikan Normal per Detik")]
    [SerializeField, Min(0f)]
    private float globalRisePerSecond = 0.4f;

    [SerializeField, Min(0f)]
    private float localRisePerSecond = 0.8f;

    [Header("Deteksi Pemain Diam")]
    [SerializeField] private Transform player;
    [SerializeField, Min(0f)]
    private float idleGracePeriod = 8f;

    [SerializeField, Min(0f)]
    private float movementThreshold = 0.01f;

    [SerializeField, Min(0f)]
    private float idleGlobalBonusPerSecond = 1f;

    [SerializeField, Min(0f)]
    private float idleLocalBonusPerSecond = 2f;

    [Header("Penalti Gagal Menyapu")]
    [SerializeField, Min(0f)]
    private float failedGlobalPenalty = 4f;

    [SerializeField, Min(0f)]
    private float failedLocalPenalty = 10f;

    [SerializeField, Min(0f)]
    private float failureBoostDuration = 5f;

    [SerializeField, Min(0f)]
    private float failureGlobalBonusPerSecond = 1.5f;

    [SerializeField, Min(0f)]
    private float failureLocalBonusPerSecond = 3f;

    [Header("Pengurangan Saat Berhasil")]
    [SerializeField, Min(0f)]
    private float globalDecreasePerClean = 2f;

    [SerializeField, Min(0f)]
    private float localDecreasePerClean = 12f;

    private Vector3 lastPlayerPosition;
    private float idleTimer;
    private float failureBoostTimer;

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
                player = playerObject.transform;
        }

        if (player != null)
            lastPlayerPosition = player.position;

        SetupSlider(globalGaugeSlider);
        SetupSlider(localGaugeSlider);

        UpdateGaugeUI();
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        UpdateIdleTimer(deltaTime);

        bool isIdle = idleTimer >= idleGracePeriod;
        bool failureBoostActive = failureBoostTimer > 0f;

        float globalRate = globalRisePerSecond;
        float localRate = localRisePerSecond;

        // Tambahan kenaikan jika pemain terlalu lama diam.
        if (isIdle)
        {
            globalRate += idleGlobalBonusPerSecond;
            localRate += idleLocalBonusPerSecond;
        }

        // Tambahan kenaikan setelah kegagalan.
        if (failureBoostActive)
        {
            globalRate += failureGlobalBonusPerSecond;
            localRate += failureLocalBonusPerSecond;

            failureBoostTimer = Mathf.Max(
                0f,
                failureBoostTimer - deltaTime
            );
        }

        globalContamination = Mathf.Clamp(
            globalContamination + globalRate * deltaTime,
            0f,
            100f
        );

        localContamination = Mathf.Clamp(
            localContamination + localRate * deltaTime,
            0f,
            100f
        );

        UpdateGaugeUI();
    }

    private void UpdateIdleTimer(float deltaTime)
    {
        if (player == null)
            return;

        float distanceSquared =
            (player.position - lastPlayerPosition).sqrMagnitude;

        if (distanceSquared >=
            movementThreshold * movementThreshold)
        {
            // Pemain bergerak.
            idleTimer = 0f;
            lastPlayerPosition = player.position;
        }
        else
        {
            idleTimer += deltaTime;
        }
    }

    public void RegisterActivity()
    {
        // Dipanggil saat pemain sedang menyapu.
        idleTimer = 0f;

        if (player != null)
            lastPlayerPosition = player.position;
    }

    public void RegisterFailedSweep()
    {
        // Penalti langsung.
        globalContamination = Mathf.Clamp(
            globalContamination + failedGlobalPenalty,
            0f,
            100f
        );

        localContamination = Mathf.Clamp(
            localContamination + failedLocalPenalty,
            0f,
            100f
        );

        // Gauge naik lebih cepat selama beberapa detik.
        failureBoostTimer = failureBoostDuration;

        UpdateGaugeUI();

        Debug.Log("Gagal menyapu! Kontaminasi meningkat.");
    }

    public void RegisterCleaningSuccess()
    {
        // Kurangi kedua gauge setiap berhasil membersihkan debu.
        globalContamination = Mathf.Clamp(
            globalContamination - globalDecreasePerClean,
            0f,
            100f
        );

        localContamination = Mathf.Clamp(
            localContamination - localDecreasePerClean,
            0f,
            100f
        );

        RegisterActivity();
        UpdateGaugeUI();

        Debug.Log("Kontaminasi berhasil dikurangi!");
    }

    private void SetupSlider(Slider slider)
    {
        if (slider == null)
            return;

        slider.minValue = 0f;
        slider.maxValue = 100f;
        slider.value = 0f;
    }

    private void UpdateGaugeUI()
    {
        if (globalGaugeSlider != null)
            globalGaugeSlider.value = globalContamination;

        if (localGaugeSlider != null)
            localGaugeSlider.value = localContamination;

        if (globalGaugeText != null)
        {
            globalGaugeText.text =
                $"Global: {Mathf.RoundToInt(globalContamination)}%";
        }

        if (localGaugeText != null)
        {
            localGaugeText.text =
                $"Lokal: {Mathf.RoundToInt(localContamination)}%";
        }
    }
}