import { useEffect, useState } from 'react';
import { apiGet, ApiError } from '../api/client';
import type { UnitSummary, UnitHistory } from '../api/types';
import { UnitStatusLabels } from '../api/types';
import { UnitHistoryView } from './UnitHistoryView';

function getStatusBadgeClass(status: number): string {
    switch (status) {
        case 4: return 'badge badge-green';   // Passed
        case 5: return 'badge badge-red';     // Scrapped
        case 2: return 'badge badge-yellow';  // OnHold
        case 3: return 'badge badge-orange';  // InRework
        default: return 'badge badge-gray';   // Created, InProgress
    }
}

export function UnitsPage() {
    const [units, setUnits] = useState<UnitSummary[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [selectedUnitId, setSelectedUnitId] = useState<number | null>(null);
    const [history, setHistory] = useState<UnitHistory | null>(null);
    const [historyLoading, setHistoryLoading] = useState(false);
    const [historyError, setHistoryError] = useState<string | null>(null);

  // Cargar lista de unidades
    useEffect(() => {
        let mounted = true;
        apiGet<UnitSummary[]>('/units')
        .then((data) => {
            if (mounted) {
            setUnits(data);
            setError(null);
            }
        })
        .catch((err: unknown) => {
            if (mounted) {
            setError(err instanceof ApiError ? `${err.code}: ${err.message}` : 'Unexpected error');
            }
        })
        .finally(() => {
            if (mounted) setLoading(false);
        });
        return () => { mounted = false; };
    }, []);

    // Cargar historial cuando se selecciona una unidad
    useEffect(() => {
        if (selectedUnitId === null) {
        setHistory(null);
        return;
        }
        let mounted = true;
        setHistoryLoading(true);
        setHistoryError(null);
        apiGet<UnitHistory>(`/units/${selectedUnitId}/history`)
        .then((data) => {
            if (mounted) setHistory(data);
        })
        .catch((err: unknown) => {
            if (mounted) {
            setHistoryError(err instanceof ApiError ? `${err.code}: ${err.message}` : 'Unexpected error');
            }
        })
        .finally(() => {
            if (mounted) setHistoryLoading(false);
        });
        return () => { mounted = false; };
    }, [selectedUnitId]);

    return (
        <section>
        <h2>Units</h2>

        {loading && <p>Loading...</p>}
        {error && <p className="error">{error}</p>}

        {!loading && !error && units.length === 0 && (
            <p>No units found.</p>
        )}

        {!loading && !error && units.length > 0 && (
            <table className="table">
            <thead>
                <tr>
                <th>Serial</th>
                <th>Order</th>
                <th>Status</th>
                <th>Created</th>
                </tr>
            </thead>
            <tbody>
                {units.map((u) => (
                <tr
                    key={u.id}
                    className={selectedUnitId === u.id ? 'row-selected' : 'row-clickable'}
                    onClick={() => setSelectedUnitId(u.id)}
                >
                    <td><strong>{u.serialNumber}</strong></td>
                    <td>{u.orderNumber}</td>
                    <td>
                    <span className={getStatusBadgeClass(u.status)}>
                        {UnitStatusLabels[u.status] ?? u.status}
                    </span>
                    </td>
                    <td>{new Date(u.createdAt).toLocaleString()}</td>
                </tr>
                ))}
            </tbody>
            </table>
        )}

        {selectedUnitId !== null && (
            <div className="history-section">
            <div className="history-header">
                <h3>History</h3>
                <button className="btn-close" onClick={() => setSelectedUnitId(null)}>
                Close
                </button>
            </div>

            {historyLoading && <p>Loading history...</p>}
            {historyError && <p className="error">{historyError}</p>}
            {history && !historyLoading && (
                <UnitHistoryView history={history} />
            )}
            </div>
        )}
    </section>
    );
}