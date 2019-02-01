import pytest
import json

from sanitizeFiles import FileSanitizer

class TestFixture_FileSanitizer:
    def setup_method(self):
        self.trueVehicleFileNames = {"vehicle1.json", "vehicle2.json"}
        self.trueSensorTypeFileNames = {"sensorType1.json", "sensorType2.json"}

        self.sanitizer = FileSanitizer(["sanitizeFiles.py",
                                   "-s", "sensorType1.json",
                                   "-v","vehicle1.json",
                                   "-s", "sensorType2.json",
                                   "-v", "vehicle2.json",
                                   ])

    def test_argLists(self):

        assert(set(self.sanitizer.sensorTypeFileNames) == self.trueSensorTypeFileNames)
        assert(set(self.sanitizer.vehicleFileNames) == self.trueVehicleFileNames)

    def test_loadFiles(self):
        self.sanitizer.loadSensorFiles()
        self.sanitizer.loadVehicleFiles()
