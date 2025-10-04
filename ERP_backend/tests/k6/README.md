# k6 Metrics & Outputs

## Summary Export

- Run `./ERP_backend/tests/k6/run_steady_state.sh` to execute the steady-state scenario inside the k6 Docker image and save a timestamped JSON summary under `reports/`.
- Pass `API_BASE_URL` before invoking the script when targeting alternative environments.
- For ad-hoc runs: `docker run --rm -v "$PWD/ERP_backend/tests/k6:/src" -w /src grafana/k6 run --summary-export reports/<name>.json scenarios/<scenario>.js`.

## Time-Series Outputs

- Stream metrics to InfluxDB: `grafana/k6 run --out influxdb=http://localhost:8086/k6 ...` (ensure the Influx instance is reachable and credentials are set via env vars).
- Prometheus remote write: `--out experimental-prometheus-rw=http://prometheus:9090/api/v1/write` with appropriate TLS/auth options.
- Datadog: `--out datadog` with `K6_DATADOG_*` env vars exported (API key, site).

## Alerts & Thresholds

- Threshold failures already break the k6 run (non-zero exit). Wire the command into CI so builds fail automatically.
- Combine with Slack/PagerDuty webhooks via your CI pipeline (e.g., GitHub Actions step that posts when the k6 job fails).
- Keep custom metrics (`product_list_duration`, `product_list_has_items`, etc.) in `lib/metrics.js` so domain KPIs are tracked consistently across scenarios.
- Example GitHub Actions step:

  ```yaml
  - name: Run k6 steady-state
    run: |
      ./ERP_backend/tests/k6/run_steady_state.sh
  - name: Notify Slack on failure
    if: failure()
    uses: slackapi/slack-github-action@v1
    with:
      payload: '{"text":"k6 load test failed for ${{ github.sha }}"}'
    env:
      SLACK_WEBHOOK_URL: ${{ secrets.SLACK_WEBHOOK_URL }}
  ```

## TODOs

- Integrate summary uploads into artifact storage (e.g., GitHub Actions `upload-artifact`).
- Configure per-environment Influx/Prometheus credentials via secrets manager instead of local env exports.
