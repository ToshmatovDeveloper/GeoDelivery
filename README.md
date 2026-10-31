# GeoDelivery

**Backend platform for a food-delivery marketplace, built as a modular monolith with .NET 10, DDD, CQRS, PostgreSQL/PostGIS and Redis.**

GeoDelivery connects three sides of a delivery business in one API:

- **customers** browse restaurants, place orders and cancel them;
- **restaurants** publish their menu, categories and available dishes;
- **operators and couriers** manage delivery zones, publish courier positions and assign couriers.

The system is one deployable ASP.NET Core application divided into **five isolated business modules** plus a Web host. Each module owns its domain model, handlers and database schema, which keeps boundaries clear and allows future extraction into separate services.

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-Web_API-512BD4)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-4169E1?logo=postgresql&logoColor=white)
![PostGIS](https://img.shields.io/badge/PostGIS-3.4-4169E1)
![Redis](https://img.shields.io/badge/Redis-7-DC382D?logo=redis&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?logo=docker&logoColor=white)

---

## What it does

- **Restaurant catalog** — restaurants, categories, dishes, prices and availability.
- **Ordering** — create an order with delivery coordinates, snapshot line items, calculate totals, view and cancel orders.
- **Delivery zones** — PostGIS polygons, point-in-zone checks, nearest-zone lookup and distance calculation.
- **Courier tracking** — live courier positions in Redis GEO and nearby-courier search by radius.
- **Dispatch** — courier assignments with statuses and a background dispatch worker.
- **Identity** — registration, login, JWT access tokens, rotating refresh tokens and role-based access.
- **Performance** — Redis read-through cache for selected catalog queries.

## Business rules

### Catalog

- A category belongs to one restaurant; a dish belongs to one restaurant and one of its categories.
- A dish can only be created when its category belongs to the same restaurant.
- Dish name is required and price must be greater than zero.
- A dish can be marked unavailable so clients stop offering it.
- Catalog reads are public; catalog writes require the `Admin` role.

### Orders

- An order belongs to one customer, one restaurant and one delivery coordinate.
- Each line item snapshots the dish ID, name, unit price and quantity.
- The order total is calculated on the server as `Σ(price × quantity)`.
- Line-item prices are currently taken from the request and are not re-validated against the catalog yet.
- A customer can cancel only their own order. Creation sets `Created`; cancellation sets `Cancelled`.
- Other statuses: `Accepted`, `Cooking`, `ReadyForDelivery`, `Delivering`, `Completed`.

### Delivery and dispatch

- A delivery zone requires at least three valid latitude/longitude points and is stored as a PostGIS polygon (SRID 4326).
- The system can check whether a point is inside a zone and find the nearest active zone.
- Courier positions live in the Redis GEO key `active_couriers`; nearby search uses a radius in kilometres.
- Publishing a courier position requires the `Courier` role.
- A courier assignment stores the order, restaurant, courier, status and timestamps. Statuses: `Pending`, `Assigned`, `Completed`, `Cancelled`.

### Identity and security

- Username and email must be unique; passwords are hashed by ASP.NET Core Identity.
- Login returns a 10-minute access token and a 7-day refresh token.
- Refresh tokens are rotated on every refresh and cleaned daily.
- Access is role-based: `Admin`, `Manager`, `Courier`, `Client`.
- New registrations receive the `User` role; privileged roles are not seeded automatically yet.

---

## Architecture

GeoDelivery is a **modular monolith**:

- **One deployable unit** — a single Web API hosts all modules.
- **Module isolation** — modules do not reference each other; in-process communication goes through MediatR.
- **Schema per module** — one PostgreSQL database, separate schemas for Auth, Catalog, Order, Geo and Dispatch.
- **CQRS** — commands and queries have dedicated handlers; controllers only call `IMediator`.
- **Pipeline behaviors** — FluentValidation validation, Redis caching and Geo-specific validation run around handlers.
- **Thin presentation** — controllers bind HTTP input and return handler results; domain errors are mapped by exception handlers.

Each module follows the same layout:

```text
<Module>/
├── Domain/          # Entities, value objects, enums
├── Application/     # Commands, queries, handlers, validators
├── Infrastructure/  # DbContext, mappings, migrations, services
└── Presenters/      # Controllers
```

`Web` is the composition root: it references all modules, registers services and hosts the HTTP pipeline, Swagger and Serilog.

---

## Modules

The solution contains **six projects: five business modules plus the Web host**.

| Module | Tasks | Own data |
| --- | --- | --- |
| **Auth** | Users, roles, JWT issuing, refresh-token rotation and cleanup | `auth` schema |
| **Catalog** | Restaurants, categories, dishes, availability and cached reads | `public` schema + Redis |
| **Order** | Order creation, line items, totals, statuses and cancellation | `order` schema |
| **Geo** | Delivery-zone polygons, distance, nearest zone and live courier positions | `geo` schema + Redis GEO |
| **Dispatch** | Courier assignments, assignment statuses and dispatch worker | `dispatch` schema |
| **Web** | DI composition, middleware, authentication, Swagger, logging | — |

### Module details

- **Auth** — registers users with unique credentials, issues JWTs with role claims, stores refresh tokens and deletes stale tokens daily.
- **Catalog** — creates restaurants and categories, validates that a dish's restaurant and category match, supports unavailable dishes and caches popular reads.
- **Order** — persists orders with owned delivery coordinates and line items, computes totals and enforces customer ownership on cancellation.
- **Geo** — stores zone polygons with a GiST index, closes polygons automatically, checks coverage, finds the nearest zone and tracks couriers in Redis.
- **Dispatch** — records who is assigned to an order. The domain and persistence are ready; automatic pending-order lookup and courier matching are still placeholders.
- **Web** — wires everything together, applies JWT authentication, exception handling, Swagger and structured logging.

---

## Technologies

| Area | Technology |
| --- | --- |
| Runtime | .NET 10 / ASP.NET Core, C# 14 |
| Architecture | Modular monolith, DDD, CQRS |
| Messaging | MediatR |
| Validation | FluentValidation |
| Persistence | EF Core 10, Npgsql |
| Database | PostgreSQL 16 + PostGIS 3.4 |
| Geospatial | NetTopologySuite, PostGIS, Redis GEO |
| Cache | Redis 7, StackExchange.Redis |
| Identity | ASP.NET Core Identity |
| Auth | JWT Bearer + refresh tokens |
| Logging | Serilog (console, file, Seq) |
| API docs | Swagger / Swashbuckle |
| Local infrastructure | Docker Compose |

---

## License

No license file is included yet. Until one is added, the code is **all rights reserved**; contact the repository owner for permission to use or distribute it.
