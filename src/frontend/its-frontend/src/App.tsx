import './App.css';
import { ProductsPage } from './pages/ProductsPage';

function App() {
  return (
    <div className="app">
      <header className="app-header">
        <h1>Industrial Traceability System</h1>
        <p>Fictitious industrial traceability system</p>
      </header>

      <main className="app-main">
        <ProductsPage />
      </main>
    </div>
  );
}

export default App;