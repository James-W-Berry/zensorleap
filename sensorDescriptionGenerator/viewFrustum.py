import numpy as np
from numpy import r_, c_, pi

from typing import Tuple
from rotations import qmult, eulerToQuat, rotateVector, d2r

class ViewFrustum:
    def __init__(self,azimuthExtent:Tuple[float,float],elevationExtent:Tuple[float,float],rangeExtent:Tuple[float,float]):
        self._points = np.zeros((8,3))
        minAz,maxAz = azimuthExtent
        minEl,maxEl = elevationExtent
        minRange,maxRange = rangeExtent

        self.rotationsAndScale = r_[
            c_[minAz,maxEl,minRange],
            c_[maxAz,maxEl,minRange],
            c_[maxAz,minEl,minRange],
            c_[minAz,minEl,minRange],
            c_[minAz,maxEl,maxRange],
            c_[maxAz,maxEl,maxRange],
            c_[maxAz,minEl,maxRange],
            c_[minAz,minEl,maxRange],
        ]

        for ptIdx in range(self._points.shape[0]):
            az = d2r*self.rotationsAndScale[ptIdx,0]
            el = d2r*self.rotationsAndScale[ptIdx,1]
            rng = self.rotationsAndScale[ptIdx,2]
            self._points[ptIdx, :] = rng * rotateVector(
                qmult(
                    eulerToQuat(r_[-el, 0.0, 0.0]),
                    eulerToQuat(r_[0.0, -az, 0.0])
                ),
                r_[0.0, 0.0, 1.0]
            )

    def getPoints(self):
        return np.copy(self._points)
