import { Trend, Rate, Counter } from 'k6/metrics';

// Trend tracking response times for core API workflows.
export const productListDuration = new Trend('product_list_duration', true);

// Rate capturing business-level success ratio (e.g., products returned with data).
export const productListHasItems = new Rate('product_list_has_items');

// Counter for total items retrieved, useful for throughput KPIs.
export const productListItemsCount = new Counter('product_list_items_count');

// NOTE: Extend this module with additional KPIs as new scenarios are added.
