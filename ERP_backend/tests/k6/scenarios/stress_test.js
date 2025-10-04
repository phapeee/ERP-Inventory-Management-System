import { group, sleep } from 'k6';
import { apiGet } from '../lib/httpClient.js';

const baseUrl = __ENV.API_BASE_URL ?? 'http://localhost:5000';
const thinkTime = Number(__ENV.THINK_TIME ?? 0.05);

const defaultStages = [
    { duration: '2m', target: 50 },
    { duration: '2m', target: 150 },
    { duration: '4m', target: 300 },
    { duration: '3m', target: 450 },
    { duration: '2m', target: 50 },
    { duration: '1m', target: 0 },
];

const stressStages = JSON.parse(__ENV.STRESS_STAGES ?? JSON.stringify(defaultStages));

export const options = {
    scenarios: {
        stress: {
            executor: 'ramping-arrival-rate',
            startRate: Number(__ENV.START_RATE ?? 20),
            timeUnit: '1s',
            preAllocatedVUs: Number(__ENV.PRE_ALLOCATED_VUS ?? 80),
            maxVUs: Number(__ENV.MAX_VUS ?? 500),
            stages: stressStages.map((stage) => ({
                duration: stage.duration,
                target: stage.target,
            })),
        },
    },
    thresholds: {
        'http_req_duration{scenario:stress}': ['p(95)<1200'],
        'http_req_failed{scenario:stress}': ['rate<0.10'],
    },
    summaryTrendStats: ['avg', 'p(90)', 'p(95)', 'p(99)', 'min', 'max'],
};

export default function () {
    group('Inventory catalogue under stress', () => {
        apiGet('/api/products', {
            baseUrl,
            checks: [
                ['json array', (res) => {
                    try {
                        return Array.isArray(res.json());
                    } catch (err) {
                        return false;
                    }
                }],
            ],
        });
    });

    group('Health signal', () => {
        apiGet('/health', {
            baseUrl,
            expect: 200,
        });
    });

    if (thinkTime > 0) {
        sleep(thinkTime);
    }
}
