import { group, sleep } from 'k6';
import { apiGet } from '../lib/httpClient.js';
import {
    productListDuration,
    productListHasItems,
    productListItemsCount,
} from '../lib/metrics.js';

const DEFAULT_RATE = 20;
const DEFAULT_PRE_ALLOCATED = 10;
const DEFAULT_MAX_VUS = 50;
const DEFAULT_DURATION = '5m';
const DEFAULT_THINK_TIME = 0.1;

export const options = {
    scenarios: {
        steady_state: {
            executor: 'constant-arrival-rate',
            rate: Number(__ENV.REQUEST_RATE ?? DEFAULT_RATE),
            timeUnit: '1s',
            duration: __ENV.SCENARIO_DURATION ?? DEFAULT_DURATION,
            preAllocatedVUs: Number(__ENV.PRE_ALLOCATED_VUS ?? DEFAULT_PRE_ALLOCATED),
            maxVUs: Number(__ENV.MAX_VUS ?? DEFAULT_MAX_VUS),
        },
    },
    thresholds: {
        'http_req_duration{scenario:steady_state}': ['p(95)<500'],
        'http_req_failed{scenario:steady_state}': ['rate<0.01'],
        'product_list_duration{scenario:steady_state}': ['p(95)<450'],
        'product_list_has_items{scenario:steady_state}': ['rate>0.95'],
        'product_list_items_count{scenario:steady_state}': ['count>0'],
    },
};

const baseUrl = __ENV.API_BASE_URL ?? 'http://localhost:5000';
const thinkTime = Number(__ENV.THINK_TIME ?? DEFAULT_THINK_TIME);

export default function () {
    group('Products steady-state read', () => {
        apiGet('/api/products', {
            baseUrl,
            checks: [
                ['json array', (res) => {
                    try {
                        const data = res.json();
                        const isArray = Array.isArray(data);
                        if (isArray) {
                            productListItemsCount.add(data.length, { scenario: 'steady_state' });
                            productListHasItems.add(data.length > 0, { scenario: 'steady_state' });
                        } else {
                            productListHasItems.add(false, { scenario: 'steady_state' });
                        }
                        return isArray;
                    } catch (err) {
                        productListHasItems.add(false, { scenario: 'steady_state' });
                        return false;
                    }
                }],
            ],
            afterResponse: (res) => {
                productListDuration.add(res.timings.waiting, { scenario: 'steady_state' });
            },
        });
    });

    if (thinkTime > 0) {
        sleep(thinkTime);
    }
}
