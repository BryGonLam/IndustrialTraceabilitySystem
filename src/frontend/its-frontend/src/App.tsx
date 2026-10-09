import { useState } from 'react';
import './App.css';
import { ProductsPage } from './pages/ProductsPage';
import { UnitsPage } from './pages/UnitsPage';
import { ProductionOrdersPage } from './pages/ProductionOrdersPage';
import { RegisterEventPage } from './pages/RegisterEventPage';

type Tab = 'units' | 'orders' | 'register' | 'products';

function App() {
  const [tab, setTab] = useState<Tab>('units');

  return (
    <div className="app">
      <header className="app-header">
        <h1>Industrial Traceability System</h1>
        <p>Fictitious industrial traceability system</p>
      </header>

      <nav className="tabs">
        <button
          className={tab === 'units' ? 'tab tab-active' : 'tab'}
          onClick={() => setTab('units')}
        >
          Units
        </button>
        <button
          className={tab === 'orders' ? 'tab tab-active' : 'tab'}
          onClick={() => setTab('orders')}
        >
          Orders
        </button>
        <button
          className={tab === 'register' ? 'tab tab-active' : 'tab'}
          onClick={() => setTab('register')}
        >
          Register Event
        </button>
        <button
          className={tab === 'products' ? 'tab tab-active' : 'tab'}
          onClick={() => setTab('products')}
        >
          Products
        </button>
      </nav>

      <main className="app-main">
        {tab === 'units' && <UnitsPage />}
        {tab === 'orders' && <ProductionOrdersPage />}
        {tab === 'register' && <RegisterEventPage />}
        {tab === 'products' && <ProductsPage />}
      </main>
    </div>
  );
}

export default App;