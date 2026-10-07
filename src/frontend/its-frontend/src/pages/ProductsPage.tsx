import { useEffect, useState } from 'react';
import { apiGet, ApiError } from '../api/client';
import type { Product } from '../api/types';

export function ProductsPage() {
    const [products, setProducts] = useState<Product[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        let mounted = true;

        apiGet<Product[]>('/products')
        .then((data) => {
            if (mounted) {
            setProducts(data);
            setError(null);
            }
        })
        .catch((err: unknown) => {

            if (mounted) {
                if (err instanceof ApiError) {
                    setError(`${err.code}: ${err.message}`);
                } else {
                    setError('Unexpected error while loading products.');
                }   
            }
        })
        .finally(() => {
            if (mounted) setLoading(false);
        });

    return () => {
        mounted = false;
        };
    }, []);

    return (
        <section>
        <h2>Products</h2>

        {loading && <p>Loading...</p>}
        {error && <p className="error">{error}</p>}

        {!loading && !error && products.length === 0 && (
            <p>No products found.</p>
        )}

        {!loading && !error && products.length > 0 && (
            <table className="table">
            <thead>
                <tr>
                <th>Code</th>
                <th>Name</th>
                <th>Description</th>
                <th>Active</th>
                </tr>
            </thead>
            <tbody>
                {products.map((p) => (
                <tr key={p.id}>
                    <td>{p.code}</td>
                    <td>{p.name}</td>
                    <td>{p.description ?? '-'}</td>
                    <td>{p.isActive ? 'Yes' : 'No'}</td>
                </tr>
                ))}
            </tbody>
            </table>
        )}
        </section>
    );
}