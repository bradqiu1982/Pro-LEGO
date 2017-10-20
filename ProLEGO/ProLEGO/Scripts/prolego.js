var ProLego = function () {
    var wel_init = function(){
        // $('body').on('mouseenter', '.wel-op-btn-left', function(){
        //     if(!$('.main-body').hasClass('main-body-hover')){
        //         $('.main-body').addClass('main-body-hover');
        //     }
        // });
        // $('body').on('mouseleave', '.wel-op-btn-left', function(){
        //     if($('.main-body').hasClass('main-body-hover')){
        //         $('.main-body').removeClass('main-body-hover');
        //     }
        // });
        $('body').on('click', '.wel-op-btn-left', function(){

        });
    }

    var common = function () {
        $('body').on('mouseenter', '.search', function () {
            $('#keywords').removeClass('hidden');
            setTimeout(
                "if($('#keywords').val == '') $('#keywords').addClass('hidden')"
            , 5000);
        });

        $('body').on('mouseleave', '.search', function () {
            if ($('#keywords').val() == '') {
                $('#keywords').addClass('hidden');
            }
        })

        $('body').on('click', '.search-img', function () {
            //search
        });

        $('body').on('click', '.logo', function () {
            window.location.href = '/';
        });
    }

    var pro_list = function(){
        $('body').on('click', '.pro-detail-edit', function () {
            $(this).parent('.back').children('.pro-detail').children('.dot-img').removeClass('dot-img').addClass('dot-img-del');
            $(this).parent('.back').children('.pro-detail-end').children('.dot-img-end').removeClass('dot-img-end').addClass('dot-img-del');
            $(this).parent('.back').children('.pro-detail-end').removeClass('pro-detail-end').addClass('pro-detail');
            $(this).parent('.back').children('.pro-detail-add').removeClass('hidden');
            $(this).addClass('hidden');
            $(this).parent('.back').children('.pro-mes').children('.detail-content').addClass('hidden');
            $(this).parent('.back').children('.pro-mes').children('.edit-mes').removeClass('hidden');
        });

        $('body').on('click', '.dot-img-del', function(){
            $(this).parent('.pro-detail').addClass('hidden');
        });

        $('body').on('click', '.mes-type', function(){
            $('.mes-type').removeClass('mes-active').addClass('mes-inactive');
            $(this).removeClass('mes-inactive').addClass('mes-active');
        });

        $('body').on('click', '.project-edit-content', function () {
            var project_key = $(this).parent('div').prev('.front').children('div').eq(0).html();
            window.location.href = '/ProLEGO/ProjectDetail?ProjectName=' + project_key;
        });
    }

    var pro_detail = function(){
        $('.date').datepicker();
        $('.date').datepicker().on('changeDate', function (ev) {
            $('.date').datepicker('hide');
        });
        //$("#member_name").autoComplete({
        //    minChars: 0,
        //    source: function(term, suggest){
        //        term = term.toLowerCase();
        //        var choices = ['Test@Finisar.com', 'Test2@Finisar.com'];
        //        var suggestions = [];
        //        for (i=0;i<choices.length;i++)
        //            if (~choices[i].toLowerCase().indexOf(term)) suggestions.push(choices[i]);
        //        suggest(suggestions);
        //    }
        //})

        //$("#member_role").autoComplete({
        //    minChars: 0,
        //    source: function(term, suggest){
        //        term = term.toLowerCase();
        //        var choices = ['PQE', 'PE'];
        //        var suggestions = [];
        //        for (i=0;i<choices.length;i++)
        //            if (~choices[i].toLowerCase().indexOf(term)) suggestions.push(choices[i]);
        //        suggest(suggestions);
        //    }
        //});

        $('body').on('click', '.project-detail-edit', function(){
            $('.project-circle-img').removeClass('project-circle-img').addClass('project-del-img');
            $('.project-dot-img').removeClass('project-dot-img').addClass('project-add-img');
            $('.project-detail-edit').addClass('hidden');
            $('.project-detail-end').children('.project-dot-img-end').removeClass('hidden');
            $('.add-project-col').removeClass('hidden');
            $('.detail-show').addClass('hidden');
            $('.detail-edit').removeClass('hidden');
            $('.project-member').css('height', ($('.project-label-member').length + 3 ) * 20 +'px');
        });

        $('body').on('click', '.project-del-img', function(){
            $(this).removeClass('project-del-img').addClass('project-add-img');
        });

        $('body').on('click', '.project-add-img', function(){
            $(this).removeClass('project-add-img').addClass('project-del-img');
        });

        $('body').on('click', '.bool-def-left, .bool-def-right', function(){
            $('.bool-def-left, .bool-def-right').removeClass('bool-active').addClass('bool-inactive');
            $(this).removeClass('bool-inactive').addClass('bool-active');
        });

        $('body').on('click', '.edit-dot-del', function(){
            var project_key = $('#project_key').html();
            var member_name = $(this).parent('.detail-edit').parent('.project-label-member').children().eq(1).html();
            var member_role = $(this).parent('.detail-edit').parent('.project-label-member').children().eq(2).html();
            var sub_cnt = $(this).parent('.detail-edit').parent('.project-label-member').parent('.project-detail-content').find('.project-label-member').length;
            $(this).parent('.detail-edit').parent('.project-label-member').parent('.project-detail-content').parent('.project-detail-mid').parent('.project-detail').css('height', ( sub_cnt + 2 ) * 20 +'px');
            $(this).parent('.detail-edit').parent('.project-label-member').remove();
            // $.post('/',
            // {
            //     project_key: project_key,
            //     member_name: member_name,
            //     member_role: member_role
            // }, function(output){
            //     if(output.success){
            //         $(this).parent('.detail-edit').parent('.project-label-member').empty();
            //     }
            //     else{
            //         alert("Failed to remove member!");
            //     }
            // })
        });

        $('body').on('click', '.edit-add-img', function(){
            var member_name = $(this).prev('input').val();
            if( ! member_name){
                return false;
            }
            var $project_label_content = $(this).parent('.detail-edit').parent('.project-detail-content');
            var flg = false;
            $project_label_content.find('.project-label-member').each(function () {
                if(member_name == $(this).children('span').eq(1).html()){
                    flg = true;
                }
            });
            if(flg){
                return false;
            }
            $project_label_content.parent('.project-detail-mid').parent('.project-detail')
                    .css('height', ($project_label_content.find('.project-label-member').length + 4) * 20 + 'px');
            var appendStr = '<div class="project-label-member">'+
                    '<span class="detail-edit">'+
                        '<img src="/Content/images/dot_del.png" class="edit-dot-del">' +
                    '</span>'+
                    '<span>'+member_name+'</span>'+
                '</div>';
            $(appendStr).insertBefore($(this).parent('.detail-edit'));
        });

        //save
        $('body').on('click', '#btn_detail_save', function(){
            var values = new Array();
            var project_key = $('#project_key').html();
            //var project_des = $('.project-key').find('input').val();
            $('.project-detail').each(function(){
                var arr_tmp = new Array();
                var dis_flg = ($(this).find('.project-del-img').length) ? 1 : 0;
                var col_name = $(this).find('.project-detail-title').html();
                var col_val = '';
                var col_type = $(this).attr('data-col-type');
                if(col_type == 'ROLE'){
                    $(this).find('.project-label-member').each(function(){
                        col_val += $(this).children('span').eq(1).html() + ';';
                    })
                }
                else if(col_type == 'BOOL'){
                    col_val = $(this).find('.bool-active').html();
                }
                else{
                    col_val = $(this).find('input').val();
                }
                arr_tmp.push(col_type, dis_flg, col_name, col_val);
                values.push(arr_tmp);
            });
            $.post('/ProLEGO/SaveProjectData',
            {
                project_key: project_key,
                //project_des: project_des,
                data: JSON.stringify(values)
            }, function(output){
                if(output.success){
                    window.location.reload();
                }
                else{
                    alert('Failed to Save!');
                }
            });
        });
    }

    return {
        init: function () {
            wel_init();
            common();
        },
        pro_list: function(){
            pro_list();
            common();
        },
        pro_detail: function(){
            pro_detail();
            common();
        }
    };
}();