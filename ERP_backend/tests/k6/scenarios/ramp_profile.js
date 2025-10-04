import { group, sleep } from 'k6';
import { apiGet } from '../lib/httpClient.js';

const baseUrl = __ENV.API_BASE_URL ?? 'http://localhost:5000';
const thinkTime = Number(__ENV.THINK_TIME ?? 0.1);

const defaultRampStages = [
    { duration: '2m', target: 10 },
    { duration: '4m', target: 40 },
    { duration: '2m', target: 80 },
    { duration: '3m', target: 20 },
    { duration: '2m', target: 0 },
];

const rampStages = JSON.parse(__ENV.RAMP_STAGES ?? JSON.stringify(defaultRampStages));

export const options = {
    scenarios: {
        ramp_profile: {
            executor: 'ramping-arrival-rate',
            startRate: Number(__ENV.START_RATE ?? 5),
            timeUnit: '1s',
            preAllocatedVUs: Number(__ENV.PRE_ALLOCATED_VUS ?? 20),
            maxVUs: Number(__ENV.MAX_VUS ?? 150),
            stages: rampStages.map((stage) => ({
                duration: stage.duration,
                target: stage.target,
            })),
        },
    },
    thresholds: {
        'http_req_duration{scenario:ramp_profile}': ['p(95)<750'],
        'http_req_failed{scenario:ramp_profile}': ['rate<0.02'],
    },
};

export default function () {
    group('Products ramp read', () => {
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

    if (thinkTime > 0) {
        sleep(thinkTime);
    }
}
