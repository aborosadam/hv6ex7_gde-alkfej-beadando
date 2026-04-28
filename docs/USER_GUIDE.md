# Felhasználói útmutató

A Movies Database egy filmnyilvántartó alkalmazás, amelyben filmeket és értékeléseket lehet kezelni.

## Belépés az alkalmazásba

A telepítés után az alkalmazás a következő címen érhető el:

```
http://movies.localhost:8088
```

(Részletes telepítés: lásd `INSTALL.md`)

## Főoldal — Filmlista

A főoldalon az összes film látható, kártya formában. Egy oldalon 5 film jelenik meg, lapozóval lehet a többi között váltani.

Minden filmkártyán látható:
- Cím
- Rendező
- Megjelenési év
- Műfaj
- Rövid leírás
- Delete gomb (törlés)

A kártyára kattintva a film részleteit lehet megnézni.

## Film hozzáadása

A jobb felső sarokban a **+ Add Movie** gombra kattintva előtűnik egy űrlap az alábbi mezőkkel:
- **Title** — a film címe (kötelező)
- **Director** — rendező
- **Year** — megjelenési év
- **Genre** — műfaj
- **Description** — leírás
- **Poster URL** — opcionális, plakát URL-je

A **Save** gombra kattintva mentődik el a film. A Cancel gomb (új gomb a + Add Movie helyén) bezárja az űrlapot.

## Film törlése

Bármelyik filmkártyán a **Delete** gombra kattintva, a megerősítés után a film törlődik. Az értékelései továbbra is megmaradnak az adatbázisban (de a film nélkül elérhetetlenek a felületen).

## Film részletei

Egy filmkártyára kattintva megnyílik a részletes oldal, ahol látható:
- A film összes adata (cím, rendező, év, műfaj, leírás)
- Az **átlagos értékelés** (csillagok és (X review) számláló)
- Az értékelések listája
- Új értékelés form

A bal felső sarokban **← Back to all movies** linkkel lehet visszamenni a főoldalra.

## Értékelés hozzáadása

Az "Add Your Review" szekcióban:
1. **Your name** — saját név
2. **Csillag select** — 1-5 csillag
3. **Comment** — szöveges vélemény

A **Submit Review** gombra kattintva mentődik el az értékelés. Az átlagos értékelés azonnal frissül.

## Értékelés törlése

Bármelyik értékelés alatt a **Delete** gomb megnyomásával, megerősítés után törlődik az értékelés.

## Lapozás

A filmlista alján található lapozó:
- **← Prev** — előző oldal
- **Page X of Y** — aktuális oldalszám
- **Next →** — következő oldal

A lapozó automatikusan letiltja a Prev / Next gombokat, ha az első / utolsó oldalon vagy.

## Lehetséges hibák

### "Failed to create movie" / "Failed to load movies"

Ez azt jelenti, hogy a backend nem érhető el. Ellenőrizd:
- A K8s pod-ok futnak-e: `kubectl get pods -n movies`
- A port-forward fut-e: `kubectl port-forward -n ingress-nginx svc/ingress-nginx-controller 8088:80`

### Üres lista, miközben tudod, hogy van adat

Frissítsd az oldalt (Cmd+R). Ha még mindig üres, valószínűleg a frontend és a backend között CORS / hálózati probléma van — ellenőrizd a böngésző konzolban (Cmd+Option+I) a Network fülön.

## API közvetlen elérése

Fejlesztők számára a backend Swagger UI-ja közvetlenül is elérhető:

```
http://movies.localhost:8088/swagger
```

Itt minden CRUD végpontot ki lehet próbálni a UI-ból.

A `.http` fájlok a `http-requests/` mappában is használhatóak (VS Code REST Client extension-nel).