# ProductManager - Core Logic Explanation

## Giới thiệu

Tài liệu này giải thích chi tiết các đoạn code cốt lõi trong ProductManager, giúp người mới hiểu được logic và thuật toán đằng sau.

---

## 1. Generic CRUD với AsyncCrudAppService

### Code:

```csharp
// File: src/ProductManager.Application/Products/ProductAppService.cs

[AbpAuthorize(PermissionNames.Pages_Products)]
public class ProductAppService :
    AsyncCrudAppService<Product, ProductDto, int, 
                         PagedProductResultRequestDto, 
                         CreateProductInput, ProductDto>,
    IProductAppService
{
    public ProductAppService(IRepository<Product, int> repository) : base(repository)
    {
    }
}
```

### Giải thích:

**Vấn đề:** Khi xây dựng CRUD operations, ta thường phải viết lặp đi lặp lại các thao tác: GetAll, Get, Create, Update, Delete.

**Giải pháp:** ABP cung cấp `AsyncCrudAppService<TEntity, TDto, TKey, TGetAllInput, TCreateInput, TUpdateInput>` - một lớp generic đã implement sẵn toàn bộ CRUD.

**Các tham số generic:**
| Tham số | Ý nghĩa | Ví dụ |
|---------|---------|-------|
| `TEntity` | Entity class | `Product` |
| `TDto` | DTO trả về | `ProductDto` |
| `TKey` | Kiểu khóa chính | `int` |
| `TGetAllInput` | Input cho GetAll (phân trang/filter) | `PagedProductResultRequestDto` |
| `TCreateInput` | Input cho Create | `CreateProductInput` |
| `TUpdateInput` | Input cho Update | `ProductDto` |

**Lợi ích:**
- Không cần viết code CRUD thủ công
- Tự động có API endpoints
- Tự động validation và authorization

---

## 2. Dynamic Filtering và Searching

### Code:

```csharp
// File: src/ProductManager.Application/Products/ProductAppService.cs

protected override IQueryable<Product> CreateFilteredQuery(
    PagedProductResultRequestDto input)
{
    var query = base.CreateFilteredQuery(input);  // Lấy base query
    
    // Filter theo từ khóa (tìm trong Name và Description)
    if (!string.IsNullOrWhiteSpace(input.Keyword))
    {
        query = query.Where(x => 
            x.Name.Contains(input.Keyword) || 
            x.Description.Contains(input.Keyword));
    }
    
    // Filter theo Category
    if (input.CategoryId.HasValue)
    {
        query = query.Where(x => x.CategoryId == input.CategoryId.Value);
    }
    
    // Filter theo khoảng giá
    if (input.MinPrice.HasValue)
    {
        query = query.Where(x => x.Price >= input.MinPrice.Value);
    }
    
    if (input.MaxPrice.HasValue)
    {
        query = query.Where(x => x.Price <= input.MaxPrice.Value);
    }
    
    return query;
}

protected override IQueryable<Product> ApplySorting(
    IQueryable<Product> query,
    PagedProductResultRequestDto input)
{
    // Sắp xếp mặc định theo thời gian tạo, mới nhất lên đầu
    return query.OrderByDescending(x => x.CreationTime);
}
```

### Giải thích:

**Pattern:** Đây là **Repository Pattern với Specification Pattern** kết hợp.

**Luồng xử lý:**
```
Client Request
    ↓
PagedProductResultRequestDto (chứa filter params)
    ↓
CreateFilteredQuery() → Thêm WHERE clauses động
    ↓
ApplySorting() → Thêm ORDER BY
    ↓
ABP Repository → Execute SQL
    ↓
AutoMapper → Map to DTOs
    ↓
PagedResultDto trả về Client
```

**Ưu điểm của cách tiếp cận này:**

1. **Deferred Execution**: `IQueryable` chưa thực thi SQL ngay. Tất cả filter được build xong mới chuyển thành SQL một lần.

2. **Dynamic Query Building**: Tùy thuộc input có giá trị hay không, WHERE clause được thêm/xóa động:
   ```sql
   -- Nếu chỉ có Keyword
   SELECT * FROM Products WHERE Name LIKE '%iphone%'
   
   -- Nếu có Keyword + Category + Price
   SELECT * FROM Products 
   WHERE Name LIKE '%iphone%' 
     AND CategoryId = 1 
     AND Price >= 100 
     AND Price <= 1000
   ```

3. **SQL Injection Safe**: EF Core tự động parameterize queries, tránh SQL injection.

---

## 3. JavaScript Dynamic API Proxy và DataTables Integration

### Code:

```javascript
// File: src/ProductManager.Web.Mvc/wwwroot/view-resources/Views/Products/Index.js

(function () {
    $(function () {
        // 1. ABP tự động tạo proxy client
        var _productService = abp.services.app.product;
        
        // 2. Khởi tạo DataTable với server-side processing
        var dataTable = $('#ProductsTable').DataTable({
            paging: true,
            serverSide: true,  // Phân trang server-side
            processing: true,
            listAction: {
                ajaxFunction: _productService.getAll,
                inputFilter: function () {
                    return {
                        keyword: $('#Keyword').val(),
                        categoryId: $('#CategoryId').val(),
                        minPrice: $('#MinPrice').val(),
                        maxPrice: $('#MaxPrice').val()
                    };
                }
            },
            columnDefs: [
                { targets: 0, data: "id" },
                { targets: 1, data: "name" },
                { targets: 2, data: "categoryName" },
                { 
                    targets: 3, 
                    data: "price",
                    render: function (data) {
                        return data.toLocaleString('vi-VN') + ' đ';
                    }
                },
                { targets: 4, data: "stockQuantity" },
                {
                    targets: 5,
                    data: null,
                    orderable: false,
                    render: function (data, type, row) {
                        return `
                            <button class="btn btn-sm btn-warning edit-product" 
                                    data-id="${row.id}">
                                <i class="fas fa-edit"></i>
                            </button>
                            <button class="btn btn-sm btn-danger delete-product" 
                                    data-id="${row.id}">
                                <i class="fas fa-trash"></i>
                            </button>`;
                    }
                }
            ]
        });

        // 3. Create/Update operation
        $('#SaveProductButton').click(function (e) {
            e.preventDefault();
            
            var productId = $('#ProductId').val();
            var productData = {
                name: $('#ProductName').val(),
                price: parseFloat($('#ProductPrice').val()),
                stockQuantity: parseInt($('#ProductStock').val()),
                categoryId: parseInt($('#ProductCategoryId').val())
            };

            abp.ui.setBusy('#ProductModal');  // Show loading spinner

            if (productId) {
                // UPDATE: Thêm Id vào data
                productData.id = parseInt(productId);
                _productService.update(productData).done(function () {
                    abp.notify.success('Cập nhật thành công');
                    $('#ProductModal').modal('hide');
                    dataTable.ajax.reload();  // Refresh table
                }).always(function () {
                    abp.ui.clearBusy('#ProductModal');
                });
            } else {
                // CREATE: Không cần Id
                _productService.create(productData).done(function () {
                    abp.notify.success('Thêm sản phẩm thành công');
                    $('#ProductModal').modal('hide');
                    dataTable.ajax.reload();
                }).always(function () {
                    abp.ui.clearBusy('#ProductModal');
                });
            }
        });

        // 4. Delete with confirmation
        $(document).on('click', '.delete-product', function () {
            var productId = $(this).data('id');
            
            abp.message.confirm(
                'Bạn có chắc muốn xóa sản phẩm này?',
                'Xác nhận xóa',
                function (isConfirmed) {
                    if (isConfirmed) {
                        _productService.delete({ id: productId })
                            .done(function () {
                                abp.notify.success('Xóa thành công');
                                dataTable.ajax.reload();
                            });
                    }
                }
            );
        });
    });
})();
```

### Giải thích:

#### A. ABP Dynamic JavaScript Proxies

**Vấn đề:** Truyền thống để gọi API từ JavaScript, ta phải:
```javascript
// Cách truyền thống - phải viết thủ công
fetch('/api/products', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data)
})
```

**ABP giải quyết:** Tự động generate JavaScript proxy từ AppService:
```javascript
// ABP tự động tạo
var _productService = abp.services.app.product;

// Gọi như function thông thường, ABP lo phần HTTP
_productService.create(data).done(callback);
```

**Ưu điểm:**
- Type-safe (có IntelliSense)
- Tự động xử lý authentication token
- Tự động serialize/deserialize JSON
- Built-in error handling

#### B. DataTables Server-Side Processing

**Luồng dữ liệu:**
```
User Search/Filter/Pagination
    ↓
DataTables gửi request với parameters:
  - start: 0 (bắt đầu từ record 0)
  - length: 10 (lấy 10 records)
  - search[value]: "iphone"
    ↓
Index.js → inputFilter() chuyển thành PagedProductResultRequestDto:
  {
    skipCount: 0,
    maxResultCount: 10,
    keyword: "iphone"
  }
    ↓
ProductAppService.GetAllAsync()
    ↓
Database Query ( chỉ lấy 10 records phù hợp )
    ↓
Trả về:
  {
    items: [10 products],
    totalCount: 150 (tổng số phù hợp)
  }
    ↓
DataTables hiển thị: "Showing 1 to 10 of 150 entries"
```

**Tại sao dùng Server-Side Processing?**

| Trường hợp | Client-Side | Server-Side |
|------------|-------------|-------------|
| 100 records | ✅ Nhanh | ❌ Chậm (thừa) |
| 10,000 records | ❌ Chậm, lag browser | ✅ Nhanh |
| Filter trên 10,000 | ❌ Download all rồi filter | ✅ SQL WHERE |

**ColumnDefs giải thích:**

```javascript
columnDefs: [
    { targets: 0, data: "id" },  // Cột 0: lấy field "id" từ JSON
    { 
        targets: 3, 
        data: "price",
        render: function (data) {  // Custom render
            return data.toLocaleString('vi-VN') + ' đ';
            // 1000000 → "1.000.000 đ"
        }
    },
    {
        targets: 5,
        data: null,  // Không lấy từ data, tự render
        orderable: false,  // Không sort cột này
        render: function (data, type, row) {
            // row = { id: 1, name: "iPhone", ... }
            return `<button data-id="${row.id}">...</button>`;
        }
    }
]
```

#### C. Event Delegation Pattern

```javascript
// ❌ Cách sai: Event sẽ không bind với row mới sau khi reload
$('.delete-product').click(function() { ... });

// ✅ Cách đúng: Dùng Event Delegation
$(document).on('click', '.delete-product', function () {
    // Event được delegate lên document, hoạt động với element động
});
```

---

## 4. AutoMapper Configuration

### Code:

```csharp
// File: src/ProductManager.Application/Products/ProductMapProfile.cs

public class ProductMapProfile : Profile
{
    public ProductMapProfile()
    {
        // 1. Entity → DTO (đọc)
        CreateMap<Product, ProductDto>();
        
        // 2. CreateInput → Entity (tạo mới)
        CreateMap<CreateProductInput, Product>();
        
        // 3. DTO → Entity (cập nhật)
        CreateMap<ProductDto, Product>();
    }
}
```

### Giải thích:

**Vấn đề:** Entity chứa cả property không cần trả về client (như IsDeleted, DeleterUserId). Truyền thẳng Entity ra ngoài là **NGUY HIỂM**.

**Giải pháp:** DTO (Data Transfer Object) chỉ chứa field cần thiết.

**Mapping qua lại:**

```
┌─────────────────────────────────────────────────────────────┐
│                    READ Flow                                │
│  Database                                                     │
│     ↓                                                        │
│  Product Entity { Id, Name, Price, IsDeleted, ... }          │
│     ↓  AutoMapper.Map<Product, ProductDto>()                │
│  ProductDto { Id, Name, Price, CategoryName }              │
│     ↓                                                        │
│  JSON → Browser                                              │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│                    CREATE Flow                              │
│  Browser                                                     │
│     ↓                                                        │
│  CreateProductInput { Name, Price, CategoryId }              │
│     ↓  AutoMapper.Map<CreateProductInput, Product>()        │
│  Product Entity { Name, Price, CategoryId, IsDeleted... }  │
│     ↓                                                        │
│  Database INSERT                                             │
└─────────────────────────────────────────────────────────────┘
```

---

## 5. Authorization với ABP Attributes

### Code:

```csharp
// File: src/ProductManager.Application/Products/ProductAppService.cs

[AbpAuthorize(PermissionNames.Pages_Products)]  // Class-level: tất cả methods đều cần quyền này
public class ProductAppService : ...
{
    [AbpAuthorize(PermissionNames.Pages_Products_Create)]
    public override Task<ProductDto> CreateAsync(CreateProductInput input)
    {
        return base.CreateAsync(input);
    }

    [AbpAuthorize(PermissionNames.Pages_Products_Edit)]
    public override Task<ProductDto> UpdateAsync(ProductDto input)
    {
        return base.UpdateAsync(input);
    }

    [AbpAuthorize(PermissionNames.Pages_Products_Delete)]
    public override async Task DeleteAsync(EntityDto<int> input)
    {
        await base.DeleteAsync(input);
    }
}
```

### Giải thích:

**Permission Hierarchy:**
```
Pages_Products (xem danh sách)
├── Pages_Products_Create (thêm mới)
├── Pages_Products_Edit (chỉnh sửa)
└── Pages_Products_Delete (xóa)
```

**Luồng kiểm tra quyền:**
```
HTTP Request đến API
    ↓
ABP Authorization Filter
    ↓
Kiểm tra User đã login? (IsAuthenticated)
    ↓
Lấy UserId → Query AbpUserRoles
    ↓
Kiểm tra Role có Permission này không?
    ↓
[Yes] → Cho phép thực thi
[No]  → Trả về 403 Forbidden
```

**Ưu điểm:**
- Declarative: Chỉ cần gắn attribute, không cần viết `if (!hasPermission) return;`
- Centralized: Tất cả rules ở một nơi
- Audit trail: ABP tự log ai làm gì

---

## Tóm tắt

| Pattern | File | Lợi ích |
|---------|------|---------|
| **Generic CRUD** | `ProductAppService.cs` | Không viết code lặp |
| **Dynamic Filtering** | `CreateFilteredQuery()` | Filter động, SQL tối ưu |
| **API Proxy** | `Index.js` | Type-safe, tự động HTTP |
| **Server-Side DT** | `DataTables config` | Hiệu năng tốt với data lớn |
| **AutoMapper** | `ProductMapProfile.cs` | Tách biệt Entity/DTO |
| **Declarative Auth** | `[AbpAuthorize]` | An toàn, dễ maintain |
