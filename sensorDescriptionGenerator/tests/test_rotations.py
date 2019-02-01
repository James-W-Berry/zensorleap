import pytest
import numpy as np
from numpy import r_, c_, pi

import rotations

d2r = pi/180.0
rt2_2 = np.sqrt(2.0)/2.0

def makeTestCase(angle:float, axisNumber:int):
    testCaseInfo = {}

    #Zenuity convention: using -3, -2, 1 euler angles:
    # rotate
    # counterclockwise about -z axis then
    # counterlockwise about -y axis, then
    # counterclockwise about +x axis
    if axisNumber not in [-3,-2,1]:
        raise ValueError(f"axisNumber must be -3, -2, or 1, not {axisNumber}")

    angleRad = d2r * angle

    testCaseInfo['angle'] = angleRad
    testCaseInfo['euler'] = (
        r_[angleRad,0,0] if axisNumber == -3 else
        r_[0.0,angleRad,0.0] if axisNumber == -2 else
        r_[0.0, 0.0, angleRad] if axisNumber == 1 else
        r_[np.nan,np.nan,np.nan] #impossible!
    )
    testCaseInfo['quat'] = r_[0.0, 0.0, 0.0, 1.0]
    testCaseInfo['quat'][np.abs(axisNumber)-1] = np.sign(axisNumber) * np.sin(angleRad/2.0)
    testCaseInfo['quat'][3] = np.cos(angleRad/2.0)
    testCaseInfo['axis'] = (
        r_[0.0,0.0,-1.0] if axisNumber==-3 else
        r_[0.0,-1.0,0.0] if axisNumber==-2 else
        r_[1.0,0.0,0.0] if axisNumber==1 else
        r_[np.nan,np.nan,np.nan] #impossible!
    )

    return testCaseInfo



class TestFixture_rotations:
    def setup_method(self):
        self.rotationTestCases = [
            makeTestCase(10.0,-3),
            makeTestCase(-10.0,-3),
            makeTestCase(10.0,-2),
            makeTestCase(-10.0,-2),
            makeTestCase(10.0,1),
            makeTestCase(-10.0,1),
        ]

    def test_angleAxisToQuat(self):
        for rtc in self.rotationTestCases:
            testQuat = rotations.angleAxisToQuat(rtc['angle'],rtc['axis'])
            assert(np.all(rtc['quat']==testQuat))

    def test_eulerToQuat(self):
        for rtc in self.rotationTestCases:
            testQuat = rotations.eulerToQuat(rtc['euler'])
            assert(np.all((rtc['quat']-testQuat)<1e-10))

    def test_vecsMatch(self):
        testVector = r_[1.892,4.3,8.5]
        assert(rotations.vecsMatch(testVector,testVector+r_[0.0,0.0,1e-11],threshold=1e-10))
        assert(not rotations.vecsMatch(testVector,testVector+r_[0.0,0.0,1e-9],threshold=1e-10))

    def test_quatFromVec(self):
        vec = r_[1.,2.,3.]
        quatFromVecTrue = r_[1.,2.,3.,0.]
        quatFromVec = rotations.quatFromVec(vec)
        assert(np.all(quatFromVec==quatFromVecTrue))

    def test_rotateAxes(self):
        result = rotations.rotateAxes(rotations.eulerToQuat(d2r * r_[45.0, 0.0, 0.0]), r_[1.0, 0.0, 0.0])

        assert (rotations.vecsMatch(result, rt2_2 * r_[1.0,-1.0, 0.0]))

    def test_rotateVector(self):
        result = rotations.rotateVector(rotations.eulerToQuat(d2r*r_[45.0,0.0,0.0]),r_[1.0,0.0,0.0])

        assert(rotations.vecsMatch(result,rt2_2*r_[1.0,1.0,0.0]))

