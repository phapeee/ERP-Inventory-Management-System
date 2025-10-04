# Synthetic Datasets

This directory contains example fixtures used by k6 scenarios.

- `products.json` – seeded catalogue entries used in smoke/load tests.
- `purchase_orders.json` – sample payloads representing bulk purchase orders.

Guidelines:

- Version-control only synthetic, non-sensitive data.
- Mask or externalize credentials, API keys, or customer data via environment variables or secret stores (e.g., Azure Key Vault).
- For large fixtures, consider splitting by domain (e.g., `inventory/`, `orders/`) to keep diffs manageable.
