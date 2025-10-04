// Tracks created resource identifiers so teardown can remove leftovers.
// TODO: Replace in-memory registry with durable shared store if multiple VUs require coordination.
const state = globalThis.__k6CleanupState ?? (globalThis.__k6CleanupState = {
  products: new Set(),
});

export function trackProductId(id) {
  if (!id) {
    return;
  }
  state.products.add(id);
}

export function untrackProductId(id) {
  if (!id) {
    return;
  }
  state.products.delete(id);
}

export function drainTrackedProductIds() {
  const ids = Array.from(state.products);
  state.products.clear();
  return ids;
}
