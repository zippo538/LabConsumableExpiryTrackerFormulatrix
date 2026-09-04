# AI Agent Skill Profile: Lab Consumable Expiry Tracker & WMS Management

## Reference Material
This skill profile integrates WMS operational logic  with the specific business, architectural, and quality assurance requirements of the Lab Consumable Expiry Tracker.

## 1. Core Objectives & Laboratory Context
* **Primary Goal:** Prevent silent quality defects and invalid QC test results caused by the use of expired or near-empty laboratory reagents and consumables.
* **Automated Expiry Block:** The system must automatically block any expired lot from being assigned to a new experimental job or production run.

## 2. Expiry Status Engine & Configurable Thresholds
* **Lot Status Evaluation:** The system continuously evaluates every lot's status into four categories: Available, Expiring Soon, Low Stock, or Blocked/Expired .
* **Configurable Thresholds:** Minimum stock rules and expiry thresholds must be configurable per reagent/item type (applying the Open/Closed Principle) to avoid hardcoding and ensure maintainability as new reagents are introduced .
* **Auditable Consumption Log:** Every time a lot is consumed against a job, the remaining quantity is updated, and the system produces a full auditable history of stock usage and disposals.

## 3. Sub-Lot Architecture & Inbound Validation
* **Sub-Lot Generation:** If a single supplier lot contains items with different expiry dates, they must be split into Internal Lots or Sub-Lots (e.g., `LOT-A-01` and `LOT-A-02`).
* **GRN Validation:** Upon receipt, warehouse staff are forced by the system to input the Lot Number and Expiry Date. Discrepancies trigger a pop-up and an automated stock split .
* **Automated Putaway & LPN:** Sub-lots with different expiries are allocated to separate bin locations. Pallets/cartons are tracked via a unique License Plate Number (LPN) barcode binding Item Name, Manufacturing Lot, and Specific Expiry Date.

## 4. FEFO Lot Recommendation & Edge Case Handling
* **FEFO Selection Rules:** The system automatically recommends lots using First-Expired-First-Out (FEFO) in the following order :
  1. Status is Active .
  2. Item is not expired .
  3. Expiry Date is the closest .
  4. Received Date (ReceivedAt) is the oldest .
  5. SubLotNumber (as the final tie-breaker for lots with the exact same expiry date) .
* **Edge Case Mitigation:** The AI must correctly model the state machine for edge cases, including :
  * A lot that expires mid-job .
  * A partially consumed lot that later expires .
  * Handling multiple lots with identical expiry dates .

## 5. Transaction Validations
The system must reject Consume, Dispose, or Split operations if :
* Quantity is zero or negative .
* Quantity exceeds `RemainingQuantity` .
* Sub-lot is expired (for consumption) .
* Sub-lot status is Quarantined, ManuallyBlocked, Disposed, or Depleted .
* A concurrency conflict occurs (mismatched `RowVersion`) .

## 6. Role Matrix & Responsibilities
* **Scientist:** Selects items and quantities . Does not manually check expiry or select specific lots .
* **System (AI Agent):** Validates stock, applies FEFO, automatically checks expiry, blocks expired lots, recommends valid lots, and logs consumption .
* **Admin Gudang:** Corrects quantities, handles exceptions, verifies physical expiry, and overrides sub-lots if justified .

## 7. Database Schema Knowledge
* **`m_product`**: `product_id`, `product_name`, `uom` .
* **`t_lot_hdr`**: `lot_hdr_id`, `supplier_lot_number`, `supplier_id` .
* **`t_lot_dtl` (Sub-Lot)**: `lot_dtl_id`, `sub_lot_code`, `expiry_date`, `production_date` .
* **`t_stock_balance`**: Tracks availability linking specifically to `lot_dtl_id` and `bin_location_id` .

## 8. Technical Architecture & Quality Assurance
* **Clean Architecture:** Domain and Application layers must be strictly independent of infrastructure to allow isolated unit testing .
* **Design Patterns:** System utilizes Repository Pattern and Dependency Injection .
* **Time Testing:** The system MUST use the `.NET 8 TimeProvider` abstraction for all expiry evaluations . This ensures deterministic, expiry-date-based testing at 100% coverage without mocking the system clock directly .
* **Code Quality Rules:** CI pipeline must be green on every push, enforced zero compiler warnings (`TreatWarningsAsErrors`), and static analysis warnings kept at 0 .
