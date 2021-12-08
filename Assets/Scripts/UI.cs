using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{

    public Camera PlayerCamera;
    public Player PlayerController;
    public GameObject StartMenu;
    public GameObject PauseMenu;
    public SpawnFinder SpawnFinderController;
    public ChunkLoader chunkLoaderController;

    public bool DebugMovement = true;
    public Toggle DebugMovementToggle;
    public Toggle MiddayToggle;
    public float MouseSensitivity = 7f;
    public Slider MouseSensitivitySlider;
    public Text MouseSensitivityValue;
    public float FOV = 90f;
    public Slider FOVSlider;
    public Text FOVValue;
    public Slider TimeScaleSlider;
    public Text TimeScaleValue;

    public TextMeshProUGUI SheepCounter;
    public TextMeshProUGUI AverageFPSCounter;
    public TextMeshProUGUI FPSCounter;
    public TextMeshProUGUI SecondsSinceStart;

    static public bool midday;

    private static UI instance;
    private static int sheepCount = 0;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        MouseSensitivitySlider.value = PlayerPrefs.HasKey("MouseSensitivity") ? PlayerPrefs.GetFloat("MouseSensitivity") : MouseSensitivity;
        SetMouseSensitivity();
        FOVSlider.value = PlayerPrefs.HasKey("FOV") ? PlayerPrefs.GetFloat("FOV") : FOV;
        SetFOV();
        DebugMovementToggle.isOn = PlayerPrefs.HasKey("DebugMovement") ? PlayerPrefs.GetInt("DebugMovement") > 0 : DebugMovement;
        SetDebugMovement();
        DebugMovementToggle.isOn = PlayerPrefs.HasKey("Midday") ? PlayerPrefs.GetInt("Midday") > 0 : DebugMovement;
        SetMidday();
    }

    private void Update()
    {
        AverageFPSCounter.text = ((int)(Time.frameCount / Time.time)).ToString();
        FPSCounter.text = ((int)(1f / Time.unscaledDeltaTime)).ToString();
        SecondsSinceStart.text = ((int)Time.time).ToString();
        if (Input.GetButtonDown("Cancel") && !StartMenu.activeSelf)
            TogglePauseMenu();
    }

    public void TogglePauseMenu()
    {
        Pause.isPaused = !Pause.isPaused;
        PauseMenu.SetActive(Pause.isPaused);
        Cursor.lockState = Pause.isPaused ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void StartGame()
    {
        SpawnFinderController.SpawnPlayer();
        StartMenu.SetActive(false);
    }

    public void Exit()
    {
        Application.Quit();
        //EditorApplication.isPlaying = false;
    }

    public void SetMouseSensitivity()
    {
        MouseSensitivity = MouseSensitivitySlider.value;
        PlayerController.MouseSensitivity = MouseSensitivity;
        MouseSensitivityValue.text = MouseSensitivity.ToString("#.#");
        PlayerPrefs.SetFloat("MouseSensitivity", MouseSensitivity);
    }

    public void SetFOV()
    {
        FOV = FOVSlider.value;
        PlayerCamera.fieldOfView = FOV;
        FOVValue.text = FOV.ToString("#.#");
        PlayerPrefs.SetFloat("FOV", FOV);
    }

    public void SetDebugMovement()
    {
        DebugMovement = DebugMovementToggle.isOn;
        PlayerController.DebugMovement = DebugMovement;
        PlayerPrefs.SetInt("DebugMovement", DebugMovement ? 1 : 0);
    }

    public void SetMidday()
    {
        midday = MiddayToggle.isOn;
    }

    public void SetTimeScale()
    {
        Time.timeScale = TimeScaleSlider.value;
        TimeScaleValue.text = TimeScaleSlider.value.ToString();
    }

    public static void IncreaseSheepCount(int increase)
    {
        sheepCount += increase;
        instance.SheepCounter.text = sheepCount.ToString();
    }

}
