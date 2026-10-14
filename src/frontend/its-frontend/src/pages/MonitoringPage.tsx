import { useEffect, useState } from 'react';
import { apiGet, ApiError } from '../api/client';
import type { DashboardSummary, LineStatus, UnitsSummary } from '../api/types';

interface Data {
    dashboard: DashboardSummary;
    lines: LineStatus[];
    units: UnitsSummary;
}

export function MonitoringPage() {
    const [data, setData] = useState<Data | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        let mounted = true;
        Promise.all([
        apiGet<DashboardSummary>('/monitoring/dashboard'),
        apiGet<LineStatus[]>('/monitoring/lines'),
        apiGet<UnitsSummary>('/monitoring/units-summary'),
        ])
        .then(([dashboard, lines, units]) => {
            if (mounted) {
            setData({ dashboard, lines, units });
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

    if (loading) return <p>Loading...</p>;
    if (error) return <p className="error">{error}</p>;
    if (!data) return <p>No data.</p>;

    const { dashboard, lines, units } = data;

    return (
        <section>
        <h2>Monitoring</h2>

        <h3>Overview</h3>
        <div className="cards">
            <Card label="Products" value={dashboard.totalProducts} />
            <Card label="Lines" value={dashboard.totalLines} />
            <Card label="Stations" value={dashboard.totalStations} />
            <Card label="Orders" value={dashboard.totalOrders} />
            <Card label="Active orders" value={dashboard.activeOrders} highlight />
            <Card label="Units" value={dashboard.totalUnits} />
            <Card label="Events" value={dashboard.totalEvents} />
        </div>

        <h3>Units by status</h3>
        <div className="cards">
            <Card label="Total" value={units.total} />
            <Card label="In Progress" value={units.inProgress} />
            <Card label="In Rework" value={units.inRework} highlight />
            <Card label="On Hold" value={units.onHold} />
            <Card label="Passed" value={units.passed} />
            <Card label="Scrapped" value={units.scrapped} />
        </div>

        <h3>Lines</h3>
        <table className="table">
            <thead>
            <tr>
                <th>Code</th>
                <th>Name</th>
                <th>Stations</th>
                <th>Active orders</th>
                <th>Total units</th>
            </tr>
            </thead>
            <tbody>
            {lines.map((l) => (
                <tr key={l.lineId}>
                <td><strong>{l.lineCode}</strong></td>
                <td>{l.lineName}</td>
                <td>{l.totalStations}</td>
                <td>{l.activeOrders}</td>
                <td>{l.totalUnits}</td>
                </tr>
            ))}
            </tbody>
        </table>

        <h3>Events by type</h3>
        {units.eventsByType.length === 0 && <p>No events registered.</p>}
        {units.eventsByType.length > 0 && (
            <table className="table">
            <thead>
                <tr>
                <th>Event type</th>
                <th>Count</th>
                </tr>
            </thead>
            <tbody>
                {units.eventsByType.map((e) => (
                <tr key={e.eventType}>
                    <td>{e.eventType}</td>
                    <td>{e.count}</td>
                </tr>
                ))}
            </tbody>
            </table>
        )}

        <h3>Events by station</h3>
        {units.eventsByStation.length === 0 && <p>No events registered.</p>}
        {units.eventsByStation.length > 0 && (
            <table className="table">
            <thead>
                <tr>
                <th>Station</th>
                <th>Name</th>
                <th>Count</th>
                </tr>
            </thead>
            <tbody>
                {units.eventsByStation.map((s) => (
                <tr key={s.stationId}>
                    <td><strong>{s.stationCode}</strong></td>
                    <td>{s.stationName}</td>
                    <td>{s.count}</td>
                </tr>
                ))}
            </tbody>
            </table>
        )}

        <p className="monitoring-timestamp">
            Last updated: {new Date(dashboard.generatedAt).toLocaleString()}
        </p>
        </section>
    );
}

interface CardProps {
    label: string;
    value: number;
    highlight?: boolean;
}

function Card({ label, value, highlight }: CardProps) {
    return (
        <div className={highlight ? 'card card-highlight' : 'card'}>
        <div className="card-value">{value}</div>
        <div className="card-label">{label}</div>
        </div>
    );
}