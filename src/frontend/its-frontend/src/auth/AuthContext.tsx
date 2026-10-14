import { createContext, useContext, useEffect, useState, type ReactNode } from 'react';
import { apiGet, apiPost, ApiError } from '../api/client';
import type {
    LoginRequest,
    LoginResponse,
    UserInfo,
} from '../api/types';

const TOKEN_KEY = 'its.auth.token';
const USER_KEY = 'its.auth.user';

interface AuthState {
    user: UserInfo | null;
    token: string | null;
    loading: boolean;
}

interface AuthContextValue extends AuthState {
    login: (username: string, password: string) => Promise<void>;
    logout: () => void;
}

const AuthContext = createContext<AuthContextValue | null>(null);

function readStoredUser(): UserInfo | null {
    const raw = localStorage.getItem(USER_KEY);
    if (!raw) return null;
    try {
        return JSON.parse(raw) as UserInfo;
    } catch {
        return null;
    }
}

export function AuthProvider({ children }: { children: ReactNode }) {
    const [user, setUser] = useState<UserInfo | null>(() => readStoredUser());
    const [token, setToken] = useState<string | null>(() => localStorage.getItem(TOKEN_KEY));
    const [loading, setLoading] = useState<boolean>(true);

  // Al montar, si hay token guardado, verificamos con /auth/me
    useEffect(() => {
        let mounted = true;

        if (!token) {
        setLoading(false);
        return () => { mounted = false; };
        }

        apiGet<UserInfo>('/auth/me')
        .then((me) => {
            if (mounted) {
            setUser(me);
            localStorage.setItem(USER_KEY, JSON.stringify(me));
            }
        })
        .catch((err: unknown) => {
            if (mounted) {
            // Token invalido o expirado: limpiamos
            if (err instanceof ApiError && (err.status === 401 || err.status === 403)) {
                localStorage.removeItem(TOKEN_KEY);
                localStorage.removeItem(USER_KEY);
                setUser(null);
                setToken(null);
            }
            }
        })
        .finally(() => {
            if (mounted) setLoading(false);
        });

        return () => { mounted = false; };
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, []);

    async function login(username: string, password: string) {
        const body: LoginRequest = { username, password };
        const response = await apiPost<LoginRequest, LoginResponse>('/auth/login', body);

        localStorage.setItem(TOKEN_KEY, response.token);
        localStorage.setItem(USER_KEY, JSON.stringify(response.user));

        setToken(response.token);
        setUser(response.user);
    }

    function logout() {
        localStorage.removeItem(TOKEN_KEY);
        localStorage.removeItem(USER_KEY);
        setUser(null);
        setToken(null);
    }

    return (
        <AuthContext.Provider value={{ user, token, loading, login, logout }}>
        {children}
        </AuthContext.Provider>
    );
}

export function useAuth(): AuthContextValue {
    const ctx = useContext(AuthContext);
    if (!ctx) {
        throw new Error('useAuth must be used inside AuthProvider');
    }
    return ctx;
}