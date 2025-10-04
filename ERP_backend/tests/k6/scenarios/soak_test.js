import { group, sleep } from 'k6';
import { apiGet } from '../lib/httpClient.js';

const DEFAULT_RATE = 10;
const DEFAULT_PRE_ALLOCATED = 25;
const DEFAULT_MAX_VUS = 150;
const DEFAULT_DURATION = '1h';
const DEFAULT_THINK_TIME = 0.5;

export const options = {
    scenarios: {
        soak: {
            executor: 'constant-arrival-rate',
            rate: Number(__ENV.REQUEST_RATE ?? DEFAULT_RATE),
            timeUnit: '1s',
            duration: __ENV.SCENARIO_DURATION ?? DEFAULT_DURATION,
            preAllocatedVUs: Number(__ENV.PRE_ALLOCATED_VUS ?? DEFAULT_PRE_ALLOCATED),
            maxVUs: Number(__ENV.MAX_VUS ?? DEFAULT_MAX_VUS),
        },
    },
    thresholds: {
        'http_req_duration{scenario:soak}': ['p(95)<600', 'avg<400'],
        'http_req_failed{scenario:soak}': ['rate<0.01'],
    },
    summaryTrendStats: ['avg', 'p(95)', 'p(99)', 'min', 'max'],
};

const baseUrl = __ENV.API_BASE_URL ?? 'http://localhost:5000';
const thinkTime = Number(__ENV.THINK_TIME ?? DEFAULT_THINK_TIME);

export default function () {
    group('Health probe', () => {
        apiGet('/health', { baseUrl });
    });

    group('Products catalogue read', () => {
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
