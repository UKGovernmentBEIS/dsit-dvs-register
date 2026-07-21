# Cab portal & Register

**Requirements:**
  - [Docker Desktop](https://www.docker.com/products/docker-desktop/) (includes `docker compose`)
  - [.NET 8 SDK](https://learn.microsoft.com/en-us/dotnet/core/install/macos)
  - `dotnet tool install --global dotnet-ef --version 8.*`
  - Node / NPM (for frontend asset compilation)
  - `make`

## Getting Started

### One-command bootstrap (recommended)

```sh
make dev
```

This single command:
1. Starts LocalStack and creates the required S3 bucket.
2. Starts Postgres and runs all EF Core migrations.
3. Builds and starts the application container.

The app is available at **http://localhost:8080** once all services are healthy.

### Available `make` targets

| Command | Description |
|---|---|
| `make up` | Start app, Postgres and LocalStack containers |
| `make down` | Stop and remove all containers |
| `make db` | Start Postgres and run EF Core migrations |
| `make localstack` | Start LocalStack and create the S3 bucket |
| `make dev` | Full local bootstrap (LocalStack → DB migrations → app) |
| `make logs` | Tail logs for all services |
| `make ps` | Show container status |
| `make help` | List all available targets |

Run `make help` to see the full list of targets with descriptions.

### Manual setup (alternative / Windows)

1. Clone the repo.
2. Install dev certs:
   - `dotnet dev-certs https --trust`
3. Add user secrets — ask a team member for the values.
4. Start Postgres:
   - `docker run -d --name postgres-container -e POSTGRES_PASSWORD=postgres -p 5432:5432 postgres:15-alpine`
5. Run DB migrations:
   - `dotnet ef database update --project DVSRegister.Data --startup-project DVSRegister`
6. Start LocalStack and create the S3 bucket:
   - `brew install localstack/tap/localstack-cli`
   - `localstack start -d`
   - `awslocal s3api create-bucket --bucket s3-dvs-dev20240529103145426300000001`
   - http://s3-dvs-dev20240529103145426300000001.s3.localhost.localstack.cloud:4566

> Note: Make sure to update the S3 bucket in user secrets to use path style instead of host style.

## GOV.UK Frontend and Styling
This project relies on the [GOV.UK Frontend NPM package](https://www.npmjs.com/package/govuk-frontend) which contains images, fonts, styling, and JavaScript.

### Updating styles
The project loosely follows and ITCSS folder structure(LINK) and BEM(LINK) for writing CSS
If you're adding new styles to the codebase be sure to run `npm run compile-sass` inside the project folder `DVSAdmin`.

## Contributing
On this project we use [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/#summary). Conventional Commits establish a formal convention for structuring commit messages.

## Development credentials
We store credentials for the dev site inside the UserSecrets of the project. Speak to someone on the team to get these.

## Database
At the root of this repo -
1. For adding migrations: run  `dotnet ef migrations add nameofmigration --project DVSRegister.Data  --startup-project DVSRegister`
2. For updating changes to local dabase: run `dotnet ef database update --project DVSRegister.Data  --startup-project DVSRegister`

## Running cab portal locally
When running the CAB portal locally, you can bypass the login flow (username, password, MFA) by hardcoding values in the BaseController. This is useful for local development and testing.

1. Open BaseController.
2. Replace the following variables with appropriate values:
    // Hardcoded values for local testing
     - protected string UserEmail => "testemail@dsit.com"; // Replace with a valid user email
     - protected int CabId => 1233;                        // Replace with the CAB ID from the Cab table
     - protected string Cab => "TEST";                     // Replace with the CAB name from the Cab table linked to the user email

> Note: Ensure the UserEmail exists in your database and is mapped to the specified CAB.
        The CabId and Cab values must match the entries in the Cab table for the given user.
        This approach is for local development only. Do not use hardcoded credentials in production.

## Register
- The Register application is publicly accessible and does not require any modifications.
- It serves as the root of the solution and will load automatically when the solution is run.



