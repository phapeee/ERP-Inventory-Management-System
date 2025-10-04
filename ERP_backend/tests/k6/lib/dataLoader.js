import { SharedArray } from 'k6/data';

// Loads JSON fixture once per test run using SharedArray.
export function loadProductsDataset(path = './data/products.json') {
    const shared = new SharedArray('productsDataset', () => {
        const text = open(path);
        return JSON.parse(text);
    });
    return shared;
}

// Utility to pick a deterministic item per iteration index.
export function pickProduct(sharedArray, index) {
    if (!sharedArray?.length) {
        throw new Error('Shared products dataset is empty');
    }
    return sharedArray[index % sharedArray.length];
}
