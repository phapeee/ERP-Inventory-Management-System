import { group, sleep } from 'k6';
import { apiPost } from '../lib/httpClient.js';
import { loadProductsDataset, pickProduct } from '../lib/dataLoader.js';

const DEFAULT_ITERATIONS = 50;
const DEFAULT_VUS = 5;
const DEFAULT_BATCH_SIZE = 25;
const DEFAULT_PAUSE = 1;

export const options = {
    scenarios: {
        batch_ingest: {
            executor: __ENV.EXECUTOR ?? 'shared-iterations',
            vus: Number(__ENV.VUS ?? DEFAULT_VUS),
            iterations: Number(__ENV.ITERATIONS ?? DEFAULT_ITERATIONS),
            startTime: __ENV.START_TIME ?? '0s',
        },
    },
    thresholds: {
        'http_req_duration{scenario:batch_ingest}': ['p(95)<800'],
        'http_req_failed{scenario:batch_ingest}': ['rate<0.02'],
    },
};

const baseUrl = __ENV.API_BASE_URL ?? 'http://localhost:5000';
const batchSize = Number(__ENV.BATCH_SIZE ?? DEFAULT_BATCH_SIZE);
const pauseSeconds = Number(__ENV.BATCH_PAUSE ?? DEFAULT_PAUSE);
const productsDataset = loadProductsDataset();

export default function () {
    group('Bulk product upload', () => {
        const payload = Array.from({ length: batchSize }, (_, index) => {
            const seed = pickProduct(productsDataset, (__ITER * batchSize) + index);
            return {
                name: `${seed.name} Batch ${__VU}-${__ITER}-${index}`,
                price: seed.price,
            };
        });

        apiPost('/api/products/batch', payload, {
            baseUrl,
            expect: 202,
            checks: [
                ['accepted batch', (res) => res.status === 202],
            ],
        });
    });

    if (pauseSeconds > 0) {
        sleep(pauseSeconds);
    }
}
