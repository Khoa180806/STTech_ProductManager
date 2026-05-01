(function () {
    $(function () {
        var _$categoriesTable = $('#CategoriesTable');
        var _categoryService = abp.services.app.category;

        var dataTable = _$categoriesTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _categoryService.getAll
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
                    data: "description"
                },
                {
                    targets: 3,
                    data: null,
                    orderable: false,
                    render: function (data, type, row) {
                        var html = '';
                        if (abp.auth.isGranted('Pages.Products.Edit')) {
                            html += '<button class="btn btn-sm btn-warning edit-category" data-id="' + row.id + '"><i class="fas fa-edit"></i></button> ';
                        }
                        if (abp.auth.isGranted('Pages.Products.Delete')) {
                            html += '<button class="btn btn-sm btn-danger delete-category" data-id="' + row.id + '"><i class="fas fa-trash"></i></button>';
                        }
                        return html;
                    }
                }
            ]
        });

        // Thêm mới
        $('#CreateNewCategoryButton').click(function () {
            $('#CategoryModal').modal('show');
            $('#CategoryForm')[0].reset();
            $('#CategoryId').val('');
            $('#CategoryModalLabel').text('Thêm danh mục');
        });

        // Nút đóng modal
        $(document).on('click', '[data-dismiss="modal"]', function () {
            var modalId = $(this).closest('.modal').attr('id');
            $('#' + modalId).modal('hide');
        });

        // Sửa
        $(document).on('click', '.edit-category', function () {
            var categoryId = $(this).data('id');
            _categoryService.get({ id: categoryId }).done(function (result) {
                $('#CategoryModal').modal('show');
                $('#CategoryId').val(result.id);
                $('#CategoryName').val(result.name);
                $('#CategoryDescription').val(result.description);
                $('#CategoryModalLabel').text('Sửa danh mục');
            }).fail(function (error) {
                abp.notify.error('Không thể tải thông tin danh mục');
                console.error(error);
            });
        });

        // Lưu
        $('#SaveCategoryButton').click(function (e) {
            e.preventDefault();
            var $form = $('#CategoryForm');

            if (!$form.valid()) {
                return;
            }

            var categoryId = $('#CategoryId').val();
            var categoryData = {
                name: $('#CategoryName').val(),
                description: $('#CategoryDescription').val()
            };

            abp.ui.setBusy('#CategoryModal');

            if (categoryId) {
                categoryData.id = parseInt(categoryId);
                _categoryService.update(categoryData).done(function () {
                    abp.notify.success('Cập nhật thành công');
                    $('#CategoryModal').modal('hide');
                    dataTable.ajax.reload();
                }).fail(function (error) {
                    abp.notify.error(error.message || 'Có lỗi xảy ra');
                }).always(function () {
                    abp.ui.clearBusy('#CategoryModal');
                });
            } else {
                _categoryService.create(categoryData).done(function () {
                    abp.notify.success('Thêm danh mục thành công');
                    $('#CategoryModal').modal('hide');
                    dataTable.ajax.reload();
                }).fail(function (error) {
                    abp.notify.error(error.message || 'Có lỗi xảy ra');
                }).always(function () {
                    abp.ui.clearBusy('#CategoryModal');
                });
            }
        });

        // Xóa
        $(document).on('click', '.delete-category', function () {
            var categoryId = $(this).data('id');
            abp.message.confirm(
                'Bạn có chắc muốn xóa danh mục này?',
                'Xác nhận xóa',
                function (isConfirmed) {
                    if (isConfirmed) {
                        _categoryService.delete({ id: categoryId }).done(function () {
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
