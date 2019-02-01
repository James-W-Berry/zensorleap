using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorRenderer : MonoBehaviour {

    public SensorManager _manager;
    public GameObject _tempSensor;

    //private void Start()
    //{
    //    PersistFiducialPositionOnClick.OnObjectPlaced += OnObjectPlaced;
    //}

    // Use this for initialization
    public void OnObjectPlaced () {
        _manager.DoWork();
        VehicleSensors _vehicleSensors = _manager.vehicles["taz"];
        MountedSensor _mountedSensor = _vehicleSensors.sensors[0];
        Sensor _sensor = _manager.sensors[_mountedSensor.name];

        Vector3 sensorPos = _mountedSensor.getMountPointAsVector3();

        _tempSensor.transform.localPosition = sensorPos;
	}
}
