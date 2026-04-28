# Telepítési útmutató

Ez az útmutató bemutatja, hogyan lehet a Movies Database alkalmazást helyi Kubernetes klaszterre telepíteni.

## Előfeltételek

A telepítéshez a következő eszközökre van szükség (macOS-en):

- **Docker Desktop** — beépített Kubernetes támogatással
- **kubectl** — `brew install kubectl`
- **Helm** — `brew install helm`
- **Git** — kódbázis klónozásához

A Docker Desktop beállítása:
1. Nyisd meg a Docker Desktopot
2. Settings → Kubernetes → "Enable Kubernetes" pipa → Apply & Restart
3. Várd meg, amíg a státusz zöldre vált (kb. 2-5 perc)

## Telepítési lépések

### 1. Repó klónozása

```bash
git clone https://github.com/aborosadam/hv6ex7_gde-alkfej-beadando.git
cd hv6ex7_gde-alkfej-beadando
```

### 2. Ingress controller telepítése

A klaszteren kell egy Ingress controller, ami a kívülről jövő forgalmat a megfelelő szolgáltatáshoz irányítja.

```bash
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.11.3/deploy/static/provider/cloud/deploy.yaml
```

Várd meg, amíg az Ingress controller pod fut:

```bash
kubectl wait --namespace ingress-nginx \\
  --for=condition=ready pod \\
  --selector=app.kubernetes.io/component=controller \\
  --timeout=120s
```

### 3. Namespace létrehozása

```bash
kubectl apply -f deployment/kubernetes/01-namespace.yaml
```

### 4. MongoDB telepítése

A MongoDB telepíthető Helm-mel (Bitnami chart) vagy a mellékelt raw YAML-lel. A követelményspecifikáció Helm chart-ot kér, viszont a Bitnami chart image-e (`bitnami/mongodb:8.0`) a Bitnami Secure Images átalakítása óta nem érhető el a Docker Hub-on.

Ezért a hivatalos `mongo:8` image-zel deploy-oljuk, raw YAML-lel:

```bash
kubectl apply -f deployment/kubernetes/00-mongodb.yaml
```

A Helm chart values fájl referenciaként megtalálható: `deployment/kubernetes/helm/mongodb-values.yaml`. Amikor a Bitnami chart újra elérhető lesz, ezzel a paranccsal lehet telepíteni:

```bash
helm repo add bitnami https://charts.bitnami.com/bitnami
helm install mongodb bitnami/mongodb -n movies -f deployment/kubernetes/helm/mongodb-values.yaml
```

### 5. Backend és frontend telepítése

```bash
kubectl apply -f deployment/kubernetes/02-backend-deployment.yaml
kubectl apply -f deployment/kubernetes/03-frontend-deployment.yaml
kubectl apply -f deployment/kubernetes/04-ingress.yaml
```

### 6. Pod-ok ellenőrzése

```bash
kubectl get pods -n movies
```

Mind a 3 pod legyen `Running 1/1`:

```
NAME                       READY   STATUS    RESTARTS   AGE
backend-...                1/1     Running   0          1m
frontend-...               1/1     Running   0          1m
mongodb-...                1/1     Running   0          1m
```

### 7. Hostname beállítása

A `movies.localhost` hostnevet rá kell mutatnia a localhostra. Add hozzá a `/etc/hosts` fájlhoz:

```bash
echo "127.0.0.1 movies.localhost" | sudo tee -a /etc/hosts
```

### 8. Hozzáférés a Mac-en

Docker Desktop K8s-en a LoadBalancer szolgáltatás nem mindig kap publikus IP-t. Ezért port-forwardot használunk:

```bash
kubectl port-forward -n ingress-nginx svc/ingress-nginx-controller 8088:80
```

Most a böngészőben nyisd meg:

```
http://movies.localhost:8088
```

## A rendszer leállítása

```bash
kubectl delete -f deployment/kubernetes/04-ingress.yaml
kubectl delete -f deployment/kubernetes/03-frontend-deployment.yaml
kubectl delete -f deployment/kubernetes/02-backend-deployment.yaml
kubectl delete -f deployment/kubernetes/00-mongodb.yaml
kubectl delete -f deployment/kubernetes/01-namespace.yaml
```

## Alternatíva: Docker Compose lokál környezethez

Fejlesztés vagy gyors tesztelés esetén Docker Compose-szal is elindítható az alkalmazás:

```bash
cd deployment/docker
docker compose up -d
```

Hozzáférés:
- Frontend: http://localhost:8081
- Backend Swagger: http://localhost:8080/swagger

Leállítás: `docker compose down`

## Hibakeresés

### Pod nem indul el

```bash
kubectl describe pod -n movies <pod-név>
kubectl logs -n movies <pod-név>
```

### Image pull hibák

A friss image-ek a `ghcr.io/aborosadam/movies-api` és `ghcr.io/aborosadam/movies-frontend` registry-kben vannak, publikus láthatósággal. Ha mégis pull problémád lenne:

```bash
docker pull ghcr.io/aborosadam/movies-api:latest
docker pull ghcr.io/aborosadam/movies-frontend:latest
```

### Frontend "0 Unknown Error"

Ez többnyire azt jelenti, hogy a backend pod nem készült el még. Várj 1 percet, majd próbáld újra. Ha tartós:

```bash
kubectl rollout restart deployment/backend -n movies
```
## Telepítés ArgoCD-vel (CD pipeline)

A repó tartalmaz ArgoCD Application CR-eket, amelyek lehetővé teszik az automatikus, GitOps-stílusú deploymentet.

### Előfeltételek

- Telepített ArgoCD a klaszteren

### ArgoCD telepítése

```bash
kubectl create namespace argocd
kubectl apply -n argocd -f https://raw.githubusercontent.com/argoproj/argo-cd/stable/manifests/install.yaml
```

### ArgoCD UI elérése

```bash
kubectl port-forward -n argocd svc/argocd-server 8443:443
```

Browser: https://localhost:8443

Admin jelszó kinyerése:
```bash
kubectl -n argocd get secret argocd-initial-admin-secret -o jsonpath="{.data.password}" | base64 -d
```

### Application CR-ek telepítése

```bash
kubectl apply -f deployment/argocd/apps.yaml
```

Ez után az ArgoCD automatikusan szinkronizálja a teljes alkalmazást. Bármely Git push az `main` branchre, ami a `deployment/kubernetes/` mappát érinti, automatikusan deploy-olódik a klaszterre.

### Komponensek

Az `apps.yaml` 6 Application-t definiál:
- `movies-namespace` — A movies namespace létrehozása
- `movies-mongodb` — MongoDB deployment + PVC + Service
- `movies-backend` — ASP.NET backend
- `movies-frontend` — Angular frontend
- `movies-mcp-server` — MCP server microservice (AI tool integration)
- `movies-ingress` — Nginx Ingress route-ok