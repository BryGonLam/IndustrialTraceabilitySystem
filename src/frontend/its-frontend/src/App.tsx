import { Navigate, Route, Routes, useLocation } from 'react-router-dom';
import './App.css';
import { AppNav } from './components/AppNav';
import { ProductsPage } from './pages/ProductsPage';
import { UnitsPage } from './pages/UnitsPage';
import { ProductionOrdersPage } from './pages/ProductionOrdersPage';
import { RegisterEventPage } from './pages/RegisterEventPage';
import { MonitoringPage } from './pages/MonitoringPage';
import { LoginPage } from './pages/LoginPage';
import { ProtectedRoute } from './auth/ProtectedRoute';
import { useAuth } from './auth/AuthContext';

function App() {
  const { user } = useAuth();
  const location = useLocation();
  const isLoginPage = location.pathname === '/login';

  if (isLoginPage) {
    return (
      <div className="app">
        <header className="app-header">
          <h1>Industrial Traceability System</h1>
          <p>Fictitious industrial traceability system</p>
        </header>
        <main className="app-main">
          <Routes>
            <Route path="/login" element={<LoginPage />} />
          </Routes>
        </main>
      </div>
    );
  }

  return (
    <div className="app">
      <header className="app-header">
        <h1>Industrial Traceability System</h1>
        <p>Fictitious industrial traceability system</p>
      </header>

      {user && <AppNav />}

      <main className="app-main">
        <Routes>
          <Route path="/login" element={<Navigate to="/" replace />} />
          <Route
            path="/monitoring"
            element={<ProtectedRoute><MonitoringPage /></ProtectedRoute>}
          />
          <Route
            path="/units"
            element={<ProtectedRoute><UnitsPage /></ProtectedRoute>}
          />
          <Route
            path="/orders"
            element={<ProtectedRoute><ProductionOrdersPage /></ProtectedRoute>}
          />
          <Route
            path="/register"
            element={<ProtectedRoute><RegisterEventPage /></ProtectedRoute>}
          />
          <Route
            path="/products"
            element={<ProtectedRoute><ProductsPage /></ProtectedRoute>}
          />
          <Route path="/" element={<Navigate to="/units" replace />} />
          <Route path="*" element={<NotFoundPage />} />
        </Routes>
      </main>
    </div>
  );
}

function NotFoundPage() {
  return (
    <section>
      <h2>Page not found</h2>
      <p>The page you are looking for does not exist.</p>
    </section>
  );
}

export default App;