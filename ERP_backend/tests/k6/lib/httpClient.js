import http from "k6/http";
import {
  runChecks,
  buildUrl,
  mergeHeaders,
  toJsonBody,
  assertStatus,
} from "./util.js";

const defaultBaseUrl = __ENV.API_BASE_URL ?? "http://localhost:5000";
const defaultHeaders = {
  "Content-Type": "application/json",
};

// TODO: Inject bearer token or session cookie once setup() retrieves auth data.
let sharedAuthHeaders = {};

export function setAuthHeaders(headers = {}) {
  sharedAuthHeaders = headers;
}

export function clearAuthHeaders() {
  sharedAuthHeaders = {};
}

/**
 * Centralised request helper so scenarios share auth, headers, and checks.
 * Options allow overrides per call without duplicating boilerplate in tests.
 */
export function apiRequest(method, path, options = {}) {
  const {
    baseUrl = defaultBaseUrl,
    body,
    params,
    headers = {},
    tags,
    expect = 200,
    checks = [],
    afterResponse,
  } = options;

  const mergedHeaders = mergeHeaders(defaultHeaders, sharedAuthHeaders, headers);
  const payload = body === undefined ? null : toJsonBody(body);
  const response = http.request(method, buildUrl(baseUrl, path), payload, {
    params,
    headers: mergedHeaders,
    tags,
  });

  const checkEntries = Array.isArray(checks) ? [...checks] : [checks];
  if (expect !== false) {
    checkEntries.unshift(assertStatus(expect));
  }
  runChecks(response, checkEntries);

  if (typeof afterResponse === 'function') {
    afterResponse(response);
  }

  return response;
}

export function apiGet(path, options) {
  return apiRequest("GET", path, options);
}

export function apiPost(path, body, options = {}) {
  return apiRequest("POST", path, { ...options, body });
}

export function apiPut(path, body, options = {}) {
  return apiRequest("PUT", path, { ...options, body });
}

export function apiPatch(path, body, options = {}) {
  return apiRequest("PATCH", path, { ...options, body });
}

export function apiDel(path, options) {
  return apiRequest("DELETE", path, options);
}
