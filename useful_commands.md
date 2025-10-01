## PostgreSQL docker

docker run --name pg \
  -e POSTGRES_USER=app \
  -e POSTGRES_PASSWORD=secret \
  -e POSTGRES_DB=erp \
  -p 5432:5432 \
  -v pgdata:/var/lib/postgresql/data \
  -d postgres:16

## API controller with CRUD actions using EF Core (not working on .NET 9)

dotnet aspnet-codegenerator controller \
  -name ProductsController \
  -api \
  -m Product \
  -dc AppDb \
  -outDir Controllers

## Create simple API controller

dotnet new apicontroller \
  -n <controller_name> \
  -o Controllers \
  --namespace ERP_backend.Controllers
  
## Run Plantuml with jetty or tomcat container

docker run -d -p 8080:8080 plantuml/plantuml-server:jetty
docker run -d -p 8080:8080 plantuml/plantuml-server:tomcat

## Run Newman test, ensure load .env.development

newman run "$POSTMAN_TEST_COLLECTION_URL$?apikey=$POSTMAN_API_KEY"
