using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class PersistFiducialPositionOnClick : MonoBehaviour {

    public GameObject _FiducialCube;
    public GameObject _PersistedOrigin;

    void Awake()
    {
        MLInput.Start();
        MLInput.OnControllerButtonDown += OnButtonDown;
    }

    void OnButtonDown(byte controller_id, MLInputControllerButton button)
    {
        _PersistedOrigin.transform.position = _FiducialCube.transform.position;
        _PersistedOrigin.transform.rotation = _FiducialCube.transform.rotation;

    }
}
