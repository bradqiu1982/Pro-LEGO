var AddColumn = function () {
    var add_column = function(){
        $('.date').datepicker();
        $('#project_type').autoComplete({
            minChars: 0,
            source: function(term, suggest){
                term = term.toLowerCase();
                var choices = ['Parallel', 'Test'];
                var suggestions = [];
                for (i=0;i<choices.length;i++)
                    if (~choices[i].toLowerCase().indexOf(term)) suggestions.push(choices[i]);
                suggest(suggestions);
            }
            // function (term, suggest) {
            //     term = term.toLowerCase();
            //     $.post('/ProLego/RoleList',
            //     {

            //     }, function (output){
            //         var suggestions = [];
            //         for (i = 0; i < output.length; i++) {
            //             if (~output[i].toLowerCase().indexOf(term)) {
            //                 suggestions.push(output[i]);
            //             }
            //         }
            //         suggest(suggestions);
            //     });
            // }
        });

        $("#member_name").autoComplete({
            minChars: 0,
            source: function(term, suggest){
                term = term.toLowerCase();
                var choices = ['Test@Finisar.com', 'Test2@Finisar.com'];
                var suggestions = [];
                for (i=0;i<choices.length;i++)
                    if (~choices[i].toLowerCase().indexOf(term)) suggestions.push(choices[i]);
                suggest(suggestions);
            }
        })

        $("#member_role").autoComplete({
            minChars: 0,
            source: function(term, suggest){
                term = term.toLowerCase();
                var choices = ['PQE', 'PE'];
                var suggestions = [];
                for (i=0;i<choices.length;i++)
                    if (~choices[i].toLowerCase().indexOf(term)) suggestions.push(choices[i]);
                suggest(suggestions);
            }
        });

        $("#col_name_date").autoComplete({
            minChars: 0,
            source: function(term, suggest){
                term = term.toLowerCase();
                var choices = ['Start Date', 'Due Date'];
                var suggestions = [];
                for (i=0;i<choices.length;i++)
                    if (~choices[i].toLowerCase().indexOf(term)) suggestions.push(choices[i]);
                suggest(suggestions);
            }
        });

        $('body').on('click', '.project-del-img', function(){
            $(this).removeClass('project-del-img').addClass('project-add-img');
        })
        $('body').on('click', '.project-add-img', function(){
            $(this).removeClass('project-add-img').addClass('project-del-img');
        })

        $('body').on('click', '.bool-def-left, .bool-def-right', function(){
            $(this).parent('.div-span').children('span').removeClass('bool-active').addClass('bool-inactive');
            $(this).removeClass('bool-inactive').addClass('bool-active');
            if($(this).parent('.div-span').hasClass('bool-def')){
                $('.info-up').removeClass('info-up').addClass('addpro-dot-img');
                $(this).parent('.bool-def').parent('.project-detail-mid').prev('.addpro-dot-img')
                        .removeClass('addpro-dot-img').addClass('info-up');
                $(this).parent('.bool-def').find('input').val($(this).html());
                $(this).parent('.col-name').css('border-bottom', 'unset').children('span').addClass('hidden');
                $(this).parent('.col-name').children('div').removeClass('hidden');
                $(this).parent('.col-val').children('span').addClass('hidden');
                $(this).parent('.col-val').children('div').removeClass('hidden');
            }
        });

        $('body').on('click', '.span-col-val, .span-col-name', function(){
            $('.info-up').removeClass('info-up').addClass('addpro-dot-img');
            $('.info-up-end').removeClass('info-up-end').addClass('addpro-dot-img-end');
            $(this).parent('.span-column').parent('.project-detail-mid').prev('.addpro-dot-img')
                    .removeClass('addpro-dot-img').addClass('info-up');
            batch_update_css();
            $(this).parent('.col-name').css('border-bottom', 'unset').children('span').addClass('hidden');
            $(this).parent('.col-name').children('div').removeClass('hidden');
            $(this).parent('.col-val').children('span').addClass('hidden');
            $(this).parent('.col-val').children('div').removeClass('hidden');
        });

        function batch_update_css(){
            //col-name
            $('.col-name').css('border-bottom','1px #939393 solid').children('span').removeClass('hidden');
            $('.col-name').children('div').addClass('hidden');
            //col-val
            $('.col-val').children('div').children('input').each(function(){
                if($(this).val()){
                    $(this).parent('div').prev('span').html($(this).val());
                }
            });
            $('.col-val').children('span').removeClass('hidden');
            $('.col-val').children('div').addClass('hidden');
        }

        $('body').on('click', '.edit-dot-del', function(){
            var col_id = $(this).parent('.project-label').parent('.project-detail-content').attr('data-col-id');
            var sub_cnt = $(this).parent('.project-label').parent('.project-detail-content').find('.project-label').length;
            $(this).parent('.project-label').parent('.project-detail-content').parent('.project-detail-mid')
                .parent('.project-detail').css('height', ( sub_cnt + 2 ) * 20 +'px');
            $(this).parent('.project-label').remove();
            // $.post('/',
            // {
            //     col_id: col_id
            // }, function(output){
            //     if(output.success){
            //         $(this).parent('.detail-edit').parent('.project-label').empty();
            //     }
            //     else{
            //         alert("Failed to remove member!");
            //     }
            // })
        });

        $('body').on('click', '.edit-add-img', function(){
            var add_value = $(this).prev('input').val();
            if( ! add_value){
                return false;
            }
            var $col_value = $(this).parent('.detail-edit').parent('.project-detail-content');
            var flg = false;
            $col_value.find('.project-label').each(function(){
                if(add_value == $(this).children('span').eq(0).html()){
                    flg = true;
                }
            });
            if(flg){
                return false;
            }
            var appendStr = '<div class="project-label">'+
                '<img src="images/dot_del.png" class="edit-dot-del">'+
                '<span>'+add_value+'</span>'+
            '</div>';
            $col_value.parent('.project-detail-mid').parent('.project-detail')
                .css('height', ($col_value.find('.project-label').length + 4 ) * 20 +'px');
            $(appendStr).insertBefore($(this).parent('.detail-edit'));
        });

        //add column name
        $('body').on('click', '.add-column', function(){
            var column_name = $(this).prev('input').prev('input').val();
            if(column_name != ''){
                $(this).prev('input').val(column_name);
                $(this).parent('.input-group').parent('.col-name').css('border-bottom','1px #939393 solid')
                    .children('span').removeClass('hidden').html(column_name);
                $(this).parent('div').addClass('hidden');
                //default value
                var $col_val = $(this).parent('.input-group').parent('.col-name').prev('.col-val');
                $col_val.children('span').addClass('hidden');
                $col_val.children('div').removeClass('hidden');
            }
        });

        //add column
        $('body').on('click', '.info-up', function(){
            var column_name = $(this).next('div').children('span').eq(1).find('input').eq(1).val();
            var column_value = $(this).next('div').children('span').eq(0).find('input').val();
            if( ! column_name || ! column_value){
                return false;
            }
            var flg = false;
            $('.project-detail').each(function(){
                if(column_name == $(this).find('.project-detail-title').html()){
                    flg = true;
                }
            });
            if(flg){
                return false;
            }
            var sub_appendStr = '';
            var col_type = $(this).attr('data-col-type');
            if(col_type == 'Date'){
                sub_appendStr = '<div class="input-group date" data-date="'+column_value+'" data-date-format="yyyy-mm-dd">'+
                    '<input type="text" class="form-control" value="'+column_value+'" readonly>'+
                    '<span class="input-group-addon">'+
                        '<span class="glyphicon glyphicon-th"></span>'+
                    '</span>'+
                '</div>';
            }
            else if(col_type == 'Bool'){
                if(column_value == 'True'){
                    sub_appendStr = '<div class="detail-edit edit-bool-def div-span">'+
                        '<span class="bool-def-left bool-active">True</span>'+
                        '<span class="bool-def-right bool-inactive" style="margin-left: 0;">False</span>'+
                    '</div>';
                }
                else{
                    sub_appendStr = '<div class="detail-edit edit-bool-def div-span">'+
                        '<span class="bool-def-left bool-inactive">True</span>'+
                        '<span class="bool-def-right bool-active" style="margin-left: 0;">False</span>'+
                    '</div>';
                }
            }
            else if(col_type == 'Role'){
                sub_appendStr = '<div class="project-label">'+
                    '<img src="images/dot_del.png" class="edit-dot-del">'+
                    '<span>'+column_value+'</span>'+
                '</div>'+
                '<div class="detail-edit">'+
                    '<input type="text" class="form-control autocomplete-input"'+
                        'autocomplete="off" placeholder="Member Role" />'+
                    '<div class="edit-add-img"></div>'+
                '</div>';
            }
            else if(col_type == 'Info'){
                sub_appendStr = '<input type="text" class="form-control" value="'+column_value+'">';
            }
            var appendStr = '<div class="project-detail">'+
                '<div class="project-del-img"></div>'+
                '<div class="project-detail-mid">'+
                    '<div class="project-detail-content">'+
                        sub_appendStr +
                    '</div>'+
                    '<div class="project-detail-line"></div>'+
                    '<div class="project-detail-title" style="margin-left: 0;">'+column_name+'</div>'+
                    '<div class="project-title-circle"></div>'+
                '</div>'+
            '</div>';
            $(appendStr).insertBefore($('.add-project-col'));
            $('.date').datepicker();
            // $.post('/', {

            // }, function(output){
            //     if(output.success){
            //         $(appendStr).insertBefore($('.add-project-col'));
            //     }
            //     else{
            //         alert('Faled to add column!');
            //     }
            // });
        });
    }
    return {
        init: function(){
            add_column();
        }
    };
}();