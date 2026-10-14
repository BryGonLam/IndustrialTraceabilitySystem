import { NavLink } from 'react-router-dom';

const links = [
    { to: '/monitoring', label: 'Monitoring' },
    { to: '/units', label: 'Units' },
    { to: '/orders', label: 'Orders' },
    { to: '/register', label: 'Register Event' },
    { to: '/products', label: 'Products' },
];

export function AppNav() {
    return (
        <nav className="tabs">
        {links.map((link) => (
            <NavLink
            key={link.to}
            to={link.to}
            className={({ isActive }) =>
                isActive ? 'tab tab-active' : 'tab'
            }
            >
            {link.label}
            </NavLink>
        ))}
        </nav>
    );
}