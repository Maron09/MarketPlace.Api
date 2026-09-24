<!-- dotnet watch run --project src/Marketplace.Api -->
<!-- cat src/Marketplace.Infrastructure.Shared/Persistence/Migrations/*InitialCartSchema.cs -->

<!-- dotnet ef database update \
  --project src/Marketplace.Infrastructure.Shared \
  --startup-project src/Marketplace.Api -->

  <!-- docker exec -it marketplace-postgres psql -U marketplace -d marketplace -c "\dt cart.*"
docker exec -it marketplace-postgres psql -U marketplace -d marketplace -c "\d cart.\"CartItems\"" -->