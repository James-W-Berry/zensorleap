import pytest
import json
import numpy as np
from numpy import r_, c_, pi

from sensorDescriptionLibrary import SensorType, angleAxisToQuat

class TestFixture_Sensor:

    def setup_method(self):
        self.testValues = {
            "name":"test_sensor_type",
            "azimuthMin":-15.0,
            "azimuthMax":15.0,
            "elevationMin":-10.0,
            "elevationMax":10.0,
            "rangeMin":2.0,
            "rangeMax":70.0,
        }



    def _populatedSensorTypeJsonTemplate(self, **kwargs):
        testJson = f"""{{
    "name": "{kwargs['name']}",
    "azimuthMin": {kwargs['azimuthMin']},
    "azimuthMax": {kwargs['azimuthMax']},
    "elevationMin": {kwargs['elevationMin']},
    "elevationMax": {kwargs['elevationMax']},
    "rangeMin": {kwargs['rangeMin']},
    "rangeMax": {kwargs['rangeMax']}
    }}
        """
        return testJson

    def test_createWithDefaultValues(self):
        sensorType = SensorType(jsonData={})
        sensorTypeDefault = SensorType(jsonData=SensorType.defaults)
        sensorTypeDefaultAlso = SensorType()

        assert(sensorType==sensorTypeDefault)
        assert(sensorTypeDefault == sensorTypeDefaultAlso)

    def test_loadSensorFromJson(self):
        testJson = self._populatedSensorTypeJsonTemplate(**self.testValues)
        parsedJson = json.loads(testJson)

        sensorType = SensorType(jsonData=parsedJson)

        self._assertExtentsMatch(sensorType, self.testValues)

    def test_storeSensorToJson(self):
        trueJsonText = self._populatedSensorTypeJsonTemplate(**self.testValues)
        trueJson = json.loads(trueJsonText)

        sensorType = SensorType(jsonData=trueJson)
        sensorTypeAsJson = sensorType.toJson()

        self._assertJsonContentsMatch(trueJson, sensorTypeAsJson)

    def _assertJsonContentsMatch(self, parsedJson, sensorTypeAsJson):
        reparsedJson = json.loads(sensorTypeAsJson)
        assert (parsedJson == reparsedJson)

    def _assertExtentsMatch(self, sensorType, trueValues):
        assert (sensorType.getAzimuthExtent() == [trueValues['azimuthMin'],trueValues['azimuthMax']])
        assert (sensorType.getElevationExtent() == [trueValues['elevationMin'],trueValues['elevationMax']])
        assert (sensorType.getRangeExtent() == [trueValues['rangeMin'],trueValues['rangeMax']])

class TestFixture_SensorPlacement:
    def setup_method(self):
        self.testValues1 = {
            "type":"sensorType1",
            "mountPoint":[3.0,0.0,0.0],
            "orientation":[45.0,0.0,0.0],
        }

        self.testValues2 = {
            'type':"sensorType1",
            "mountPoint":[3.0,0.0,0.0],
            "orientation":[0.0,0.0,],
        }

d2r = pi/180.

class TestFixture_geometry:
    def setup_method(self):
        self.angleTests = [
            {
                'euler':r_[45,0,0],
                'quat':r_[np.sin(45./2.*d2r),0.0,0.0,np.cos(45./2.*d2r)],
            },
            {
                'euler':r_[-45.0,0.0,0.0],
                'quat':r_[np.sin(-45./2.*d2r),0.0,0.0,np.cos(45./2.*d2r)],
            }
        ]
    # def test_euler2quat(self):
    #     for trueEuler,trueQuat in self.testPairs:
    #         testQuat = euler2quat(trueEuler)
    #         assert(np.all(testQuat==trueQuat))

    def test_angleAxisToQuat(self):
        for testCase in self.angleTests:
            euler = testCase['euler']
            quat = testCase['quat']
            axisIndex = np.where(euler)[0][0]
            angle = euler[axisIndex]
            angleAxisToQuat(angle,axisIndex)

