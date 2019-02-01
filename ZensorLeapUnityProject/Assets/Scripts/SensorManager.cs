using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorManager : MonoBehaviour {

	private JsonIO jsonIO;
	public Dictionary<string, VehicleSensors> vehicles;
	public Dictionary<string, Sensor> sensors;

    private void Start()
    {
        //DoWork();
    }

    public void DoWork () {
		jsonIO = new JsonIO();

		sensors = new Dictionary<string, Sensor>();
		Sensor[] readSensors = jsonIO.readAllSensorsInDirectory();
		foreach(Sensor sensor in readSensors) {
			sensors.Add(sensor.name, sensor);
		}

		vehicles = new Dictionary<string, VehicleSensors>();
		VehicleSensors[] readVehicles = jsonIO.readAllVehiclesInDirectory();
		foreach(VehicleSensors vehicle in readVehicles) {
			vehicles.Add(vehicle.name, vehicle);
		}

		Debug.Log("all vehicles and sensors loaded");
	}
	
	void Update () {
		
	}
}
