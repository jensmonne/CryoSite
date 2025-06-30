using BNG;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum TurnType { Snap, Smooth }

public class VRSettingsManager : MonoBehaviour
{
    public static VRSettingsManager Instance;

    private TurnType _turnSetting = TurnType.Snap;

    public TurnType TurnSetting
    {
        get => _turnSetting;
        set
        {
            if (_turnSetting == value) return;
            _turnSetting = value;
            ApplyLocomotionSettings();
        }
    }

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        
        DontDestroyOnLoad(this);
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyLocomotionSettings();
    }

    private void ApplyLocomotionSettings()
    {
        PlayerRotation rotation = FindObjectOfType<PlayerRotation>();

        if (TurnSetting == TurnType.Snap)
        {
            rotation.RotationType = RotationMechanic.Snap;
        }
        else
        {
            rotation.RotationType = RotationMechanic.Smooth;
        }
    }
}
