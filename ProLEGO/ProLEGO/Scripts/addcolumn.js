var AddColumn = function () {
    var common = function () {
        $('body').on('click', '.logo', function () {
            window.location.href = '/';
        });
    }
    var add_column = function(){
        $('.date').datepicker();
        $('.date').datepicker().on('changeDate', function (ev) {
            //modify date value
            if ( ! $(this).hasClass('add-date')) {
                var col_val = $(this).find('input').val();
                var col_name = $(this).parent('.project-detail-content').next().next().html();

                //add default date
                $.post('/ProLEGO/UpdateDateDefColumnValue',
                {
                    col_value: col_val,
                    col_name: col_name
                }, function (output) {
                    if (output.success) {
                        window.location.reload();
                    }
                    else {
                        alert('Failed to set default date!');
                    }
                });
            }
            $('.date').datepicker('hide');
        });


        $('body').on('click', '.project-del-img', function () {
            $(this).removeClass('project-del-img').addClass('project-add-img');
            var column_name = $(this).next('div').find('.project-detail-title').html();
            var dis_flg = 0;
            modify_dis_flg(column_name, dis_flg);
        });

        $('body').on('click', '.project-add-img', function () {
            $(this).removeClass('project-add-img').addClass('project-del-img');
            var column_name = $(this).next('div').find('.project-detail-title').html();
            var dis_flg = 1;
            modify_dis_flg(column_name, dis_flg);
        });

        //whole display
        function modify_dis_flg(column_name, dis_flg) {
            $.post('/ProLEGO/DisplayPlatformCol',
            {
                column_name: column_name,
                dis_play: dis_flg
            }, function (output) {
                if (output.success) {
                    window.location.reload();
                }
                else {
                    alert('Failed to set!');
                }
            });
        }

        $('body').on('click', '.bool-def-left, .bool-def-right', function(){
            $(this).parent('.div-span').children('span').removeClass('bool-active').addClass('bool-inactive');
            $(this).removeClass('bool-inactive').addClass('bool-active');
            if($(this).parent('.div-span').hasClass('bool-def')){
                $('.info-up').removeClass('info-up').addClass('addpro-dot-img');
                $(this).parent('.bool-def').parent('.project-detail-mid').prev('.addpro-dot-img')
                        .removeClass('addpro-dot-img').addClass('info-up');
                $(this).parent('.bool-def').find('input').val($(this).html());
                $(this).parent('.col-name').css('border-bottom', '0 #939393 solid').children('span').addClass('hidden');
                $(this).parent('.col-name').children('div').removeClass('hidden');
                $(this).parent('.col-val').children('span').addClass('hidden');
                $(this).parent('.col-val').children('div').removeClass('hidden');
            }
            else {
                var col_val = $(this).parent('.detail-edit').find('.bool-active').html();
                var col_name = $(this).parent('.detail-edit').parent('.project-detail-content').next().next().html();
                $.post('/ProLEGO/UpdateDateDefColumnValue', {
                    col_value: col_val,
                    col_name: col_name
                }, function (output) {
                    if (output.success) {
                        window.location.reload();
                    }
                    else {
                        alert("Failed to change!");
                    }
                });
            }
        });

        $('body').on('click', '.span-col-val, .span-col-name', function(){
            $('.info-up').removeClass('info-up').addClass('addpro-dot-img');
            $('.info-up-end').removeClass('info-up-end').addClass('addpro-dot-img-end');
            $(this).parent('.span-column').parent('.project-detail-mid').prev('.addpro-dot-img')
                    .removeClass('addpro-dot-img').addClass('info-up');
            batch_update_css();
            $(this).parent('.col-name').css('border-bottom', '0 #939393 solid').children('span').addClass('hidden');
            $(this).parent('.col-name').children('div').removeClass('hidden');
            $(this).parent('.col-val').children('span').addClass('hidden');
            $(this).parent('.col-val').children('div').removeClass('hidden');
        });

        function batch_update_css(){
            //col-name
            $('.col-name').css('border-bottom','1px #939393 solid').children('span').removeClass('hidden');
            $('.col-name').children('div').addClass('hidden');
            //col-val
            $('.col-val').children('div').children('input').each(function () {
                if($(this).val()){
                    $(this).parent('div').prev('span').html($(this).val());
                }
                else {
                    $(this).parent('div').prev('span').html($(this).parent('div').prev('span').attr('data-def'));
                }
            });
            $('.col-val').children('span').removeClass('hidden');
            $('.col-val').children('div').addClass('hidden');
        }

        //delete default value
        $('body').on('click', '.edit-dot-del', function () {
            var $project_detail_content = $(this).parent('.project-label').parent('.project-detail-content');
            var col_val  = $(this).next('span').html();
            var col_name = $project_detail_content.next().next().html();
            var sub_cnt  = $project_detail_content.find('.project-label').length;
            $project_detail_content.parent('.project-detail-mid')
                .parent('.project-detail').css('height', (sub_cnt + 2) * 20 + 'px');
            $(this).parent('.project-label').remove();
            $.post('/ProLEGO/RemoveDefColumnValue',
            {
                col_val: col_val,
                col_name: col_name
            }, function (output) {
                if (output.success) {
                    window.location.reload();
                }
                else {
                    alert("Failed to remove member!");
                }
            });
        });

        //add default value
        $('body').on('click', '.edit-add-img', function(){
            var add_value = $(this).prev('input').val();
            if (!add_value) {
                return false;
            }
            var $col_value = $(this).parent('.detail-edit').parent('.project-detail-content');
            var flg = false;
            $col_value.find('.project-label').each(function () {
                if (add_value == $(this).children('span').eq(0).html()) {
                    flg = true;
                }
            });
            if (flg) {
                return false;
            }
            var col_name = $col_value.next().next().html();
            $.post('/ProLEGO/AddDefColumnValue',
            {
                col_val: add_value,
                col_name: col_name
            }, function (output) {
                if (output.success) {
                    window.location.reload();
                }
                else {
                    alert('Failed to add!');
                }
            });
        });

        //add column name
        //$('body').on('click', '.add-column', function(){
        //    var column_name = $(this).prev('input').prev('input').val();
        //    $(this).prev('input').val(column_name);
        //    $(this).parent('div').addClass('hidden');
        //    //default value
        //    var $col_val = $(this).parent('.input-group').parent('.col-name').prev('.col-val');
        //    $col_val.children('span').addClass('hidden');
        //    $col_val.children('div').removeClass('hidden');
        //    column_name = (column_name != '') ? column_name : $(this).parent('.input-group').prev('span').attr('data-def');
        //    $(this).parent('.input-group').parent('.col-name').css('border-bottom','1px #939393 solid')
        //            .children('span').removeClass('hidden').html(column_name);

        //});


        $('body').on('blur', '#col_name_info, #col_name_name, #col_name_bool, #col_name_date', function () {
            var column_name = $(this).val();
            $(this).next('input').val(column_name);
            $(this).parent('div').addClass('hidden');
            //default value
            var $col_val = $(this).parent('.input-group').parent('.col-name').prev('.col-val');
            $col_val.children('span').addClass('hidden');
            $col_val.children('div').removeClass('hidden');
            column_name = (column_name != '') ? column_name : $(this).parent('.input-group').prev('span').attr('data-def');
            $(this).parent('.input-group').parent('.col-name').css('border-bottom', '1px #939393 solid')
                    .children('span').removeClass('hidden').html(column_name);
        });

        //add column
        $('body').on('click', '.info-up', function () {
            var col_type = $(this).attr('data-col-type');
            var dis_flg = 1;
            var col_name = $(this).next('div').children('span').eq(1).find('input').eq(1).val();
            var col_value = $(this).next('div').children('span').eq(0).find('input').val();
            if (!col_name) {
                return false;
            }
            var flg = false;
            $('.project-detail').each(function(){
                if (col_name == $(this).find('.project-detail-title').html()) {
                    flg = true;
                }
            });
            if(flg){
                return false;
            }

            //add column
            $.post('/ProLEGO/AddProColumn', {
                col_type: col_type,
                dis_flg: dis_flg,
                col_name: col_name,
                col_value: col_value
            }, function(output){
                if (output.success) {
                    window.location.reload();
                }
                else {
                    alert('Failed to add column!');
                }
            });
        });
    }
    return {
        init: function(){
            add_column();
            common();
        }
    };
}();