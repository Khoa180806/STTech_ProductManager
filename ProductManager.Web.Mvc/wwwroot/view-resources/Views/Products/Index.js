(function () {
    $(function () {
        // Khởi tạo DataTable
        var _$productsTable = $('#ProductsTable');
        var _productService = abp.services.app.product;
        var _categoryService = abp.services.app.category;

        // Load categories cho dropdown
        function loadCategories() {
            _categoryService.getAll({ maxResultCount: 100 }).done(function (result) {
                var $categorySelect = $('#ProductCategoryId');
                var $filterCategory = $('#CategoryId');
                
                $categorySelect.empty().append('<option value="">-- Chọn danh mục --</option>');
                $filterCategory.empty().append('<option value="">-- Tất cả danh mục --</option>');
                
                $.each(result.items, function (index, category) {
                    $categorySelect.append('<option value="' + category.id + '">' + category.name + '</option>');
                    $filterCategory.append('<option value="' + category.id + '">' + category.name + '</option>');
                });
            }).fail(function (error) {
                console.error('Error loading categories:', error);
            });
        }

        // Load categories khi trang load
        loadCategories();

        var dataTable = _$productsTable.DataTable({
            paging: true,
            serverSide: true, // Phân trang server-side
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
                {
                    targets: 0,
                    data: "id"
                },
                {
                    targets: 1,
                    data: "name"
                },
                {
                    targets: 2,
                    data: "categoryName"
                },
                {
                    targets: 3,
                    data: "price",
                    render: function (data) {
                        return data.toLocaleString('vi-VN') + ' đ';
                    }
                },
                {
                    targets: 4,
                    data: "stockQuantity"
                },
                {
                    targets: 5,
                    data: null,
                    orderable: false,
                    render: function (data, type, row) {
                        var html = '';
                        if (abp.auth.isGranted('Pages.Products.Edit')) {
                            html += '<button class="btn btn-sm btn-warning edit-product" data-id="' + row.id + '"><i class="fas fa-edit"></i></button> ';
                        }
                        if (abp.auth.isGranted('Pages.Products.Delete')) {
                            html += '<button class="btn btn-sm btn-danger delete-product" data-id="' + row.id + '"><i class="fas fa-trash"></i></button>';
                        }
                        return html;
                    }
                }
            ]
        });

        // Nút tìm kiếm
        $('#SearchButton').click(function (e) {
            e.preventDefault();
            dataTable.ajax.reload();
        });

        // Nút thêm mới - mở modal
        $('#CreateNewProductButton').click(function () {
            $('#ProductModal').modal('show');
            $('#ProductForm')[0].reset();
            $('#ProductId').val('');
            $('#ProductModalLabel').text('Thêm sản phẩm mới');
        });

        // Nút đóng modal (cả nút "Đóng" và "X")
        $(document).on('click', '[data-dismiss="modal"]', function () {
            var modalId = $(this).closest('.modal').attr('id');
            $('#' + modalId).modal('hide');
        });

        // Nút sửa - mở modal với data
        $(document).on('click', '.edit-product', function () {
            var productId = $(this).data('id');
            _productService.get({ id: productId }).done(function (result) {
                $('#ProductModal').modal('show');
                $('#ProductId').val(result.id);
                $('#ProductName').val(result.name);
                $('#ProductDescription').val(result.description);
                $('#ProductPrice').val(result.price);
                $('#ProductStock').val(result.stockQuantity);
                $('#ProductCategoryId').val(result.categoryId);
                $('#ProductModalLabel').text('Sửa sản phẩm');
            }).fail(function (error) {
                abp.notify.error('Không thể tải thông tin sản phẩm');
                console.error(error);
            });
        });

        // Submit form (Thêm/Sửa)
        $('#SaveProductButton').click(function (e) {
            e.preventDefault();
            var $form = $('#ProductForm');

            if (!$form.valid()) {
                return;
            }

            var productId = $('#ProductId').val();
            var productData = {
                name: $('#ProductName').val(),
                description: $('#ProductDescription').val(),
                price: parseFloat($('#ProductPrice').val()),
                stockQuantity: parseInt($('#ProductStock').val()),
                categoryId: parseInt($('#ProductCategoryId').val())
            };

            abp.ui.setBusy('#ProductModal');

            if (productId) {
                // Update
                productData.id = parseInt(productId);
                _productService.update(productData).done(function () {
                    abp.notify.success('Cập nhật thành công');
                    $('#ProductModal').modal('hide');
                    dataTable.ajax.reload();
                }).fail(function (error) {
                    abp.notify.error(error.message || 'Có lỗi xảy ra khi cập nhật');
                    console.error('Update error:', error);
                }).always(function () {
                    abp.ui.clearBusy('#ProductModal');
                });
            } else {
                // Create
                _productService.create(productData).done(function () {
                    abp.notify.success('Thêm sản phẩm thành công');
                    $('#ProductModal').modal('hide');
                    dataTable.ajax.reload();
                }).fail(function (error) {
                    abp.notify.error(error.message || 'Có lỗi xảy ra khi thêm sản phẩm');
                    console.error('Create error:', error);
                }).always(function () {
                    abp.ui.clearBusy('#ProductModal');
                });
            }
        });

        // Nút xóa với xác nhận
        $(document).on('click', '.delete-product', function () {
            var productId = $(this).data('id');
            var productName = $(this).closest('tr').find('td:eq(1)').text();

            abp.message.confirm(
                'Bạn có chắc muốn xóa sản phẩm "' + productName + '"?',
                'Xác nhận xóa',
                function (isConfirmed) {
                    if (isConfirmed) {
                        _productService.delete({ id: productId }).done(function () {
                            abp.notify.success('Xóa thành công');
                            dataTable.ajax.reload();
                        }).fail(function (error) {
                            abp.notify.error('Xóa thất bại: ' + (error.message || 'Unknown error'));
                            console.error(error);
                        });
                    }
                }
            );
        });
    });
})();