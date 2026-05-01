# ProductManager - Project Structure

## Overview

ProductManager là một ứng dụng quản lý sản phẩm và danh mục xây dựng trên ASP.NET Boilerplate (ABP) framework với kiến trúc phân tầng Layered Architecture.

---

## Layer Architecture

```
┌─────────────────────────────────────────┐
│         Web.Mvc Layer                  │
│  (Controllers, Views, JavaScript)      │
├─────────────────────────────────────────┤
│       Application Layer                │
│  (AppServices, DTOs, Interfaces)       │
├─────────────────────────────────────────┤
│         Core Layer                     │
│  (Entities, Domain Logic)              │
├─────────────────────────────────────────┤
│    EntityFrameworkCore Layer           │
│  (DbContext, Migrations)               │
└─────────────────────────────────────────┘
```

---

## File Structure Details

### 1. Core Layer (`src/ProductManager.Core`)

| File | Path | Purpose |
|------|------|---------|
| `Product.cs` | `Entities/Product.cs` | Entity sản phẩm với các thuộc tính: Name, Description, Price, StockQuantity, CategoryId |
| `Category.cs` | `Entities/Category.cs` | Entity danh mục với Name, Description |
| `PermissionNames.cs` | `Authorization/PermissionNames.cs` | Constants định nghĩa các quyền: Pages_Products, Pages_Products_Create, Pages_Products_Edit, Pages_Products_Delete |
| `ProductManagerAuthorizationProvider.cs` | `Authorization/` | Đăng ký các permission vào ABP authorization system |
| `ProductManager.xml` | `Localization/SourceFiles/` | File ngôn ngữ chứa các text hiển thị (Products, Categories, v.v.) |

### 2. EntityFrameworkCore Layer (`src/ProductManager.EntityFrameworkCore`)

| File | Path | Purpose |
|------|------|---------|
| `ProductManagerDbContext.cs` | `EntityFrameworkCore/` | DbContext định nghĩa DbSet<Product> và DbSet<Category>, cấu hình relationship |
| Migrations | `Migrations/` | Các file migration tự động tạo database schema |

### 3. Application Layer (`src/ProductManager.Application`)

#### Products Module

| File | Path | Purpose |
|------|------|---------|
| `IProductAppService.cs` | `Products/IProductAppService.cs` | Interface định nghĩa các phương thức CRUD |
| `ProductAppService.cs` | `Products/ProductAppService.cs` | Implementation CRUD logic, phân quyền, filtering, sorting |
| `ProductDto.cs` | `Products/Dto/ProductDto.cs` | DTO trả về thông tin sản phẩm (bao gồm CategoryName) |
| `CreateProductInput.cs` | `Products/Dto/CreateProductInput.cs` | Input DTO cho việc tạo sản phẩm mới |
| `PagedProductResultRequestDto.cs` | `Products/Dto/` | DTO cho phân trang và tìm kiếm (Keyword, CategoryId, MinPrice, MaxPrice) |
| `ProductMapProfile.cs` | `Products/ProductMapProfile.cs` | AutoMapper profile chuyển đổi Entity ↔ DTO |

#### Categories Module

| File | Path | Purpose |
|------|------|---------|
| `ICategoryAppService.cs` | `Categories/ICategoryAppService.cs` | Interface CRUD cho Category |
| `CategoryAppService.cs` | `Categories/CategoryAppService.cs` | Implementation Category service |
| `CategoryDto.cs` | `Categories/Dto/CategoryDto.cs` | DTO cho Category |
| `CreateCategoryInput.cs` | `Categories/Dto/CreateCategoryInput.cs` | Input cho tạo Category |
| `PagedCategoryResultRequestDto.cs` | `Categories/Dto/` | DTO phân trang Category |
| `CategoryMapProfile.cs` | `Categories/CategoryMapProfile.cs` | AutoMapper cho Category |

### 4. Web.Mvc Layer (`src/ProductManager.Web.Mvc`)

#### Controllers

| File | Path | Purpose |
|------|------|---------|
| `ProductsController.cs` | `Controllers/ProductsController.cs` | MVC Controller xử lý request, gọi AppService, trả về Views |
| `CategoriesController.cs` | `Controllers/CategoriesController.cs` | Controller cho Category |

#### Views (Razor)

| File | Path | Purpose |
|------|------|---------|
| `Index.cshtml` | `Views/Products/Index.cshtml` | View hiển thị danh sách sản phẩm với DataTable, modal form, filters |
| `Index.cshtml` | `Views/Categories/Index.cshtml` | View danh sách danh mục |

#### JavaScript

| File | Path | Purpose |
|------|------|---------|
| `Index.js` | `wwwroot/view-resources/Views/Products/Index.js` | Logic DataTable, Ajax calls, modal handling, validation |
| `Index.js` | `wwwroot/view-resources/Views/Categories/Index.js` | Logic cho Categories |

#### Navigation & Layout

| File | Path | Purpose |
|------|------|---------|
| `ProductManagerNavigationProvider.cs` | `Startup/` | Đăng ký menu sidebar (Products, Categories) |
| `bundles.json` | Root | Cấu hình bundle JavaScript/CSS |

---

## Dependency Flow

```
Web.Mvc Controller
       ↓ (injects)
IProductAppService (Interface)
       ↓ (implemented by)
ProductAppService
       ↓ (uses)
IRepository<Product> (ABP)
       ↓ (EF Core)
ProductManagerDbContext
       ↓ (SQL Server)
Database
```

---

## Key Features Implemented

1. **CRUD Operations** - Create, Read, Update, Delete cho Products và Categories
2. **Server-side Pagination** - Phân trang với DataTables
3. **Advanced Filtering** - Tìm theo keyword, danh mục, khoảng giá
4. **Authorization** - Phân quyền Create/Edit/Delete với ABP Attributes
5. **AutoMapper** - Tự động mapping giữa Entities và DTOs
6. **Modal Forms** - Thêm/Sửa dữ liệu không cần load lại trang
7. **Dynamic API Proxies** - ABP tự tạo JavaScript API clients
