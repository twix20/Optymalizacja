import numpy as np
from scipy.optimize import minimize

import assumptions
import initialGuess

# python -m pip install --user numpy scipy


users = np.arange(5)
objects = np.arange(1, 5)
servers = np.arange(1, 2)
links = np.arange(1, users.size * (servers.size + 1))


x0_mns = initialGuess.getX0_mns(users, objects, servers, links)
y0_mns = initialGuess.getY0_mns(users, objects, servers, links)
ig = np.array([x0_mns, y0_mns])


def total_y_mn(y, m, n):
    total = 0
    for s in servers:
        total += y[m][n][s]

    return total * assumptions.b_n(n)


def total_y_mns(y, m, n, s):
    total = y[m][n][s]

    return total * assumptions.b_n(n)


def Q(x, y):
    U_r = 100 * 100  # U(y)
    G_r = G(x, y)
    H_r = H(x[0])

    return U_r - G_r - H_r


def U(y):
    r = 0

    for n in objects:
        leftStatement = sum(total_y_mns(y, 0, n, s) for s in servers)
        rightStatement = sum(total_y_mn(y, m, n) for m in users[1:])

        r += leftStatement + rightStatement

    return r


def G(x, y):
    r = 0
    for n in objects:
        for s in servers:
            for l in links:

                leftStatement = (
                    assumptions.a_mnsl(x, 0, n, s, l) * x[0][n][s] * y[0][n][s]
                )
                rigtStatement = sum(
                    assumptions.a_mnsl(x, m, n, s, l) * x[m][n][s] * total_y_mn(y, m, n)
                    for m in users[1:]
                )

                r += assumptions.k_l(l) * sum([leftStatement, rigtStatement])

    return r


def H(x_0):
    r = 0
    for n in objects:
        for s in servers:
            r += assumptions.d_ns(n, s) * x_0[n][s]

    return r


def f(params):
    usersSize = users.size
    x = params[0:usersSize]
    y = params[usersSize:]

    return -1 * Q(x, y)  # because we want to maximize


res = minimize(f, ig, options={"disp": True})


print(res)

