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
    var pro_list = function(){
        $('body').on('click', '.pro-detail-edit', function(){
            $(this).parent('.back').children('.pro-detail')
                .children('.dot-img').removeClass('dot-img').addClass('dot-img-del');
            $(this).parent('.back').children('.pro-detail-end')
                .removeClass('pro-detail-end').addClass('pro-detail').children('.dot-img-end')
                    .removeClass('dot-img-end').addClass('dot-img-del');
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
    }
    var show = function () {
        $('#role-list').autoComplete({
            minChars: 1,
            source: function (term, suggest) {
                term = term.toLowerCase();
                $.post('/ProLego/RoleList',
                {

                }, function (output){
                    var suggestions = [];
                    for (i = 0; i < output.length; i++) {
                        if (~output[i].toLowerCase().indexOf(term)) {
                            suggestions.push(output[i]);
                        }
                    }
                    suggest(suggestions);
                });
            }
        });

        $('body').on('click', '.add_role', function () {
            var role = $('#new_role').val();
            if (role != '') {
                $.post('/',
                {
                    role: role
                }, function (output) {
                    if (output.success) {
                        //bind dropdownlist

                    }
                    else {
                        alert('Failed to add role !');
                    }
                });
            }
            else {
                alert('Please input role !');
            }
        });

        $('body').on('click', '.add_member', function () {
            var project_key = $('#pro_list').val();
            var role = $('#role_list').val();
            var members = $('#members').val();
            if (project_key == '') {
                alert('Please select project !');
                return false;
            }
            if (role == '') {
                alert('Please select role !');
                return false;
            }
            if (members == '') {
                alert('Please at least add one member !');
                return false;
            }
            $.post('/',
            {
                project_key: project_key,
                role: role,
                members: members
            }, function (output) {
                if (output.success) {
                    //update project member list
                }
                else {
                    alert('Failed to add members !');
                }
            })
        });
        $('body').on('click', '.delete', function () {
            if (!confirm('Do you really want to delete this member ?')) {
                return false;
            }
            var member = $(this).attr('data-name');
            var project_key = $(this).attr('data-prokey');
            var role = $(this).attr('data-role');
            if (member && project_key && role) {
                $.post('/',
                {
                    member: member,
                    project_key: project_key,
                    role: role
                }, function (output) {
                    if (output.success) {
                        //remove this member
                    }
                    else {
                        alert('Failed to remove this member !');
                    }
                })
            }
        });
        $('body').on('click', '.export', function () {

        });
    }

    return {
        init: function () {
            wel_init();
        },
        pro_list: function(){
            pro_list();
        }
    };
}();