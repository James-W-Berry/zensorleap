import numpy as np
from numpy import r_, c_, pi

d2r = pi/180.0

def angleAxisToQuat(angle:float,axis:np.ndarray):
    return r_[np.sin(angle/2.0)*axis.reshape(3,),np.cos(angle/2.0)]

def eulerToQuat(euler:np.ndarray):
    return qmult(angleAxisToQuat(euler[0],r_[ 0.0, 0.0,-1.0]),
                 angleAxisToQuat(euler[1],r_[ 0.0,-1.0, 0.0]),
                 angleAxisToQuat(euler[2],r_[ 1.0, 0.0, 0.0]))

def qmult(*quats):
    if len(quats)==0:
        return r_[0.0,0.0,0.0,1.0]
    elif len(quats)==1:
        return quats[0]
    elif len(quats)==2:
        l,r = quats
        lMat = r_[
            c_[ l[3], l[2],-l[1], l[0]],
            c_[-l[2], l[3], l[0], l[1]],
            c_[ l[1],-l[0], l[3], l[2]],
            c_[-l[0],-l[1],-l[2], l[3]],
        ]
        result = np.dot(lMat,r.reshape((4,1)))
        return result.ravel()

    else:
        return qmult(quats[0],qmult(*tuple(quats[1:])))

def qinv(q):
    qi = r_[-1.0,-1.0,-1.0,1.0] * q
    return qi

def vecsMatch(vecL,vecR,threshold=1e-10):
    return np.linalg.norm(vecL-vecR)<threshold

def quatFromVec(vec):
    return r_[vec.ravel(),r_[0.0]]

def rotateAxes(quat,vec):
    return rotateVector(qinv(quat),vec)

def rotateVector(quat,vec):
    return qmult(quat,quatFromVec(vec),qinv(quat))[:-1].reshape(3,)


