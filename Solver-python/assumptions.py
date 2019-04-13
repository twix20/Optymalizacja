def C_l(l):
    return 1024 * 30


def B_s(s):
    return 1024 * 2


def b_n(n):
    return 1024 * (n + 1)


def k_l(l):
    return 0.001


def d_ns(n, s):
    return n * s


def a_mnsl(x_mns, m, n, s, l):
    return x_mns[m][n][s]
