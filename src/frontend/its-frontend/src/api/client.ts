const API_BASE = '/api/v1';
const TOKEN_KEY = 'its.auth.token';

export class ApiError extends Error {
    status: number;
    code: string;

    constructor(status: number, code: string, message: string) {
        super(message);
        this.status = status;
        this.code = code;
    }
}

function buildHeaders(includeJson: boolean): HeadersInit {
    const headers: Record<string, string> = {
        'Accept': 'application/json',
    };

    if (includeJson) {
        headers['Content-Type'] = 'application/json';
    }

    const token = localStorage.getItem(TOKEN_KEY);
    if (token) {
        headers['Authorization'] = `Bearer ${token}`;
    }

    return headers;
}

async function handleResponse<T>(response: Response): Promise<T> {
    if (!response.ok) {
        let code = 'UNKNOWN_ERROR';
        let message = `Request failed with status ${response.status}`;

        try {
        const body = await response.json();
        if (body?.error?.code) code = body.error.code;
        if (body?.error?.message) message = body.error.message;
        } catch {
        // ignorar errores de parseo
        }

        throw new ApiError(response.status, code, message);
    }

    return response.json() as Promise<T>;
}

export async function apiGet<T>(path: string): Promise<T> {
    const response = await fetch(`${API_BASE}${path}`, {
        method: 'GET',
        headers: buildHeaders(false),
    });

    return handleResponse<T>(response);
}

export async function apiPost<TRequest, TResponse>(
    path: string,
    body: TRequest,
    ): Promise<TResponse> {
    const response = await fetch(`${API_BASE}${path}`, {
        method: 'POST',
        headers: buildHeaders(true),
        body: JSON.stringify(body),
    });

    return handleResponse<TResponse>(response);
}