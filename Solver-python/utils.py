import numpy as np


def toVector(a):
    return np.hstack([np.hstack(b) for b in a])


def to3D(vector, len_x, len_y, len_z):
    return vector.reshape((len_x, len_y, len_z))
