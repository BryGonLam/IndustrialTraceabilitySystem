import { Navigate, Route, Routes } from 'react-router-dom';
import './App.css';
import { AppNav } from './components/AppNav';
import { ProductsPage } from './pages/ProductsPage';
import { UnitsPage } from './pages/UnitsPage';
import { ProductionOrdersPage } from './pages/ProductionOrdersPage';
import { RegisterEventPage } from './pages/RegisterEventPage';
import { MonitoringPage } from './pages/MonitoringPage';

function App() {
  return (
    <div className="app">
      <header className="app-header">
        <h1>Industrial Traceability System</h1>
        <p>Fictitious industrial traceability system</p>
      </header>

      <AppNav />

      <main className="app-main">
        <Routes>
          <Route path="/" element={<Navigate to="/units" replace />} />
          <Route path="/units" element={<UnitsPage />} />
          <Route path="/orders" element={<ProductionOrdersPage />} />
          <Route path="/register" element={<RegisterEventPage />} />
          <Route path="/products" element={<ProductsPage />} />
          <Route path="/monitoring" element={<MonitoringPage />} />
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