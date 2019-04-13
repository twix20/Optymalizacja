import numpy as np
from scipy.optimize import minimize

import utils


len_x = 2
len_y = 4
len_z = 3


r = [None] * len_x

for i in range(len_x):
    r[i] = [None] * len_y
    for j in range(len_y):
        r[i][j] = [None] * len_z
        for k in range(len_z):
            r[i][j][k] = i * j * k


# print(r)

vec = utils.toVector(r)
multiDim = utils.to3D(vec, len_x, len_y, len_z)

print(r)
print(len(vec))
print(multiDim)

# arr_1d = np.arange(12)
# arr_3d = arr_1d.reshape(2, 3, 2)


# print(arr_3d)
