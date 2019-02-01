using UnityEngine;

[System.Serializable]
public class MountedSensor {

	public string name;
	public double[] mountPoint;
	public double[] orientation;

	public Quaternion getOrientationAsQuaterian() {
		return new Quaternion((float)orientation[0], (float)orientation[1], (float)orientation[2], (float)orientation[3]);
	}

	public Vector3 getMountPointAsVector3() {
		return new Vector3((float)mountPoint[0], (float)mountPoint[1], (float)mountPoint[2]);
	}

}
