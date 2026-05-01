# ProductManager - Workflow Documentation

## Tổng quan luồng xử lý

Đây là mô tả chi tiết luồng chạy của chức năng quản lý sản phẩm từ giao diện người dùng đến database.

---

## 1. Luồng hiển thị danh sách sản phẩm

### Sequence Diagram

```mermaid
sequenceDiagram
    participant User
    participant Browser
    participant Controller as ProductsController
    participant AppService as ProductAppService
    participant Repository as ABP Repository
    participant DbContext
    participant Database

    User->>Browser: Truy cập /Products
    Browser->>Controller: GET /Products/Index
    Controller->>AppService: GetAllAsync(input)
    AppService->>Repository: GetAllListAsync()
    Repository->>DbContext: Query Products
    DbContext->>Database: SELECT * FROM Products
    Database-->>DbContext: Return data
    DbContext-->>Repository: IQueryable<Product>
    Repository-->>AppService: List<Product>
    AppService->>AppService: AutoMapper.Map → ProductDto
    AppService-->>Controller: PagedResultDto<ProductDto>
    Controller-->>Browser: Return View(result)
    Browser->>Browser: DataTables khởi tạo
    Browser-->>User: Hiển thị bảng sản phẩm
```

---

## 2. Luồng thêm mới sản phẩm

```mermaid
sequenceDiagram
    participant User
    participant Browser/JS
    participant API as ABP Dynamic API
    participant AppService as ProductAppService
    participant Repository
    participant Database

    User->>Browser/JS: Click "Thêm sản phẩm"
    Browser/JS->>Browser/JS: $('#ProductModal').modal('show')
    User->>Browser/JS: Nhập form → Click Lưu
    Browser/JS->>Browser/JS: Validate form
    Browser/JS->>API: POST /api/services/app/product/create
    API->>AppService: CreateAsync(CreateProductInput)
    AppService->>AppService: Check Permission [AbpAuthorize]
    AppService->>AppService: MapToEntity(input)
    AppService->>Repository: InsertAsync(Product)
    Repository->>Database: INSERT INTO Products
    Database-->>Repository: Success
    Repository-->>AppService: Product entity
    AppService->>AppService: MapToDto(entity)
    AppService-->>API: ProductDto
    API-->>Browser/JS: 200 OK + data
    Browser/JS->>Browser/JS: abp.notify.success()
    Browser/JS->>Browser/JS: dataTable.ajax.reload()
    Browser/JS-->>User: Hiển thị sản phẩm mới
```

---

## 3. Luồng chỉnh sửa sản phẩm

```mermaid
flowchart TD
    A[User click Edit] --> B[JS: _productService.get({id})]
    B --> C[API: GET /product/get]
    C --> D[ProductAppService.GetAsync]
    D --> E[Repository.GetAsync]
    E --> F[Database Query]
    F --> G[Return ProductDto]
    G --> H[JS Fill form data]
    H --> I[Show Modal]
    I --> J[User Edit & Save]
    J --> K[JS: _productService.update]
    K --> L[API: PUT /product/update]
    L --> M[ProductAppService.UpdateAsync]
    M --> N[Check Permission]
    N --> O[Repository.UpdateAsync]
    O --> P[Database UPDATE]
    P --> Q[Return updated ProductDto]
    Q --> R[JS Reload DataTable]
    R --> S[Notify Success]
```

---

## 4. Luồng xóa sản phẩm

```mermaid
flowchart LR
    A[User click Delete] --> B[abp.message.confirm]
    B --> C{Confirmed?}
    C -->|No| D[Cancel]
    C -->|Yes| E[JS: _productService.delete]
    E --> F[API: DELETE /product/delete]
    F --> G[ProductAppService.DeleteAsync]
    G --> H[Check Permission Pages_Products_Delete]
    H --> I[Repository.DeleteAsync]
    I --> J[Database: SOFT DELETE]
    J --> K[Return Success]
    K --> L[JS Reload DataTable]
    L --> M[abp.notify.success]
```

---

## 5. Luồng tìm kiếm và phân trang

```mermaid
sequenceDiagram
    participant User
    participant DataTables
    participant JS as Index.js
    participant API as Product API
    participant AppService
    participant DB as Database

    User->>DataTables: Nhập từ khóa / Chọn filter
    User->>DataTables: Click Search
    DataTables->>JS: listAction.ajaxFunction
    JS->>JS: Build filter input
    JS->>API: POST getAll with filters
    API->>AppService: GetAllAsync(PagedProductResultRequestDto)
    AppService->>AppService: CreateFilteredQuery()
    Note over AppService: Apply filters:<br/>- Keyword search<br/>- CategoryId filter<br/>- Min/Max Price
    AppService->>AppService: ApplySorting()
    AppService->>DB: Executed SQL Query
    DB-->>AppService: Filtered results
    AppService->>AppService: Map to DTOs
    AppService-->>API: PagedResultDto
    API-->>JS: JSON Response
    JS-->>DataTables: Update table data
    DataTables-->>User: Display results
```

---

## 6. Request Lifecycle chi tiết

### HTTP Request Flow

```
┌─────────────────────────────────────────────────────────────┐
│ 1. Browser Request                                          │
│    GET /api/services/app/product/getAll                     │
│    Headers: Authorization: Bearer <token>                   │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│ 2. ASP.NET MVC Routing                                      │
│    ABP Dynamic API Controller chuyển tiếp đến AppService    │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│ 3. Authorization Filter                                     │
│    [AbpAuthorize(PermissionNames.Pages_Products)]           │
│    Kiểm tra user có quyền truy cập không                    │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│ 4. Model Binding                                            │
│    JSON Input → PagedProductResultRequestDto                 │
│    { keyword: "iphone", categoryId: 1, minPrice: 100 }       │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│ 5. Application Service                                      │
│    ProductAppService.GetAllAsync()                          │
│    - CreateFilteredQuery()                                   │
│    - ApplySorting()                                         │
│    - MapToDto via AutoMapper                                 │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│ 6. Repository Pattern                                       │
│    ABP Repository tự động handle:                            │
│    - Query execution                                        │
│    - Change tracking                                        │
│    - Transaction management                                 │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│ 7. Entity Framework Core                                    │
│    DbContext translate LINQ → SQL                          │
│    Execute query với filter:                                │
│    WHERE Name LIKE '%iphone%' AND CategoryId = 1              │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│ 8. Database (SQL Server)                                    │
│    Trả về records phù hợp                                  │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│ 9. Response (Reverse flow)                                  │
│    Database → EF Core → Repository → AppService → JSON      │
│    { items: [...], totalCount: 50 }                         │
└─────────────────────────────────────────────────────────────┘
```

---

## 7. Component Interaction

```mermaid
graph TB
    subgraph "Client Layer"
        V[Views: Index.cshtml]
        JS[Index.js]
        DT[DataTables Plugin]
    end

    subgraph "API Layer"
        API[ABP Dynamic API Controllers]
    end

    subgraph "Application Layer"
        I[ICategoryAppService]
        P[IProductAppService]
        PS[ProductAppService]
        CS[CategoryAppService]
    end

    subgraph "Domain Layer"
        R[ABP Repository]
        E[Product Entity]
        C[Category Entity]
    end

    subgraph "Infrastructure Layer"
        DB[ProductManagerDbContext]
        SQL[(SQL Server)]
    end

    V --> JS
    JS --> DT
    JS --> API
    API --> I
    API --> P
    P --> PS
    I --> CS
    PS --> R
    CS --> R
    R --> E
    R --> C
    R --> DB
    DB --> SQL
```

---

## 8. Data Flow Summary

| Thao tác | Input | Processing | Output |
|----------|-------|------------|--------|
| **List** | Page, Filters | Query + Filter + Sort | PagedResultDto |
| **Get** | EntityDto<int> | Repository.Find | ProductDto |
| **Create** | CreateProductInput | MapToEntity + Insert | ProductDto |
| **Update** | ProductDto | MapToEntity + Update | ProductDto |
| **Delete** | EntityDto<int> | SoftDelete | void |

---

## 9. Security Flow

```mermaid
flowchart TD
    A[Request] --> B{Has AbpAuthorize?}
    B -->|Yes| C[Check Permission]
    B -->|No| D[Allow Anonymous]
    C --> E{Has Permission?}
    E -->|Yes| F[Execute Method]
    E -->|No| G[Return 403 Forbidden]
    F --> H[Log Audit]
    D --> F
```
