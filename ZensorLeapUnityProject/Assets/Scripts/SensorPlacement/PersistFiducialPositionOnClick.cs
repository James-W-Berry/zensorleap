using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class PersistFiducialPositionOnClick : MonoBehaviour {

    public GameObject _FiducialCube;
    public GameObject _PersistedOrigin;
    private MLInputController _controller;
    public GameObject _vehicleModel;

    public delegate void ObjectPlaced();
    public static event ObjectPlaced OnObjectPlaced;

    void Awake()
    {
        MLInput.Start();
        MLInput.OnControllerButtonDown += OnButtonDown;
        _controller = MLInput.GetController(MLInput.Hand.Right);

    }

    void OnButtonDown(byte controller_id, MLInputControllerButton button)
    {
        if (button == MLInputControllerButton.Bumper)
        {
            float newX = _FiducialCube.transform.position.x;
            float newY = _FiducialCube.transform.position.y;
            float newZ = _FiducialCube.transform.position.z;

            _PersistedOrigin.transform.position = new Vector3(newX,newY,newZ);
            _PersistedOrigin.transform.rotation = _FiducialCube.transform.rotation;

            if (OnObjectPlaced != null)
                OnObjectPlaced();
        }
        else if (_controller.TriggerValue > 0.5f)
        {
            //Toggle our gameobject 
            _vehicleModel.SetActive(!_vehicleModel.activeSelf);
        }
    }
}
