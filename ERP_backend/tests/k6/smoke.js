import { group, sleep } from 'k6';
import {
  apiGet,
  apiPost,
  apiPut,
  apiDel,
  setAuthHeaders,
  clearAuthHeaders,
} from './lib/httpClient.js';
import { loadProductsDataset, pickProduct } from './lib/dataLoader.js';
import {
  trackProductId,
  untrackProductId,
  drainTrackedProductIds,
} from './lib/cleanupRegistry.js';

export const options = {
    vus: 5,
    duration: '30s',
    thresholds: {
        http_req_duration: ['p(95)<500'],
        http_req_failed: ['rate<0.01'],
    },
};

const baseUrl = __ENV.API_BASE_URL ?? 'http://localhost:5000';
const productsDataset = loadProductsDataset();

export function setup() {
  // TODO: Replace with real authentication request and propagate token to VUs.
  const token = __ENV.API_TOKEN;
  if (token) {
    setAuthHeaders({ Authorization: `Bearer ${token}` });
  }
  return { token };
}

export function teardown() {
  // TODO: Revoke or cleanup auth/session once backend exposes the endpoint.
  const leftovers = drainTrackedProductIds();
  leftovers.forEach((id) => {
    // TODO: Replace with dedicated bulk cleanup endpoint if available.
    apiDel(`/api/products/${id}`, {
      baseUrl,
      expect: false,
    });
  });
  clearAuthHeaders();
}

export default () => {
    group('Health', () => {
        apiGet('/health', {
            baseUrl,
            checks: [
                ['body ok', (res) => res.body === 'ok'],
            ],
        });
    });

    group('Products CRUD', () => {
        const seedProduct = pickProduct(productsDataset, __ITER);
        const productPayload = {
            name: `${seedProduct.name} ${__VU}-${__ITER}`,
            price: seedProduct.price,
        };

        const createRes = apiPost('/api/products', productPayload, {
            baseUrl,
            expect: 201,
            checks: [
                ['has id', (res) => Boolean(res.json()?.id)],
            ],
        });

        const created = createRes.json();
        const productId = created?.id;
        trackProductId(productId);
        if (!productId) {
            return;
        }

        apiPut(`/api/products/${productId}`, {
            ...productPayload,
            name: `${seedProduct.name} Updated ${__VU}-${__ITER}`,
        }, {
            baseUrl,
            checks: [
                ['update success', (res) => res.status === 200],
            ],
        });

        apiGet('/api/products', {
            baseUrl,
            checks: [
                ['list contains product', (res) => {
                    try {
                        return res.json().some((item) => item.id === productId);
                    } catch (err) {
                        return false;
                    }
                }],
            ],
        });

        const deleteRes = apiDel(`/api/products/${productId}`, {
            baseUrl,
            expect: 204,
        });
        if (deleteRes.status === 204) {
            untrackProductId(productId);
        }
    });

    sleep(1);
};
