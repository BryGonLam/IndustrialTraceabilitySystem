import { useEffect, useState } from 'react';
import { apiGet, apiPost, ApiError } from '../api/client';
import type {
    UnitSummary,
    Station,
    CreateProductionEventRequest,
    ProductionEvent,
} from '../api/types';
import {
    ProductionEventTypeLabels,
    ProductionEventResultLabels,
} from '../api/types';

const EVENT_TYPES: number[] = [0, 1, 2, 3, 4, 5, 6];
const RESULT_OPTIONS: number[] = [0, 1, 2, 3];

interface SuccessInfo {
    event: ProductionEvent;
}

export function RegisterEventPage() {
    const [units, setUnits] = useState<UnitSummary[]>([]);
    const [stations, setStations] = useState<Station[]>([]);
    const [loading, setLoading] = useState(true);
    const [loadError, setLoadError] = useState<string | null>(null);

    // Form state
    const [unitId, setUnitId] = useState<number | ''>('');
    const [stationId, setStationId] = useState<number | ''>('');
    const [eventType, setEventType] = useState<number | ''>('');
    const [result, setResult] = useState<number | ''>('');
    const [notes, setNotes] = useState('');
    const [occurredAt, setOccurredAt] = useState('');

    const [submitting, setSubmitting] = useState(false);
    const [submitError, setSubmitError] = useState<string | null>(null);
    const [success, setSuccess] = useState<SuccessInfo | null>(null);

  // Cargar units y stations para los dropdowns
    useEffect(() => {
        let mounted = true;
        Promise.all([
        apiGet<UnitSummary[]>('/units'),
        apiGet<Station[]>('/stations'),
        ])
        .then(([unitsData, stationsData]) => {
            if (mounted) {
            setUnits(unitsData);
            setStations(stationsData);
            setLoadError(null);
            }
        })
        .catch((err: unknown) => {
            if (mounted) {
            setLoadError(err instanceof ApiError ? `${err.code}: ${err.message}` : 'Unexpected error');
            }
        })
        .finally(() => {
            if (mounted) setLoading(false);
        });
        return () => { mounted = false; };
    }, []);

    function resetForm() {
        setUnitId('');
        setStationId('');
        setEventType('');
        setResult('');
        setNotes('');
        setOccurredAt('');
    }

    async function handleSubmit(e: React.FormEvent) {
        e.preventDefault();
        setSubmitError(null);
        setSuccess(null);

        if (unitId === '' || stationId === '' || eventType === '') {
        setSubmitError('Unit, station and event type are required.');
        return;
        }

        const body: CreateProductionEventRequest = {
        unitId: Number(unitId),
        stationId: Number(stationId),
        eventType: Number(eventType),
        };

        if (result !== '') {
        body.result = Number(result);
        }
        if (notes.trim() !== '') {
        body.notes = notes.trim();
        }
        if (occurredAt !== '') {
        // El input datetime-local da "YYYY-MM-DDTHH:mm" en hora local.
        // Convertimos a ISO UTC.
        body.occurredAt = new Date(occurredAt).toISOString();
        }

        setSubmitting(true);
        try {
        const created = await apiPost<CreateProductionEventRequest, ProductionEvent>(
            '/production-events',
            body,
        );
        setSuccess({ event: created });
        resetForm();
        } catch (err: unknown) {
        if (err instanceof ApiError) {
            setSubmitError(`${err.code}: ${err.message}`);
        } else {
            setSubmitError('Unexpected error while registering event.');
        }
        } finally {
        setSubmitting(false);
        }
    }

    if (loading) return <p>Loading...</p>;
    if (loadError) return <p className="error">{loadError}</p>;

    return (
        <section>
        <h2>Register Production Event</h2>

        <form className="form" onSubmit={handleSubmit}>
            <div className="form-row">
            <label htmlFor="unitId">Unit *</label>
            <select
                id="unitId"
                value={unitId}
                onChange={(e) => setUnitId(e.target.value === '' ? '' : Number(e.target.value))}
                required
            >
                <option value="">-- Select a unit --</option>
                {units.map((u) => (
                <option key={u.id} value={u.id}>
                    {u.serialNumber} (Order {u.orderNumber})
                </option>
                ))}
            </select>
            </div>

            <div className="form-row">
            <label htmlFor="stationId">Station *</label>
            <select
                id="stationId"
                value={stationId}
                onChange={(e) => setStationId(e.target.value === '' ? '' : Number(e.target.value))}
                required
            >
                <option value="">-- Select a station --</option>
                {stations.map((s) => (
                <option key={s.id} value={s.id}>
                    {s.code} — {s.name}
                </option>
                ))}
            </select>
            </div>

            <div className="form-row">
            <label htmlFor="eventType">Event type *</label>
            <select
                id="eventType"
                value={eventType}
                onChange={(e) => setEventType(e.target.value === '' ? '' : Number(e.target.value))}
                required
            >
                <option value="">-- Select an event type --</option>
                {EVENT_TYPES.map((t) => (
                <option key={t} value={t}>
                    {ProductionEventTypeLabels[t] ?? t}
                </option>
                ))}
            </select>
            </div>

            <div className="form-row">
            <label htmlFor="result">Result (optional)</label>
            <select
                id="result"
                value={result}
                onChange={(e) => setResult(e.target.value === '' ? '' : Number(e.target.value))}
            >
                <option value="">-- None --</option>
                {RESULT_OPTIONS.map((r) => (
                <option key={r} value={r}>
                    {ProductionEventResultLabels[r] ?? r}
                </option>
                ))}
            </select>
            </div>

            <div className="form-row">
            <label htmlFor="occurredAt">Occurred at (optional)</label>
            <input
                id="occurredAt"
                type="datetime-local"
                value={occurredAt}
                onChange={(e) => setOccurredAt(e.target.value)}
            />
            <small className="form-hint">If left empty, the current time will be used.</small>
            </div>

            <div className="form-row">
            <label htmlFor="notes">Notes (optional)</label>
            <textarea
                id="notes"
                rows={3}
                maxLength={500}
                value={notes}
                onChange={(e) => setNotes(e.target.value)}
            />
            </div>

            <div className="form-actions">
            <button type="submit" className="btn-primary" disabled={submitting}>
                {submitting ? 'Submitting...' : 'Register event'}
            </button>
            <button type="button" className="btn-secondary" onClick={resetForm} disabled={submitting}>
                Reset
            </button>
            </div>
        </form>

        {submitError && <p className="error">{submitError}</p>}

        {success && (
            <div className="success-box">
            <p><strong>Event registered successfully.</strong></p>
            <p>
                Id: {success.event.id}<br />
                Unit: {success.event.serialNumber}<br />
                Station: {success.event.stationCode} — {success.event.stationName}<br />
                Type: {ProductionEventTypeLabels[success.event.eventType] ?? success.event.eventType}<br />
                Result: {success.event.result !== null
                ? (ProductionEventResultLabels[success.event.result] ?? success.event.result)
                : '-'}<br />
                Occurred at: {new Date(success.event.occurredAt).toLocaleString()}
            </p>
        </div>
        )}
    </section>
    );
}