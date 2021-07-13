function getUrlParameter(sParam) {
    var sPageURL = window.location.search.substring(1),
        sURLVariables = sPageURL.split('&'),
        sParameterName,
        i;

    for (i = 0; i < sURLVariables.length; i++) {
        sParameterName = sURLVariables[i].split('=');

        if (sParameterName[0] === sParam) {
            return sParameterName[1] === undefined ? true : decodeURIComponent(sParameterName[1]);
        }
    }
}

function SendFiles(inputProfileId, controller, actionIncluir, actionUpload, actionMontarTela, editar = false) {
    controller = '../' + controller + '/';

    let inputFile = $('#inputFile');
    let inputProfile = $('#' + inputProfileId); //$('#imgExemplo');
    let buttonSubmit = $('.btnSubmit');
    let buttonReturn = $('.btnReturn');
    let filesContainer = $('#myFiles');
    let files = [];

    inputFile.change(function () {
        let newFiles = [];
        for (let index = 0; index < inputFile[0].files.length; index++) {
            let file = inputFile[0].files[index];
            newFiles.push(file);
            files.push(file);
        }

        newFiles.forEach(file => {
            let fileElement = $(`<tr><td>${file.name}</td><td><span class="tbl-link fa-lg fa fa-file"></span></td><td><span class="tbl-link fa-lg fa fa-trash-o"></span></td></tr>`);
            fileElement.data('fileData', file);
            $('.dataTables_empty').parent().remove();
            filesContainer.append(fileElement);

            fileElement.click(function (event) {
                let fileElement = $(event.target);
                let indexToRemove = files.indexOf(fileElement.data('fileData'));
                fileElement.parent().parent().remove();
                files.splice(indexToRemove, 1);
            });
        });
    });

    inputProfile.change(function () {
        if ($('#profile').length == 0) {
            let newFiles = [];
            for (let index = 0; index < inputProfile[0].files.length; index++) {
                let file = inputProfile[0].files[index];
                newFiles.push(file);
                files.push(file);
            };

            newFiles.forEach(file => {
                let fileElement = $(`<tr><td id="profile">${file.name}</td><td><span class="tbl-link fa-lg fa fa-user-circle"></span></td><td class="td-one-action"><span class="tbl-link fa-lg fa fa-trash-o"></span></td></tr>`);
                fileElement.data('fileData', file);
                $('.dataTables_empty').parent().remove()
                filesContainer.prepend(fileElement);

                fileElement.click(function (event) {
                    let fileElement = $(event.target);
                    let indexToRemove = files.indexOf(fileElement.data('fileData'));
                    fileElement.parent().parent().remove();
                    files.splice(indexToRemove, 1);
                });
            });
        } else {
            alert('Foto de perfil ja inclusa');
        }
    });

    buttonReturn.click(function () {
        actionMontarTela = actionIncluir;
        buttonSubmit.click();
    });

    buttonSubmit.click(function () {
        //toastr.success('Inclusão em andamento!')

        let formData = new FormData();

        files.forEach(file => {
            formData.append('files', file);
        });

        console.log('Sending...');

        $.ajax({
            url: controller + actionIncluir //'../Exemplo/IncluirExemplo'
            , async: false
            , data: $('#pwd-container1').serializeArray()
            , type: 'POST'
            , success: function (r) {
                if ($('#profile').length == 0) {
                    formData.append('perfil', 0);
                } else {
                    formData.append('perfil', 1);
                }

                if (editar || r.id != undefined) {
                    if (files.length > 0) {
                        $.ajax({
                            url: controller + actionUpload //'../Exemplo/UploadFileExemplo_Inclusao'
                            , async: false
                            , data: formData
                            , type: 'POST'
                            , success: function (data) {
                                if (getUrlParameter('voltaCliente') == "1") {
                                    window.open('../Atendimento/IncluirAtendimento', '_self');
                                } else if (getUrlParameter('voltaCompra') == "1") {
                                    window.open('../Compra/MontarTelaPedidoCompra', '_self');
                                } else {
                                    if (editar) {
                                        window.open(controller + actionMontarTela + '/' + r.id, '_self');
                                    } else {
                                        window.open(controller + actionMontarTela, '_self');
                                    }
                                }
                            } //'../Exemplo/MontarTelaExemplo'
                            , error: function (data) { console.log('ERROR!!'); }
                            , cache: false
                            , processData: false
                            , contentType: false
                        });
                    } else {
                        if (getUrlParameter('voltaCliente') == "1") {
                            window.open('../Atendimento/IncluirAtendimento', '_self');
                        } else if (getUrlParameter('voltaCompra') == "1") {
                            window.open('../Compra/MontarTelaPedidoCompra', '_self');
                        } else {
                            if (editar && r.id != undefined) {
                                window.open(controller + actionMontarTela + '/' + r.id, '_self');
                            } else if (r.error != undefined) {
                                $('.ibox-content').find('.alert').remove();
                                $('.ibox-content').prepend(
                                    '<div class="alert alert-danger">'
                                    + '<span>' + r.error + '</span>'
                                    + '<button type="button" class="close" data-dismiss="alert">Fechar</button>'
                                    + '</div>');
                            } else {
                                window.open(controller + actionMontarTela, '_self');
                            }
                        }
                    }
                }
                else {
                    if (r.error == undefined) {
                        var alert = $(r).find('.alert');
                        $('.ibox-content').find('.alert').remove();
                        $('.ibox-content').prepend(alert);
                    }
                    else {
                        $('.ibox-content').find('.alert').remove();
                        $('.ibox-content').prepend(
                            '<div class="alert alert-danger">'
                            + '<span>' + r.error + '</span>'
                            + '<button type="button" class="close" data-dismiss="alert">Fechar</button>'
                            + '</div>');
                    }
                }
            }
        });
    });
}


function SendFilesV2(inputProfileId, controller, actionUpload) {
    controller = '../' + controller + '/';

    let inputFile = $('#inputFile');
    let inputProfile = $('#' + inputProfileId); //$('#imgExemplo');
    let buttonSubmit = $('.btnSubmit');
    let buttonReturn = $('.btnReturn');
    let filesContainer = $('#myFiles');
    let files = [];

    inputFile.change(function () {
        let newFiles = [];
        for (let index = 0; index < inputFile[0].files.length; index++) {
            let file = inputFile[0].files[index];
            newFiles.push(file);
            files.push(file);
        }

        newFiles.forEach(file => {
            let fileElement = $(`<tr><td>${file.name}</td><td><span class="tbl-link fa-lg fa fa-file"></span></td><td><span class="tbl-link fa-lg fa fa-trash-o"></span></td></tr>`);
            fileElement.data('fileData', file);
            $('.dataTables_empty').parent().remove();
            filesContainer.append(fileElement);

            fileElement.click(function (event) {
                let fileElement = $(event.target);
                let indexToRemove = files.indexOf(fileElement.data('fileData'));
                fileElement.parent().parent().remove();
                files.splice(indexToRemove, 1);
            });
        });
    });

    var profileName = "";

    inputProfile.change(function () {
        if ($('#profile').length == 0) {
            let newFiles = [];
            for (let index = 0; index < inputProfile[0].files.length; index++) {
                let file = inputProfile[0].files[index];
                newFiles.push(file);
                files.push(file);
            };

            newFiles.forEach(file => {
                let fileElement = $(`<tr><td id="profile">${file.name}</td><td><span class="tbl-link fa-lg fa fa-user-circle"></span></td><td class="td-one-action"><span class="tbl-link fa-lg fa fa-trash-o"></span></td></tr>`);
                fileElement.data('fileData', file);
                $('.dataTables_empty').parent().remove()
                filesContainer.prepend(fileElement);

                profileName = file.name;

                fileElement.click(function (event) {
                    let fileElement = $(event.target);
                    let indexToRemove = files.indexOf(fileElement.data('fileData'));
                    fileElement.parent().parent().remove();
                    files.splice(indexToRemove, 1);
                });
            });
        } else {
            alert('Foto de perfil ja inclusa');
        }
    });

    buttonReturn.click(function () {
        $.ajax({
            url: controller + 'FlagContinua'
            , async: true
            , type: 'POST'
            , cache: false
            , processData: false
            , contentType: false
        });

        buttonSubmit.click();
    });

    buttonSubmit.click(function () {
        //toastr.success('Inclusão em andamento!')
        buttonSubmit.prop('disabled', true);

        let formData = new FormData();

        if (files.length > 0) {
            files.forEach(file => {
                formData.append('files', file);
            });

            if ($('#profile').length != 0) {
                formData.append('profile', profileName);
            }

            $.ajax({
                url: controller + actionUpload
                , async: false
                , data: formData
                , type: 'POST'
                , error: function (data) {
                    buttonSubmit.prop('disabled', false);
                    console.log('ERROR!!');
                }
                , cache: false
                , processData: false
                , contentType: false
            });
        }

        $('#submit').click();
    });
}

function SendFilesUsuario(inputProfileId, controller, actionIncluir, actionUpload, actionMontarTela, editar = false) {
    controller = '../' + controller + '/';

    let inputFile = $('#inputFile');
    let inputProfile = $('#' + inputProfileId); //$('#imgExemplo');
    let buttonSubmit = $('.btnSubmit');
    let filesContainer = $('#myFiles');
    let files = [];

    inputFile.change(function () {
        let newFiles = [];
        for (let index = 0; index < inputFile[0].files.length; index++) {
            let file = inputFile[0].files[index];
            newFiles.push(file);
            files.push(file);
        }

        newFiles.forEach(file => {
            let fileElement = $(`<tr><td>${file.name}</td><td><span class="tbl-link fa-lg fa fa-file"></span></td><td><span class="tbl-link fa-lg fa fa-trash-o"></span></td></tr>`);
            fileElement.data('fileData', file);
            $('.dataTables_empty').parent().remove();
            filesContainer.append(fileElement);

            fileElement.click(function (event) {
                let fileElement = $(event.target);
                let indexToRemove = files.indexOf(fileElement.data('fileData'));
                fileElement.parent().parent().remove();
                files.splice(indexToRemove, 1);
            });
        });
    });

    inputProfile.change(function () {
        if ($('#profile').length == 0) {
            let newFiles = [];
            for (let index = 0; index < inputProfile[0].files.length; index++) {
                let file = inputProfile[0].files[index];
                newFiles.push(file);
                files.push(file);
            };

            newFiles.forEach(file => {
                let fileElement = $(`<tr><td id="profile">${file.name}</td><td><span class="tbl-link fa-lg fa fa-user-circle"></span></td><td class="td-one-action"><span class="tbl-link fa-lg fa fa-trash-o"></span></td></tr>`);
                fileElement.data('fileData', file);
                $('.dataTables_empty').parent().remove()
                filesContainer.prepend(fileElement);

                fileElement.click(function (event) {
                    let fileElement = $(event.target);
                    let indexToRemove = files.indexOf(fileElement.data('fileData'));
                    fileElement.parent().parent().remove();
                    files.splice(indexToRemove, 1);
                });
            });
        } else {
            alert('Foto de perfil ja inclusa');
        }
    });

    buttonSubmit.click(function () {
        //toastr.success('Inclusão em andamento!')

        let formData = new FormData();

        files.forEach(file => {
            formData.append('files', file);
        });

        console.log('Sending...');

        $.ajax({
            url: controller + actionIncluir //'../Exemplo/IncluirExemplo'
            , data: $('#pwd-container1').serializeArray()
            , type: 'POST'
            , success: function (r) {
                if ($('#profile').length == 0) {
                    formData.append('perfil', 0);
                } else {
                    formData.append('perfil', 1);
                }

                if (editar || r == 1) {
                    if (files.length > 0) {
                        $.ajax({
                            url: controller + actionUpload //'../Exemplo/UploadFileExemplo_Inclusao'
                            , async: false
                            , data: formData
                            , type: 'POST'
                            , success: function (data) {
                                if (editar) {
                                    window.open(controller + actionMontarTela + '/' + r, '_self');
                                } else {
                                    window.open(controller + actionMontarTela, '_self');
                                }
                            } //'../Exemplo/MontarTelaExemplo'
                            , error: function (data) { console.log('ERROR!!'); }
                            , cache: false
                            , processData: false
                            , contentType: false
                        });
                    } else {
                        if (editar) {
                            window.open(controller + actionMontarTela + '/' + r, '_self');
                        } else {
                            window.open(controller + actionMontarTela, '_self');
                        }
                    }
                } else {
                    $('.tabs-container').prepend(
                        '<div class="alert alert-danger">'
                        + '<span>' + r + '</span>'
                        + '<button type="button" class="close" data-dismiss="alert">Fechar</button>'
                        + '</div>');
                }
            }
        });
    });
}