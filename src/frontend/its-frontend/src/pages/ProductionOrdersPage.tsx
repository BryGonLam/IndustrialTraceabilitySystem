import { useEffect, useState } from 'react';
import { apiGet, ApiError } from '../api/client';
import type {
    ProductionOrderSummary,
    ProductionOrderDetail,
    UnitHistory,
} from '../api/types';
import {
    UnitStatusLabels,
    ProductionOrderStatusLabels,
} from '../api/types';
import { UnitHistoryView } from './UnitHistoryView';

function getOrderStatusBadgeClass(status: number): string {
    switch (status) {
        case 1: return 'badge badge-green';   // InProgress
        case 2: return 'badge badge-gray';    // Closed
        case 3: return 'badge badge-red';     // Cancelled
        default: return 'badge badge-yellow'; // Planned
    }
}

function getUnitStatusBadgeClass(status: number): string {
    switch (status) {
        case 4: return 'badge badge-green';
        case 5: return 'badge badge-red';
        case 2: return 'badge badge-yellow';
        case 3: return 'badge badge-orange';
        default: return 'badge badge-gray';
    }
}

export function ProductionOrdersPage() {
    const [orders, setOrders] = useState<ProductionOrderSummary[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const [selectedOrderId, setSelectedOrderId] = useState<number | null>(null);
    const [orderDetail, setOrderDetail] = useState<ProductionOrderDetail | null>(null);
    const [detailLoading, setDetailLoading] = useState(false);
    const [detailError, setDetailError] = useState<string | null>(null);

    const [selectedUnitId, setSelectedUnitId] = useState<number | null>(null);
    const [unitHistory, setUnitHistory] = useState<UnitHistory | null>(null);
    const [historyLoading, setHistoryLoading] = useState(false);
    const [historyError, setHistoryError] = useState<string | null>(null);

  // Cargar lista de ordenes
    useEffect(() => {
        let mounted = true;
        apiGet<ProductionOrderSummary[]>('/production-orders')
        .then((data) => {
            if (mounted) {
            setOrders(data);
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

    // Cargar detalle de la orden seleccionada
    useEffect(() => {
        if (selectedOrderId === null) {
        setOrderDetail(null);
        return;
        }
        let mounted = true;
        setDetailLoading(true);
        setDetailError(null);
        setSelectedUnitId(null);
        setUnitHistory(null);
        apiGet<ProductionOrderDetail>(`/production-orders/${selectedOrderId}`)
        .then((data) => {
            if (mounted) setOrderDetail(data);
        })
        .catch((err: unknown) => {
            if (mounted) {
            setDetailError(err instanceof ApiError ? `${err.code}: ${err.message}` : 'Unexpected error');
            }
        })
        .finally(() => {
            if (mounted) setDetailLoading(false);
        });
        return () => { mounted = false; };
    }, [selectedOrderId]);

    // Cargar historial de la unidad seleccionada
    useEffect(() => {
        if (selectedUnitId === null) {
        setUnitHistory(null);
        return;
        }
        let mounted = true;
        setHistoryLoading(true);
        setHistoryError(null);
        apiGet<UnitHistory>(`/units/${selectedUnitId}/history`)
        .then((data) => {
            if (mounted) setUnitHistory(data);
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
        <h2>Production O        rders</h2>

        {loading && <p>Loading...</p>}
        {error && <p className="error">{error}</p>}

        {!loading && !error && orders.length === 0 && (
            <p>No production orders found.</p>
        )}

        {!loading && !error && orders.length > 0 && (
            <table className="table">
            <thead>
                <tr>
                <th>Order</th>
                <th>Product</th>
                <th>Line</th>
                <th>Planned</th>
                <th>Units</th>
                <th>Status</th>
                <th>Created</th>
                </tr>
            </thead>
            <tbody>
                {orders.map((o) => (
                <tr
                    key={o.id}
                    className={selectedOrderId === o.id ? 'row-selected' : 'row-clickable'}
                    onClick={() => setSelectedOrderId(o.id)}
                >
                    <td><strong>{o.orderNumber}</strong></td>
                    <td>{o.productCode}</td>
                    <td>{o.productionLineCode}</td>
                    <td>{o.plannedQuantity}</td>
                    <td>{o.totalUnits}</td>
                    <td>
                    <span className={getOrderStatusBadgeClass(o.status)}>
                        {ProductionOrderStatusLabels[o.status] ?? o.status}
                    </span>
                    </td>
                    <td>{new Date(o.createdAt).toLocaleString()}</td>
                </tr>
                ))}
            </tbody>
            </table>
        )}

        {selectedOrderId !== null && (
            <div className="history-section">
            <div className="history-header">
                <h3>Order detail</h3>
                <button className="btn-close" onClick={() => setSelectedOrderId(null)}>
                Close
                </button>
            </div>

            {detailLoading && <p>Loading order detail...</p>}
            {detailError && <p className="error">{detailError}</p>}

            {orderDetail && !detailLoading && (
                <>
                <div className="history-meta">
                    <p><strong>Order:</strong> {orderDetail.orderNumber}</p>
                    <p><strong>Product:</strong> {orderDetail.productCode} — {orderDetail.productName}</p>
                    <p><strong>Line:</strong> {orderDetail.productionLineCode} — {orderDetail.productionLineName}</p>
                    <p><strong>Status:</strong> {ProductionOrderStatusLabels[orderDetail.status] ?? orderDetail.status}</p>
                </div>

                <h4>Units ({orderDetail.units.length})</h4>

                {orderDetail.units.length === 0 && (
                    <p>No units registered for this order.</p>
                )}

                {orderDetail.units.length > 0 && (
                    <table className="table">
                    <thead>
                        <tr>
                        <th>Serial</th>
                        <th>Status</th>
                        <th>Created</th>
                        </tr>
                    </thead>
                    <tbody>
                        {orderDetail.units.map((u) => (
                        <tr
                            key={u.id}
                            className={selectedUnitId === u.id ? 'row-selected' : 'row-clickable'}
                            onClick={() => setSelectedUnitId(u.id)}
                        >
                            <td><strong>{u.serialNumber}</strong></td>
                            <td>
                            <span className={getUnitStatusBadgeClass(u.status)}>
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
                    <div className="history-section history-section-nested">
                    <div className="history-header">
                        <h4>Unit history</h4>
                        <button className="btn-close" onClick={() => setSelectedUnitId(null)}>
                        Close
                        </button>
                    </div>

                    {historyLoading && <p>Loading history...</p>}
                    {historyError && <p className="error">{historyError}</p>}
                    {unitHistory && !historyLoading && (
                        <UnitHistoryView history={unitHistory} />
                    )}
                    </div>
                )}
                </>
            )}
            </div>
        )}
        </section>
    );
}