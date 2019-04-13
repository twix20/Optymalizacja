import numpy as np


def getX0_mns(users, objects, servers, links):
    x0_mns = [None] * users.size
    for i in range(users.size):
        x0_mns[i] = [None] * objects.size
        for j in range(objects.size):
            x0_mns[i][j] = [None] * servers.size
            for k in range(servers.size):
                x0_mns[i][j][k] = 0

    # return x0_mns
    return np.random.randint(0, 2, size=(users.size, objects.size, servers.size))


def getY0_mns(users, objects, servers, links):
    y0_mns = [None] * users.size
    for i in range(users.size):
        y0_mns[i] = [None] * objects.size
        for j in range(objects.size):
            y0_mns[i][j] = [None] * servers.size
            for k in range(servers.size):
                y0_mns[i][j][k] = 512.0

    # return y0_mns
    return np.random.randint(100, 512, size=(users.size, objects.size, servers.size))
