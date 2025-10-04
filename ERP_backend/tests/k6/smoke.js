import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
  vus: 5,
  duration: '30s',
  thresholds: { http_req_duration: ['p(95)<500'] },
};

const baseUrl = __ENV.API_BASE_URL ?? 'http://localhost:5000';

export default () => {
  const res = http.get(`${baseUrl}/health`);
  check(res, { 'status is 200': (r) => r.status === 200 });
  sleep(1);
};
