SHELL := /bin/sh
.DEFAULT_GOAL := help

COMPOSE ?= docker compose

APP_SERVICE      ?= dvsregister
DB_SERVICE       ?= db
LOCALSTACK_SERVICE ?= localstack

POSTGRES_DB       ?= postgres
POSTGRES_USER     ?= postgres
POSTGRES_PASSWORD ?= postgres

AWS_REGION          ?= eu-west-2
LOCALSTACK_ENDPOINT ?= http://localhost:4566
S3_BUCKET           ?= s3-dvs-dev20240529103145426300000001

export DOCKER_BUILDKIT=1
export COMPOSE_DOCKER_CLI_BUILD=1

help: ## List available commands
	@echo "Available commands:"
	@grep --no-filename -E '^[a-zA-Z0-9_-]+:.*?## .*' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-30s\033[0m %s\n", $$1, $$2}'

.PHONY: help

check-tools: ## Check required local tooling is installed
	@command -v docker >/dev/null 2>&1 || { echo "Error: docker is not installed."; exit 1; }
	@docker compose version >/dev/null 2>&1 || { echo "Error: docker compose is not available."; exit 1; }
	@command -v dotnet >/dev/null 2>&1 || { echo "Error: dotnet SDK is not installed."; exit 1; }

.PHONY: check-tools

restore: check-tools ## Restore .NET tools and packages
	@dotnet tool restore || true
	@dotnet restore

.PHONY: restore

build: restore ## Build the solution
	@dotnet build

.PHONY: build

test: TEST_OPTIONS ?=
test: restore ## Run tests
	@dotnet test $(TEST_OPTIONS)

.PHONY: test

build-docker: check-tools ## Build Docker images
	@$(COMPOSE) build

.PHONY: build-docker

up: check-tools ## Start app, database and LocalStack containers
	@$(COMPOSE) up -d $(DB_SERVICE) $(LOCALSTACK_SERVICE) $(APP_SERVICE)
	@$(COMPOSE) ps

.PHONY: up

down: check-tools ## Stop and remove containers
	@$(COMPOSE) down

.PHONY: down

down-volumes: check-tools ## Stop containers and remove volumes
	@$(COMPOSE) down --volumes --remove-orphans

.PHONY: down-volumes

stop: check-tools ## Stop containers without removing them
	@$(COMPOSE) stop

.PHONY: stop

restart: down up ## Restart local environment

.PHONY: restart

ps: check-tools ## Show Docker container status
	@$(COMPOSE) ps

.PHONY: ps

logs: check-tools ## Follow logs for all services
	@$(COMPOSE) logs -f

.PHONY: logs

logs-app: check-tools ## Follow app logs
	@$(COMPOSE) logs -f $(APP_SERVICE)

.PHONY: logs-app

db-up: check-tools ## Start the local Postgres container
	@$(COMPOSE) up -d $(DB_SERVICE)
	@$(MAKE) wait-db

.PHONY: db-up

wait-db: check-tools ## Wait for Postgres to accept connections
	@echo "Waiting for Postgres..."
	@attempts=30; \
	while [ $$attempts -gt 0 ]; do \
		if $(COMPOSE) exec -T $(DB_SERVICE) pg_isready -U "$(POSTGRES_USER)" -d "$(POSTGRES_DB)" >/dev/null 2>&1; then \
			echo "Postgres is ready."; \
			exit 0; \
		fi; \
		attempts=$$((attempts - 1)); \
		sleep 2; \
	done; \
	echo "Error: Postgres did not become ready."; \
	$(COMPOSE) ps $(DB_SERVICE); \
	exit 1

.PHONY: wait-db

db: db-up ## Start DB and run Entity Framework migrations
	@echo "Running EF Core migrations..."
	@dotnet ef database update --project DVSRegister.Data --startup-project DVSRegister

.PHONY: db

db-add-migration: MIGRATION_NAME ?=
db-add-migration: check-tools ## Add an EF Core migration. Usage: make db-add-migration MIGRATION_NAME=NameOfMigration
	@test -n "$(MIGRATION_NAME)" || { echo "Error: MIGRATION_NAME is required. Usage: make db-add-migration MIGRATION_NAME=NameOfMigration"; exit 1; }
	@dotnet ef migrations add "$(MIGRATION_NAME)" --project DVSRegister.Data --startup-project DVSRegister

.PHONY: db-add-migration

db-dump: db-up ## Dump local Postgres database data
	@$(COMPOSE) exec -T $(DB_SERVICE) sh -c 'PGPASSWORD="$$POSTGRES_PASSWORD" pg_dump -a -U "$$POSTGRES_USER" -d "$$POSTGRES_DB"'

.PHONY: db-dump

db-restore: FILE ?=
db-restore: db-up ## Restore SQL backup into local Postgres. Usage: make db-restore FILE=backup.sql
	@test -n "$(FILE)" || { echo "Error: FILE is required. Usage: make db-restore FILE=backup.sql"; exit 1; }
	@test -f "$(FILE)" || { echo "Error: file not found: $(FILE)"; exit 1; }
	@cat "$(FILE)" | $(COMPOSE) exec -T $(DB_SERVICE) sh -c 'PGPASSWORD="$$POSTGRES_PASSWORD" psql -U "$$POSTGRES_USER" -d "$$POSTGRES_DB"'

.PHONY: db-restore

localstack: check-tools ## Start LocalStack and create required local S3 bucket
	@$(COMPOSE) up -d $(LOCALSTACK_SERVICE)
	@$(MAKE) wait-localstack
	@$(MAKE) localstack-create-bucket

.PHONY: localstack

wait-localstack: check-tools ## Wait for LocalStack to become available
	@echo "Waiting for LocalStack..."
	@attempts=30; \
	while [ $$attempts -gt 0 ]; do \
		if curl -fs "$(LOCALSTACK_ENDPOINT)/_localstack/health" >/dev/null 2>&1; then \
			echo "LocalStack is ready."; \
			exit 0; \
		fi; \
		attempts=$$((attempts - 1)); \
		sleep 2; \
	done; \
	echo "Error: LocalStack did not become ready."; \
	$(COMPOSE) ps $(LOCALSTACK_SERVICE); \
	exit 1

.PHONY: wait-localstack

localstack-create-bucket: check-tools ## Create required S3 bucket in LocalStack
	@echo "Ensuring LocalStack S3 bucket exists: $(S3_BUCKET)"
	@$(COMPOSE) exec -T $(LOCALSTACK_SERVICE) awslocal s3api head-bucket --bucket "$(S3_BUCKET)" >/dev/null 2>&1 || \
	$(COMPOSE) exec -T $(LOCALSTACK_SERVICE) awslocal s3api create-bucket \
		--bucket "$(S3_BUCKET)" \
		--create-bucket-configuration LocationConstraint="$(AWS_REGION)"
	@echo "S3 bucket ready: $(S3_BUCKET)"

.PHONY: localstack-create-bucket

localstack-list-buckets: check-tools ## List LocalStack S3 buckets
	@$(COMPOSE) exec -T $(LOCALSTACK_SERVICE) awslocal s3api list-buckets

.PHONY: localstack-list-buckets

dev: localstack db up ## Start LocalStack, run DB migrations and start the app

.PHONY: dev

clean: check-tools ## Remove containers and dangling Docker resources
	@$(COMPOSE) down --remove-orphans
	@docker image prune -f

.PHONY: clean
