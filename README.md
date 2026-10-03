<!-- dotnet watch run --project src/Marketplace.Api -->
<!-- cat src/Marketplace.Infrastructure.Shared/Persistence/Migrations/*InitialCartSchema.cs -->

<!-- dotnet ef database update \
  --project src/Marketplace.Infrastructure.Shared \
  --startup-project src/Marketplace.Api -->

  <!-- docker exec -it marketplace-postgres psql -U marketplace -d marketplace -c "\dt cart.*"
docker exec -it marketplace-postgres psql -U marketplace -d marketplace -c "\d cart.\"CartItems\"" -->
<!-- 
dotnet add src/Marketplace.Modules.Inventory reference src/Marketplace.Modules.Catalog -->

<!-- dotnet add src/Marketplace.Modules.Orders package Microsoft.EntityFrameworkCore -->

<!-- dotnet ef migrations add InitialOrdersSchema \
  --project src/Marketplace.Infrastructure.Shared \
  --startup-project src/Marketplace.Api \
  --output-dir Persistence/Migrations -->

  <!-- dotnet ef migrations list --project src/Marketplace.Infrastructure.Shared --startup-project src/Marketplace.Api -->
  
  <!-- docker exec -it marketplace-postgres psql -U marketplace -d marketplace -c '\dt orders.*' -->

  <!-- docker exec -it marketplace-postgres psql -U marketplace -d marketplace -->