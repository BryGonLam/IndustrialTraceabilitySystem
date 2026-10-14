export interface Product {
    id: number;
    code: string;
    name: string;
    description: string | null;
    isActive: boolean;
    createdAt: string;
}

export interface ProductionLine {
    id: number;
    code: string;
    name: string;
    isActive: boolean;
    createdAt: string;
}

export interface Station {
    id: number;
    productionLineId: number;
    code: string;
    name: string;
    isActive: boolean;
    createdAt: string;
}

export interface ProductionOrderSummary {
    id: number;
    orderNumber: string;
    productId: number;
    productCode: string;
    productName: string;
    productionLineId: number;
    productionLineCode: string;
    productionLineName: string;
    plannedQuantity: number;
    status: number;
    createdAt: string;
    totalUnits: number;
}

export interface UnitSummary {
    id: number;
    serialNumber: string;
    productionOrderId: number;
    orderNumber: string;
    status: number;
    createdAt: string;
    updatedAt: string | null;
}

export interface UnitHistoryEvent {
    id: number;
    stationId: number;
    stationCode: string;
    stationName: string;
    eventType: number;
    result: number | null;
    notes: string | null;
    occurredAt: string;
}

export interface UnitHistory {
    unitId: number;
    serialNumber: string;
    currentStatus: number;
    events: UnitHistoryEvent[];
}

// Labels temporales mientras los enums se serializan como numeros.
// Cuando el backend use JsonStringEnumConverter, se pueden eliminar.
export const UnitStatusLabels: Record<number, string> = {
    0: 'Created',
    1: 'In Progress',
    2: 'On Hold',
    3: 'In Rework',
    4: 'Passed',
    5: 'Scrapped',
};

export const ProductionEventTypeLabels: Record<number, string> = {
    0: 'Start',
    1: 'Stop',
    2: 'Pass',
    3: 'Fail',
    4: 'Rework',
    5: 'Scrap',
    6: 'Hold',
};

export const ProductionEventResultLabels: Record<number, string> = {
    0: 'Pass',
    1: 'Fail',
    2: 'Complete',
    3: 'Released',
};

export const ProductionOrderStatusLabels: Record<number, string> = {
    0: 'Planned',
    1: 'In Progress',
    2: 'Closed',
    3: 'Cancelled',
};

export interface ProductionOrderDetail {
    id: number;
    orderNumber: string;
    productId: number;
    productCode: string;
    productName: string;
    productionLineId: number;
    productionLineCode: string;
    productionLineName: string;
    plannedQuantity: number;
    status: number;
    createdAt: string;
    updatedAt: string | null;
    totalUnits: number;
    units: {
        id: number;
        serialNumber: string;
        status: number;
        createdAt: string;
    }[];
}

export interface CreateProductionEventRequest {
    unitId: number;
    stationId: number;
    userId?: number;
    eventType: number;
    result?: number | null;
    notes?: string | null;
    occurredAt?: string | null;
}

export interface ProductionEvent {
    id: number;
    unitId: number;
    serialNumber: string;
    stationId: number;
    stationCode: string;
    stationName: string;
    userId: number | null;
    username: string | null;
    eventType: number;
    result: number | null;
    notes: string | null;
    occurredAt: string;
    createdAt: string;
}

export interface DashboardSummary {
    totalProducts: number;
    totalLines: number;
    totalStations: number;
    totalOrders: number;
    activeOrders: number;
    totalUnits: number;
    totalEvents: number;
    generatedAt: string;
}

export interface LineStatus {
    lineId: number;
    lineCode: string;
    lineName: string;
    totalStations: number;
    activeOrders: number;
    totalUnits: number;
}

export interface UnitsSummary {
    total: number;
    created: number;
    inProgress: number;
    onHold: number;
    inRework: number;
    passed: number;
    scrapped: number;
    eventsByType: { eventType: string; count: number }[];
    eventsByStation: {
        stationId: number;
        stationCode: string;
        stationName: string;
        count: number;
    }[];
}

export interface LoginRequest {
    username: string;
    password: string;
}

export interface LoginResponse {
    token: string;
    expiresAt: string;
    user: UserInfo;
}

export interface UserInfo {
    id: number;
    username: string;
    displayName: string;
    roles: string[];
}