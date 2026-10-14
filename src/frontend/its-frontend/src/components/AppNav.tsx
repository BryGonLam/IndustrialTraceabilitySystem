import { NavLink, useNavigate } from 'react-router-dom';
import { useAuth } from '../auth/AuthContext';

const links = [
    { to: '/monitoring', label: 'Monitoring' },
    { to: '/units', label: 'Units' },
    { to: '/orders', label: 'Orders' },
    { to: '/register', label: 'Register Event' },
    { to: '/products', label: 'Products' },
];

export function AppNav() {
    const { user, logout } = useAuth();
    const navigate = useNavigate();

    function handleLogout() {
        logout();
        navigate('/login', { replace: true });
    }

    return (
        <div className="nav-container">
        <nav className="tabs">
            {links.map((link) => (
            <NavLink
                key={link.to}
                to={link.to}
                className={({ isActive }) => (isActive ? 'tab tab-active' : 'tab')}
            >
                {link.label}
            </NavLink>
            ))}
        </nav>

        {user && (
            <div className="nav-user">
            <span className="nav-user-name">{user.displayName}</span>
            <span className="nav-user-role">{user.roles.join(', ')}</span>
            <button className="btn-logout" onClick={handleLogout}>
                Sign out
            </button>
            </div>
        )}
        </div>
    );
}