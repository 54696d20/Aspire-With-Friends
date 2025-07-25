# build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the entire solution context
COPY . .

# Publish the app
RUN dotnet publish AspireApp.WebWasm/AspireApp.WebWasm.csproj -c Release -o /app/publish

# runtime stage
FROM nginx:alpine AS final
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf
