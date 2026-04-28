# Movies Database — Alkalmazásfejlesztési technológiái beadandó

Filmnyilvántartó alkalmazás ASP.NET 10 backend + Angular 21 frontend + MongoDB stackkel, konténerizálva, Kubernetes klaszteren telepíthetően.

## Stack
- **Frontend**: Angular 21, TypeScript
- **Backend**: ASP.NET 10, C#
- **Database**: MongoDB 8
- **Container**: Docker
- **Orchestration**: Kubernetes
- **CI**: GitHub Actions

## Container images
- Backend: `ghcr.io/aborosadam/movies-api:latest`
- Frontend: `ghcr.io/aborosadam/movies-frontend:latest`

## Documentation
- [Installation guide](docs/INSTALL.md) — local Kubernetes deployment
- [User guide](docs/USER_GUIDE.md) — how to use the application

## Quick local start (Docker Compose)
```bash
cd deployment/docker
docker compose up -d
```

Open http://localhost:8081 in your browser.