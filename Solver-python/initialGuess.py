import numpy as np


def getX0_mns(users, objects, servers, links):
    x0_mns = np.arange(users.size, dtype=object)

    for m in users:
        x0_mns[m] = np.arange(1, objects.size + 2, dtype=object)

        for n in objects:
            x0_mns[m][n] = np.arange(1, servers.size + 2, dtype=float)

            for s in servers:
                x0_mns[m][n][s] = 0.0

    return x0_mns


def getY0_mns(users, objects, servers, links):
    y0_mns = np.arange(users.size, dtype=object)

    for m in users:
        y0_mns[m] = np.arange(1, objects.size + 2, dtype=object)

        for n in objects:
            y0_mns[m][n] = np.arange(1, servers.size + 2, dtype=float)

            for s in servers:
                y0_mns[m][n][s] = 512.0

    return y0_mns
