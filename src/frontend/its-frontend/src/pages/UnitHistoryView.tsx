import type { UnitHistory } from '../api/types';
import {
    UnitStatusLabels,
    ProductionEventTypeLabels,
    ProductionEventResultLabels,
} from '../api/types';

function getEventBadgeClass(eventType: number): string {
    switch (eventType) {
        case 2: return 'badge badge-green';   // Pass
        case 3: return 'badge badge-red';     // Fail
        case 4: return 'badge badge-orange';  // Rework
        case 5: return 'badge badge-red';     // Scrap
        case 6: return 'badge badge-yellow';  // Hold
        default: return 'badge badge-gray';   // Start, Stop
    }
}

interface Props {
    history: UnitHistory;
}

export function UnitHistoryView({ history }: Props) {
    return (
        <div className="history-content">
        <div className="history-meta">
            <p>
                <strong>Serial:</strong> {history.serialNumber}
            </p>
            <p>
                <strong>Current status:</strong>{' '}
                {UnitStatusLabels[history.currentStatus] ?? history.currentStatus}
            </p>
            <p>
                <strong>Total events:</strong> {history.events.length}
            </p>
        </div>

        {history.events.length === 0 && (
            <p>No events registered for this unit.</p>
        )}

        {history.events.length > 0 && (
            <ol className="timeline">
            {history.events.map((evt) => (
                <li key={evt.id} className="timeline-item">
                    <div className="timeline-marker">
                        <span className={getEventBadgeClass(evt.eventType)}>
                        {ProductionEventTypeLabels[evt.eventType] ?? evt.eventType}
                        </span>
                    </div>
                    <div className="timeline-body">
                        <div className="timeline-title">
                            <strong>{evt.stationName}</strong>
                            <span className="timeline-code">{evt.stationCode}</span>
                        </div>
                        <div className="timeline-meta">
                            {evt.result !== null && (
                                <span>
                                    Result:{' '}
                                    <em>{ProductionEventResultLabels[evt.result] ?? evt.result}</em>
                                </span>
                            )}
                            <span>{new Date(evt.occurredAt).toLocaleString()}</span>
                        </div>
                        {evt.notes && (
                        <div className="timeline-notes">{evt.notes}</div>
                        )}
                    </div>
                </li>
            ))}
        </ol>
        )}
    </div>
    );
}