#!/usr/bin/env python3.6

import sys
import io
import argparse
import json

from sensorDescriptionLibrary import SensorType


class FileSanitizer:
    def __init__(self,argv):
        self._defineArgs(progName=argv[0])
        self.parseArgs(argv)

        self.sensorTypes = []
        self.vehicles = []

    def _defineArgs(self,progName=__file__):
        self._argParser = argparse.ArgumentParser(prog=progName, description="script to help create a new SensorType file")
        self._argParser.add_argument("-s","--sensor", dest="sensors", action='append')
        self._argParser.add_argument("-v", "--vehicle", dest="vehicles", action='append')

    def parseArgs(self,argv):
        self.args = self._argParser.parse_args(argv[1:])
        self.sensorTypeFileNames = self.args.sensors
        self.vehicleFileNames = self.args.vehicles

    def loadSensorFiles(self):
        for sensorFileName in self.sensorTypeFileNames:
            with io.open(sensorFileName,"rb") as inputFile:
                jsonData = json.load(inputFile)
            self.sensorTypes.append(SensorType(jsonData=jsonData))

    def loadVehicleFiles(self):
        for vehicleFileName in self.vehicleFileNames:
            with io.open(vehicleFileName,"rb") as inputFile:
                jsonData = json.load(inputFile)
            self.vehicles.append(Vehicle(jsonData=jsonData))

    def run(self):
        jsonFileNames = self.args.jsonFileNames

        with io.open(jsonFileNames,"rb") as inFile:
            jsonContent = json.load(inFile)

        sensorType = SensorType()
        sensorType.load(jsonContent)

        outputFileName = f"{jsonFileNames}.cleaned.json"
        print(f"writing to {outputFileName}")
        outputJson = sensorType.toJson()

        with io.open(outputFileName,'wb') as outFile:
            outFile.write(outputJson)


if __name__=="__main__":
    creator = FileSanitizer()
    creator.run()