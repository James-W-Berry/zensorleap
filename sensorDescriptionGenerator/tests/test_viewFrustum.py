import pytest
import numpy as np

from viewFrustum import ViewFrustum

class TestFixture_ViewFrustum:
    def test_construct(self):
        rangeExtent = minRange,maxRange = 2.0,70.0
        azimuthExtent = minAx,maxAz = -15.0,15.0
        elevationExtent = minEl,maxEl = -10.0,10.0
        vf = ViewFrustum(azimuthExtent,elevationExtent,rangeExtent)

        points = vf.getPoints()

        print()
        print(f"range:{rangeExtent}")
        print(f"azimuth:{azimuthExtent}")
        print(f"elevation:{elevationExtent}")
        print(points)
        # print(np.linalg.norm(points,axis=1))

        for ii in range(4):
            assert(np.linalg.norm(points[ii,:]) == minRange)

        for ii in range(4,8):
            assert(np.linalg.norm(points[ii,:]) == maxRange)


    # def test_plotFrustum(self):
    #     import matplotlib.pyplot as plt
    #     from mpl_toolkits.mplot3d import Axes3D
    #     fig = plt.figure()
    #     ax = fig.add_subplot(111, projection='3d')
    #
    #
    #     rangeExtent = minRange,maxRange = 2.0,7.0
    #     azimuthExtent = minAx,maxAz = -15.0,15.0
    #     elevationExtent = minEl,maxEl = -10.0,10.0
    #     vf = ViewFrustum(azimuthExtent,elevationExtent,rangeExtent)
    #     points = vf.getPoints()
    #
    #
    #     ax.plot(points[:,0],points[:,1],points[:,2],'r.')
    #     plt.show()

