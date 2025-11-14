using System;
using Camera;
using Smartphone;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Zenject;

public class SurveillanceCameraSwitcher : MonoBehaviour
{
    [SerializeField] private CameraBrain _cameraBrain; 
    [SerializeField] private Volume _volume;
    [SerializeField] private TMP_Text _cameraIDText;
    [SerializeField] private GameObject _cameraUI;
    [SerializeField] private GameObject _mapUI;
    [SerializeField] private SurveillanceCameraData[] _datas;
    
    private void Start()
    {
        Cursor.visible = false;
        
        foreach (var data in _datas)
        {
            data.Button.onClick.AddListener(() =>
            {
                _cameraBrain.SetPriorityId(data.CameraID);
                _volume.profile = data.Profile;
                _cameraIDText.text = data.CameraName;
                if (data.EnabledCameraUI)
                {
                    _mapUI.gameObject.SetActive(false);
                    _cameraUI.gameObject.SetActive(true);
                    Cursor.visible = false;
                }
                else
                {
                    _cameraUI.gameObject.SetActive(false);
                    _mapUI.gameObject.SetActive(true);
                    Cursor.visible = true;
                }
            });
        }
    }
}

[Serializable]
public class SurveillanceCameraData
{
    [SerializeField] private Button _button;
    [SerializeField] private int _cameraID;
    [SerializeField] private string _cameraName;
    [SerializeField] private bool _enabledCameraUI;
    [SerializeField] private VolumeProfile _profile;
    
    public Button Button => _button;
    public int CameraID => _cameraID;
    public string CameraName => _cameraName;
    public bool EnabledCameraUI => _enabledCameraUI;
    public VolumeProfile Profile => _profile;
}
