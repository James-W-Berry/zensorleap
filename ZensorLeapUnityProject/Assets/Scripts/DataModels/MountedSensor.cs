using UnityEngine;

[System.Serializable]
public class MountedSensor {

	public string name;
	public double[] mountPoint;
	public double[] orientation;

	public Quaternion getOrientationAsQuaterian() {
		Quaternion roll = Quaternion.Euler(new Vector3((float)orientation[2], 0f, 0f));
		Quaternion pitch = Quaternion.Euler(new Vector3(0f, -(float)orientation[1], 0f));
		Quaternion yaw = Quaternion.Euler(new Vector3(0f, 0f, -(float)orientation[0]));
		return roll * pitch * yaw;
	}

	public Vector3 getMountPointAsVector3() {
		return new Vector3((float)mountPoint[0], (float)mountPoint[1], (float)mountPoint[2]);
	}

}
