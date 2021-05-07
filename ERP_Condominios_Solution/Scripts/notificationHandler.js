var refreshtime = 0;

if (refreshtime == 0) {
    $.ajax({
        url: '../Notificacao/GetNotificacaoRefreshTime'
        , type: 'POST'
        , success: function (r) {
            refreshtime = r;
            CallNotification();
            BeginInterval();
        }
    });
} else {
    CallNotification();
    BeginInterval();
}

function BeginInterval() {
    window.setInterval(function () {
        CallNotification();
    }, 1000 * 60 * refreshtime);
}

function CallNotification() {
    $.ajax({
        url: '../Notificacao/GetNotificacaoNaoLida'
        , type: 'POST'
        , success: function (r) {
            if (r.msg != undefined) {
                notifier.show('Notificações', r.msg + '<span style="margin-left: 10px;" onclick="ViewNotification()" class="btn btn-warning btn-sm">Ver</span>', 'success', '', 1000 * 15);
            }
        }
    });

    $.ajax({
        url: '../Tarefa/GetTarefaNaoExecutada'
        , type: 'POST'
        , success: function (r) {
            if (r.msg != undefined) {
                notifier.show('Tarefas', r.msg + '<span style="margin-left: 10px;" onclick="ViewNotificationTarefa()" class="btn btn-warning btn-sm">Ver</span>', 'success', '', 1000 * 15);
            }
        }
    });
}

function ViewNotification() {
    //Redirecionamento para grid ou visualização de notificação
    $.ajax({
        url: '../Notificacao/NovaNotificacaoClick'
        , type: 'POST'
        , success: function (r) {
            if (r != 0) {
                window.open('../Notificacao/VerNotificacao/' + r, '_self');
            } else {
                window.open('../Notificacao/MontarTelaNotificacao?id=1', '_self')
            }
        }
    });
}

function ViewNotificationTarefa() {
    $.ajax({
        url: '../Tarefa/TarefaNaoRealizadaClick'
        , type: 'POST'
        , success: function (r) {
            window.open('../Tarefa/MontarTelaTarefa?id=1', '_self');
        }
    });
}