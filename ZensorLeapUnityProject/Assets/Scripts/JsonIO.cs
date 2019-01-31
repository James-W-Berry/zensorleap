using UnityEngine;
using System.IO;

public class JsonIO {

	private string vehiclesPath;
	private string sensorsPath;

	public JsonIO () {
		vehiclesPath = Path.Combine(Application.dataPath, "data/vehicles/");
		sensorsPath = Path.Combine(Application.dataPath, "data/sensors/");
	}

	public Sensor[] readAllSensorsInDirectory() {
		DirectoryInfo directory = new DirectoryInfo(sensorsPath);
		FileInfo[] files = directory.GetFiles("*.json");
		Sensor[] sensors = new Sensor[files.Length];
		int index = 0;
		foreach (FileInfo file in files) {
			sensors[index] = readSensorDataFromFile(file.Name);
			index++;
		}
		return sensors;
	}

	public Sensor readSensorDataFromFile(string fileName) {
		string file = sensorsPath + fileName;
		if (File.Exists(file)) {
			string dataAsJson = File.ReadAllText(file);
			Sensor sensor = JsonUtility.FromJson<Sensor>(dataAsJson);
			Debug.Log(dataAsJson);
			return sensor;
		} else {
			Debug.LogError("file not found: " + file);
			return null;
		}
	}

	public VehicleSensors[] readAllVehiclesInDirectory() {
		DirectoryInfo directory = new DirectoryInfo(vehiclesPath);
		FileInfo[] files = directory.GetFiles("*.json");
		VehicleSensors[] vehicles = new VehicleSensors[files.Length];
		int index = 0;
		foreach (FileInfo file in files) {
			vehicles[index] = readVehicleDataFromFile(file.Name);
			index++;
		}
		return vehicles;
	}
	public VehicleSensors readVehicleDataFromFile(string fileName) {
		string file = vehiclesPath + fileName;
		if (File.Exists(file)) {
			string dataAsJson = File.ReadAllText(file);
			VehicleSensors vehicle = JsonUtility.FromJson<VehicleSensors>(dataAsJson);
			Debug.Log(dataAsJson);
			return vehicle;
		} else {
			Debug.LogError("file not found: " + file);
			return null;
		}
	}
}
