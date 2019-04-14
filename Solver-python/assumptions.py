def C_l(l):
    """fizyczna przepustowość łącza l"""
    return 1024 * 30


def B_s(s):
    """pojemność dyskowa serwera s"""
    return 1024 * 2


def b_n(n):
    """wielkość obiektu n"""
    return 1024 * (n + 1)


def k_l(l):
    """cena za jednostkę transmisji na łączu l"""
    return 0.001


def d_ns(n, s):
    """cena za składowanie obiektu n na serwerze s"""
    return n * s


def a_mnsl(x_mns, m, n, s, l):
    """zmienna [0, 1] czy użytkownik m używa łącza l podczas pobierania obiektu n z serwera s"""
    return x_mns[m][n][s]


def y_mn_min(m, n):
    """"""
    return 0


def y_mn_max(m, n):
    """"""
    return 1000000
