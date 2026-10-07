const API_BASE = '/api/v1';

export class ApiError extends Error {
    status: number;
    code: string;

    constructor(status: number, code: string, message: string){
        super(message);
        this.status = status;
        this.code = code;
    }
}

export async function apiGet<T>(path: string): Promise<T> {
    const response = await fetch(`${API_BASE}${path}`, {
        method: 'GET',
        headers: {
            'Accept': 'application/json',
        },
    });

    if (!response.ok) {
        let code = 'UNKNOWN_ERROR';
        let message = `Request failed with status ${response.status}`;

        try {
            const body = await response.json();
            if (body?.error?.code) code = body.error.code;
            if (body?.error?.message) message = body.error.message;
        } catch {
            // Se ignora
        }

        throw new ApiError(response.status, code, message);
    }
    
    return response.json() as Promise<T>;
}

export async function apiPost<TRequest, TResponse>(
    path: string,
    body: TRequest,
): Promise<TResponse> {
    const response = await fetch (`${API_BASE}${path}`, {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(body),
    });
    
    if (!response.ok) {
        let code = 'UNKNOWN_ERROR';
        let message = `Request failed with status ${response.status}`;

        try {
            const data = await response.json();
            if (data?.error?.code) code = data.error.code;
            if (data?.error?.message) message = data.error.message;
        } catch {
            // ignorar errores de parseo
        }

        throw new ApiError(response.status, code, message);
    }
    
    return response.json() as Promise<TResponse>;
}