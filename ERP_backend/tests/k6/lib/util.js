import { check } from 'k6';

// Normalizes and joins base URL with a path fragment.
export function buildUrl(baseUrl, path) {
    if (!path) {
        return baseUrl;
    }
    if (path.startsWith('http')) {
        return path;
    }
    const normalizedBase = baseUrl.endsWith('/') ? baseUrl.slice(0, -1) : baseUrl;
    const normalizedPath = path.startsWith('/') ? path : `/${path}`;
    return `${normalizedBase}${normalizedPath}`;
}

// Merges header objects, ignoring falsy inputs.
export function mergeHeaders(...sets) {
    return sets.filter(Boolean).reduce((acc, current) => ({ ...acc, ...current }), {});
}

// Converts payloads to JSON strings while keeping nulls intact.
export function toJsonBody(payload) {
    if (payload === null || payload === undefined) {
        return null;
    }
    return typeof payload === 'string' ? payload : JSON.stringify(payload);
}

// Returns a labelled status check tuple for reuse in scenarios.
export function assertStatus(expected, label = `status ${expected}`) {
    return [label, (res) => res.status === expected];
}

// Wraps a JSON predicate so it can be passed into runChecks.
export function assertJsonSchema(label, predicate) {
    return [label, (res) => {
        try {
            return predicate(res.json());
        } catch (err) {
            return false;
        }
    }];
}

// Expands an array of check entries and executes them against a response.
export function runChecks(response, entries) {
    const checks = {};
    entries
        .filter(Boolean)
        .forEach((entry, index) => {
            if (Array.isArray(entry)) {
                const [label, fn] = entry;
                checks[label ?? `check_${index}`] = fn;
            } else if (typeof entry === 'function') {
                checks[`check_${index}`] = entry;
            } else if (entry && typeof entry === 'object') {
                Object.assign(checks, entry);
            }
        });
    if (Object.keys(checks).length > 0) {
        check(response, checks);
    }
}

// Adds an Authorization header when a bearer token is available.
export function withBearer(token) {
    return token ? { Authorization: `Bearer ${token}` } : {};
}

// Generates a random SKU-style string for temporary test data.
export function randomSku(prefix = 'sku') {
    return `${prefix}-${Math.random().toString(36).slice(2, 10)}`;
}
