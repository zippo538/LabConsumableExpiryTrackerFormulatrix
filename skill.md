---
name: lab-consumable-expiry-tracker
description: Implement or review the .NET 8 Lab Consumable Expiry Tracker domain, application, persistence, DTO, mapping, and tests according to its canonical Item-Lot model, FEFO rules, expiry blocking, consumption, and disposal requirements. Use for work on this project and keep all inventory tracking directly at Lot level.
---

# Lab Consumable Expiry Tracker

## Source of Truth

Use the project PDF for product scope and `diagram.mmd` for the canonical domain contract. When prose is ambiguous, keep the implementation within the classes, members, relationships, and enums defined by the diagram. Do not invent additional warehouse concepts.

## Product Goal

Prevent expired or invalid laboratory reagents and consumables from being used in a new job. Track quantity and expiry for every lot, recommend valid stock using FEFO, record consumption and disposal events, and provide reliable low-stock signals.

Keep the critical expiry, stock, and lot-selection behavior as pure business logic without UI or database dependencies.

## Lot Structure

- `Item` owns zero or more `Lot` entities directly.
- A `Lot` is the smallest tracked inventory unit and contains its own `LotNumber`, `ExpiryDate`, quantities, receipt time, administrative status, and storage location.
- Different expiry dates must be represented as different `Lot` records.
- Do not create another inventory layer beneath `Lot` and do not add a lot-splitting operation.
- Consumption and disposal always reference `LotId` directly.

## Canonical Domain Model

Preserve these diagram-defined types and responsibilities.

### `Item`

- Properties: `Id`, `Code`, `Name`, `BaseUnit`, `MinimumStock`, `ExpiringSoonDays`.
- `BaseUnit` uses `UnitOfMeasure`: `Milliliter`, `Gram`, `Unit`, or `Vial`.
- `IsLowStock(totalQuantity)` compares the aggregate remaining quantity of the item's usable lots with `MinimumStock`.

### `Lot`

- Properties: `Id`, `ItemId`, `LotNumber`, `InitialQuantity`, `RemainingQuantity`, `ExpiryDate`, `ReceivedAt`, `AdministrativeStatus`, `StorageLocation`, and `RowVersion`.
- `AdministrativeStatus` uses `LotAdministrativeStatus`: `Active`, `Quarantined`, `ManuallyBlocked`, or `Disposed`.
- `GetExpiryCondition(now, warningDays)` returns `ExpiryCondition`: `Valid`, `ExpiringSoon`, or `Expired`.
- `IsEligible(now)` determines whether the lot may be selected for new consumption.
- `Consume(quantity, jobId, now)` deducts stock and creates a `Consumption` record.
- `Dispose(quantity, reason, now)` deducts stock and creates a `Disposal` record.
- `Block(reason)` changes the lot to a manually blocked administrative state.

### `Job`

- Properties: `Id`, `JobNumber`, `Status`, `StartedAt`, and `CompletedAt`.
- `Status` uses `JobStatus`: `Draft`, `InProgress`, `Completed`, or `Cancelled`.
- `Start(now)` and `Complete(now)` enforce valid job lifecycle transitions.

### Audit Records

- `Consumption`: `Id`, `JobId`, `LotId`, `Quantity`, `ConsumedAt`, `ConsumedBy`.
- `Disposal`: `Id`, `LotId`, `Quantity`, `Reason`, `DisposedAt`, `DisposedBy`.
- Consumption belongs to one `Job` and one `Lot`; a lot can have many consumption and disposal records.

### Selection and Persistence Contracts

- `LotAllocation`: `LotId`, `Quantity`.
- `LotSelectionService.Allocate(lots, requestedQuantity, now)` returns one or more allocations.
- `ILotRepository`: `GetCandidatesAsync(itemId)`, `GetByIdAsync(id)`, and `AddAsync(lot)`.
- `IUnitOfWork.SaveChangesAsync()` commits stock changes and audit records atomically.
- Use `.NET 8 TimeProvider.GetUtcNow()` for every time-dependent decision.

## Status Semantics

Do not collapse every status into one persisted enum. Derive the user-facing state from three separate concerns:

1. Administrative state comes from `Lot.AdministrativeStatus`.
2. Expiry state comes from `Lot.GetExpiryCondition(...)`.
3. Low stock comes from `Item.IsLowStock(...)` using total usable quantity for that item.

A lot is eligible only when it is administratively `Active`, has `RemainingQuantity > 0`, and is not expired at the time of allocation or consumption. `ExpiringSoon` is a warning and remains eligible unless the user explicitly changes that rule. An expired lot is automatically excluded from new jobs even when quantity remains; do not require a persisted status mutation merely to represent passage of time.

## FEFO Allocation

For a requested item and quantity:

1. Load candidate lots for the `ItemId`.
2. Re-evaluate eligibility using the supplied `now` from `TimeProvider`.
3. Exclude expired, empty, quarantined, manually blocked, and disposed lots.
4. Sort eligible lots by `ExpiryDate` ascending, then `ReceivedAt` ascending. If both values are identical, use `Id` as a stable deterministic tie-breaker.
5. Allocate from the first lot until its remaining quantity is exhausted, then continue to the next eligible lot until the request is fulfilled.
6. Reject the operation without partial persistence when total eligible stock is insufficient.

## Transaction Rules

Reject consumption or disposal when:

- quantity is zero or negative;
- quantity exceeds `RemainingQuantity`;
- the target lot is disposed;
- a concurrency check detects a mismatched `RowVersion`.

Additionally reject consumption when the lot is expired, quarantined, or manually blocked. Re-evaluate expiry at the actual consumption time so a lot selected earlier cannot be consumed after it expires. A previously recorded consumption remains valid audit history if the partially consumed lot expires later.

Perform quantity deductions and creation of `Consumption` or `Disposal` records in one transaction. Never allow `RemainingQuantity` to become negative. Preserve `InitialQuantity`; it is not recalculated after consumption or disposal.

## Architecture and Implementation Rules

- Use Clean Architecture: Domain and Application must not depend on Infrastructure, database, or UI concerns.
- Use Repository Pattern and Dependency Injection.
- Keep expiry evaluation and FEFO allocation deterministic and independently unit-testable.
- For PostgreSQL/EF Core, map the canonical entities directly without adding a header/detail inventory layer.
- Carry `RowVersion` through update DTOs or concurrency tokens where required, and surface concurrency conflicts instead of silently overwriting data.
- DTOs and AutoMapper profiles must follow the canonical model and reference inventory through `LotId`.
- Use decimal-compatible PostgreSQL column types with explicit precision for all quantities.

## Quality Requirements

- Build and test on every push.
- Enable `TreatWarningsAsErrors`.
- Track unit-test coverage with Coverlet/Cobertura or an equivalent tool.
- Target complete coverage of critical domain behavior: expiry boundaries, expiring-soon thresholds, low stock, partial and full consumption, disposal, FEFO across multiple lots, insufficient stock, identical expiry dates, mid-job expiry, and optimistic-concurrency conflicts.
- Keep static-analysis warnings at zero where achievable.

Before completing a change, verify that every production model, DTO, mapping, repository, service, migration, test, and document keeps `Lot` as the smallest tracked inventory unit.
