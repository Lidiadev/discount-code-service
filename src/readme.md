dotnet ef migrations add InitialCreate
dotnet ef database update
docker compose build --no-cache
docker compose up --build


psql discountdb -c "SELECT table_name FROM Information_Schema.tables where table_schema='public'"


psql -U postgres

