- [x] Create Postgresql db
- [ ] Create docker compose file to run solution

## Architecture Thoughts

- A serverless-container platform such as Azure Container Apps or Google Cloud Run
  is a good initial fit for the existing ASP.NET Core API.
- Use managed PostgreSQL rather than running PostgreSQL in the application
  container.
- Store uploaded photos in object storage, not the API filesystem, because
  serverless container storage is ephemeral.
- Use a queue or event bus with serverless workers for thumbnail generation,
  metadata extraction, virus scanning, notifications, and scheduled cleanup.
- Configure database connection pooling for scale-out to avoid exhausting
  PostgreSQL connections.
- Apply EF Core migrations as a controlled deployment step rather than from
  every API instance at startup.
- Store connection strings, storage credentials, and signing keys in the
  deployment platform's secret manager.
- Native AOT may reduce cold starts, but a standard ASP.NET Core container is
  the simpler initial deployment choice.
