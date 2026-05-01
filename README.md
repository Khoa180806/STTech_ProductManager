# ProductManager - ASP.NET Boilerplate Application

## Giới thiệu

ProductManager là ứng dụng quản lý sản phẩm và danh mục được xây dựng trên **ASP.NET Boilerplate (ABP)** framework với kiến trúc **Domain Driven Design (DDD)** và **Layered Architecture**.

## Tính năng chính

- ✅ **Quản lý Sản phẩm**: Thêm, sửa, xóa, tìm kiếm sản phẩm
- ✅ **Quản lý Danh mục**: Phân loại sản phẩm theo danh mục
- ✅ **Phân trang Server-Side**: Xử lý hiệu quả với số lượng lớn dữ liệu
- ✅ **Tìm kiếm nâng cao**: Theo từ khóa, danh mục, khoảng giá
- ✅ **Phân quyền chi tiết**: View, Create, Edit, Delete permissions
- ✅ **Giao diện Modal**: Thao tác không cần load lại trang
- ✅ **Responsive UI**: AdminLTE + Bootstrap

## Kiến trúc tổng quan

```
src/
├── ProductManager.Core/                  # Domain Layer
│   ├── Entities/                         # Product, Category
│   ├── Authorization/                    # Permissions
│   └── Localization/                     # Multi-language
│
├── ProductManager.EntityFrameworkCore/   # Data Access Layer
│   ├── EntityFrameworkCore/              # DbContext
│   └── Migrations/                       # Database migrations
│
├── ProductManager.Application/           # Application Layer
│   ├── Products/                           # Product AppService, DTOs
│   └── Categories/                         # Category AppService, DTOs
│
└── ProductManager.Web.Mvc/               # Presentation Layer
    ├── Controllers/                        # MVC Controllers
    ├── Views/                              # Razor Views
    ├── wwwroot/view-resources/             # JavaScript files
    └── Startup/                            # Navigation, Bundles
```

## Công nghệ sử dụng

| Layer | Technology |
|-------|------------|
| **Backend** | ASP.NET Core 8.0, ABP Framework 10.x |
| **Database** | SQL Server, Entity Framework Core |
| **Frontend** | jQuery, DataTables, AdminLTE |
| **API** | ABP Dynamic API Proxy |
| **Mapping** | AutoMapper |
| **DI** | Built-in .NET DI Container |

## Hướng dẫn chạy project

### 1. Yêu cầu

- .NET 8.0 SDK
- SQL Server 2019+ hoặc SQL Server Express
- Visual Studio 2022 hoặc JetBrains Rider

### 2. Cấu hình Database

Mở file `appsettings.json` trong `ProductManager.Web.Mvc`:

```json
{
  "ConnectionStrings": {
    "Default": "Server=YOUR_SERVER;Database=ProductManagerDb;Trusted_Connection=True;"
  }
}
```

### 3. Chạy Migration

```bash
# Tại thư mục aspnet-core/
cd src/ProductManager.EntityFrameworkCore

# Tạo migration (nếu cần)
dotnet ef migrations add InitialCreate --startup-project ../ProductManager.Web.Mvc

# Apply migration
dotnet ef database update --startup-project ../ProductManager.Web.Mvc
```

Hoặc dùng **Package Manager Console** trong Visual Studio:

```powershell
# Chọn Default Project: ProductManager.EntityFrameworkCore
Add-Migration InitialCreate
Update-Database
```

### 4. Chạy ứng dụng

```bash
cd src/ProductManager.Web.Mvc
dotnet run
```

Truy cập: `https://localhost:44312` (hoặc port tương ứng)

**Default Account:**
- Username: `admin`
- Password: `123qwe`

## Cấu trúc Module

### Product Module

```
Products/
├── IProductAppService.cs          # Interface định nghĩa API
├── ProductAppService.cs           # Implementation CRUD
├── ProductMapProfile.cs             # AutoMapper config
├── Dto/
│   ├── ProductDto.cs                # DTO trả về
│   ├── CreateProductInput.cs        # Input tạo mới
│   └── PagedProductResultRequestDto.cs  # Input tìm kiếm
```

### Category Module

```
Categories/
├── ICategoryAppService.cs
├── CategoryAppService.cs
├── CategoryMapProfile.cs
└── Dto/
    ├── CategoryDto.cs
    ├── CreateCategoryInput.cs
    └── PagedCategoryResultRequestDto.cs
```

## API Endpoints

ABP tự động tạo REST API từ AppService:

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/services/app/product/getAll` | POST | Lấy danh sách có phân trang |
| `/api/services/app/product/get` | POST | Lấy chi tiết 1 sản phẩm |
| `/api/services/app/product/create` | POST | Tạo sản phẩm mới |
| `/api/services/app/product/update` | PUT | Cập nhật sản phẩm |
| `/api/services/app/product/delete` | DELETE | Xóa sản phẩm |
| `/api/services/app/category/getAll` | POST | Lấy danh sách danh mục |
| `/api/services/app/category/create` | POST | Tạo danh mục |
| `/api/services/app/category/update` | PUT | Cập nhật danh mục |
| `/api/services/app/category/delete` | DELETE | Xóa danh mục |

## Permissions

| Permission | Key | Description |
|------------|-----|-------------|
| View Products | `Pages.Products` | Xem danh sách sản phẩm |
| Create Product | `Pages.Products.Create` | Thêm sản phẩm mới |
| Edit Product | `Pages.Products.Edit` | Sửa sản phẩm |
| Delete Product | `Pages.Products.Delete` | Xóa sản phẩm |

## Development Guide

### Thêm Entity mới

1. **Tạo Entity** trong `Core/Entities/`
2. **Thêm DbSet** trong `DbContext`
3. **Tạo Migration**: `Add-Migration Added_X_Entity`
4. **Tạo DTOs** trong `Application/{Module}/Dto/`
5. **Tạo AppService** và Interface
6. **Tạo Controller** trong `Web.Mvc/Controllers/`
7. **Tạo Views** trong `Web.Mvc/Views/`

### Thêm JavaScript mới

1. Thêm file vào `wwwroot/view-resources/Views/{Controller}/`
2. Register trong `bundles.json`
3. Include trong View với `@section scripts`

## Troubleshooting

### Lỗi "NoDbHost" hoặc không connect được DB
- Kiểm tra ConnectionString trong `appsettings.json`
- Đảm bảo SQL Server đang chạy

### Lỗi 500 khi Create/Update/Delete
- Kiểm tra JavaScript truyền đúng format: `{ id: value }` cho Get/Delete
- Kiểm tra AppService có `[AbpAuthorize]` phù hợp

### JS Proxy không tìm thấy (abp.services.app.xxx undefined)
- Build lại project
- Hard refresh browser (Ctrl+F5)
- Kiểm tra bundle đã include file JS

## Tài liệu tham khảo

- [ABP Framework Documentation](https://docs.abp.io/)
- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [DataTables](https://datatables.net/)

## License

MIT License

## Liên hệ

Nếu có vấn đề hoặc câu hỏi, vui lòng tạo issue trong repository.
