import { useState, type FormEvent } from 'react';
import { useNavigate } from 'react-router-dom';
import { ApiError } from '../api/client';
import { useAuth } from '../auth/AuthContext';

export function LoginPage() {
    const { login } = useAuth();
    const navigate = useNavigate();

    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [submitting, setSubmitting] = useState(false);
    const [error, setError] = useState<string | null>(null);

    async function handleSubmit(e: FormEvent) {
        e.preventDefault();
        setError(null);
        setSubmitting(true);

        try {
        await login(username, password);
        navigate('/units', { replace: true });
        } catch (err: unknown) {
        if (err instanceof ApiError) {
            setError(`${err.code}: ${err.message}`);
        } else {
            setError('Unexpected error while logging in.');
        }
        } finally {
        setSubmitting(false);
        }
    }

    return (
        <section className="login-section">
        <h2>Sign in</h2>

        <form className="form" onSubmit={handleSubmit}>
            <div className="form-row">
            <label htmlFor="username">Username</label>
            <input
                id="username"
                type="text"
                autoComplete="username"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                required
            />
            </div>

            <div className="form-row">
            <label htmlFor="password">Password</label>
            <input
                id="password"
                type="password"
                autoComplete="current-password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                required
            />
            </div>

            <div className="form-actions">
            <button type="submit" className="btn-primary" disabled={submitting}>
                {submitting ? 'Signing in...' : 'Sign in'}
            </button>
            </div>
        </form>

        {error && <p className="error">{error}</p>}

        <p className="login-hint">
            Demo credentials: <code>admin</code> / <code>Admin123!</code>
        </p>
        </section>
    );
}