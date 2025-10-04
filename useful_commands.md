## PostgreSQL docker

```bash
docker run --name pg \
  -e POSTGRES_USER=app \
  -e POSTGRES_PASSWORD=secret \
  -e POSTGRES_DB=erp \
  -p 5432:5432 \
  -v pgdata:/var/lib/postgresql/data \
  -d postgres:17
```

## API controller with CRUD actions using EF Core (not working on .NET 9)

```bash
dotnet aspnet-codegenerator controller \
  -name ProductsController \
  -api \
  -m Product \
  -dc AppDb \
  -outDir Controllers
```

## Create simple API controller

```bash
dotnet new apicontroller \
  -n <controller_name> \
  -o Controllers \
  --namespace ERP_backend.Controllers
```
  
## Run Plantuml with jetty or tomcat container

```bash
docker run -d -p 8080:8080 plantuml/plantuml-server:jetty
docker run -d -p 8080:8080 plantuml/plantuml-server:tomcat
```

## Run Newman test, ensure load .env.development

```bash
newman run "$POSTMAN_TEST_COLLECTION_URL$?apikey=$POSTMAN_API_KEY"
```

## Run SonarQube (key in .evn.development)

```bash
docker run -d --name sonarqube   -p 9000:9000   -e SONAR_ES_BOOTSTRAP_CHECKS_DISABLE=true   sonarqube:latest

# .NET
dotnet sonarscanner begin /k:"ERP-IMS" /d:sonar.host.url="http://localhost:9000" /d:sonar.token=$SONARQUBE_CS_API_KEY
dotnet build
dotnet sonarscanner end /d:sonar.token=$SONARQUBE_CS_API_KEY

# JS/TS
sonar -Dsonar.host.url=http://localhost:9000 -Dsonar.token=$SONARQUBE_TSJS_API_KEY -Dsonar.projectKey=ERP-IMS-Frontend
```

## Start SonarQube with CloudFlare tunnel

```bash
docker compose -f "Docker Compose/sonarqube-cloudflared.compose.yaml" up -d
```

## Snyk scans

```bash
# Run command in backend and frontend folders separately

# Scan open-source packages for vulnerabilities
snyk monitor --all-projects --org=0ef84ead-e24b-4e1a-8775-0ddba821e8b9

# Scan your source code for vulnerabilities
snyk code test --org=0ef84ead-e24b-4e1a-8775-0ddba821e8b9
```

## Trivy

```bash
# Run Trivy docker
docker run aquasec/trivy

# Scan backend
docker run --rm -v "$(pwd)/ERP_backend:/workspace" -v "$HOME/.cache/trivy:/root/.cache/" aquasec/trivy fs /workspace

# Scan frontend
docker run --rm -v "$(pwd)/ERP_frontend:/workspace" -v "$HOME/.cache/trivy:/root/.cache/" aquasec/trivy fs /workspace
```

## Lighthouse

```bash
# more command in package.json
# Run Lighthouse test
CHROME_PATH=/snap/bin/chromium npx lighthouse http://localhost:4200   --preset=desktop   --output html   --output-path ./lighthouse-report.html   --chrome-flags="--headless=new --no-sandbox"

# View report
/snap/bin/chromium ./lighthouse-report.html
```
