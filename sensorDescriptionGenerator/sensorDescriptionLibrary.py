from typing import Tuple
class SensorType:

    defaults = {
        'name':'',
        'azimuthMin': -15.0,
        'azimuthMax': 15.0,
        'elevationMin':-10.0,
        'elevationMax': 10.0,
        'rangeMin': 2.0,
        'rangeMax': 70.0,
    }
    def __init__(self,jsonData=None):
        self._name = self.defaults['name']
        self._azimuthExtent:Tuple[float,float] = [self.defaults['azimuthMin'],self.defaults['azimuthMax']]
        self._elevationExtent:Tuple[float,float] = [self.defaults['elevationMin'],self.defaults['elevationMax']]
        self._rangeExtent:Tuple[float,float] = [self.defaults['rangeMin'],self.defaults['rangeMax']]

        if jsonData is not None:
            self.load(jsonData)

    def getAzimuthExtent(self):
        return [float(self._azimuthExtent[0]), float(self._azimuthExtent[1])]

    def getElevationExtent(self):
        return [float(self._elevationExtent[0]), float(self._elevationExtent[1])]

    def getRangeExtent(self):
        return [float(self._rangeExtent[0]), float(self._rangeExtent[1])]

    def load(self, parsedJson):
        attributeValues = {
            attributeName:parsedJson.get(attributeName, defaultValue)
            for attributeName, defaultValue in self.defaults.items()
        }


        self._name = attributeValues['name']
        self._azimuthExtent = self._getMinAndMax(attributeValues, 'azimuth')
        self._elevationExtent = self._getMinAndMax(attributeValues, 'elevation')
        self._rangeExtent = self._getMinAndMax(attributeValues, 'range')

    def _getMinAndMax(self, dataSource, keyNameBase):
        return sorted([float(dataSource[f'{keyNameBase}Min']), float(dataSource[f'{keyNameBase}Max'])])

    def toJson(self):
        indentText = "    "
        jsonText = f"""{{
{indentText}"name": "{self._name}",
{indentText}"azimuthMin": {self.getAzimuthExtent()[0]},
{indentText}"azimuthMax": {self.getAzimuthExtent()[1]},
{indentText}"elevationMin": {self.getElevationExtent()[0]},
{indentText}"elevationMax": {self.getElevationExtent()[1]},
{indentText}"rangeMin": {self.getRangeExtent()[0]},
{indentText}"rangeMax": {self.getRangeExtent()[1]}
{indentText}}}
"""
        return jsonText.encode("utf8")

    def __eq__(self,other):
        return (
            (self._name == other._name) and
            (self.getAzimuthExtent() == other.getAzimuthExtent()) and
            (self.getElevationExtent() == other.getElevationExtent()) and
            (self.getRangeExtent() == other.getRangeExtent())
        )

class SensorPlacement:
    defaults = {
        'name':'',
        'type':'',
        'mountPoint':(0.0,0.0,0.0),
        'orientation':(0.0,0.0,0.0,1.0),
    }

class Vehicle:
    defaults = {
        'name':'',
        'sensors':{}
    }


def euler2quat(euler):
    """
    Convert -3,2,1 euler angles to quaternions
    :param euler:
    :return:
    """


def angleAxisToQuat(angle:float,axis:int):
    """
    Express a rotation about ``axis`` by ``angle`` as a quaternion
    :param angle:
    :param axis:
    :return:
    """


