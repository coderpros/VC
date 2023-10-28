var sessMan_Disabled = false;
var sessMan_WarningTimer;
var sessMan_ExpiredTimer;
var sessMan_LastRefresh = new Date();
var sessMan_RefreshInProgress = false;

(function (sessMan, $, undefined) {

    sessMan.Init = function () {
        sessMan_WarningTimer = setTimeout(sessMan.WarningPrompt, 1680000); // 28 minutes (1680 seconds - 1680000)
        sessMan_ExpiredTimer = setTimeout(sessMan.Logout, 1740000); // 29 minutes (1740 seconds - 1740000)
    };

    sessMan.Refresh = function () {
        if (!sessMan_Disabled) {
            if (!window.location.origin) {
                window.location.origin = window.location.protocol + "//" + window.location.hostname + (window.location.port ? ':' + window.location.port : '');
            }

            //$.get(window.location.origin + "/auth/refresh");
            //$.get(window.location.origin + "/api/auth/refresh");
            if ((((new Date()).getTime() - sessMan_LastRefresh.getTime()) / 60000) > 1) {
                sessMan_RefreshInProgress = true;
                $.get(window.location.origin + "/api/auth/refresh", function (data) {
                    if (data === true) {
                        // valid session
                        sessMan_LastRefresh = new Date();
                        sessMan_RefreshInProgress = false;
                    }
                    else {
                        // invalid session
                        window.location = window.location.origin + "/";
                    }
                });
            }

            clearTimeout(sessMan_WarningTimer);
            clearTimeout(sessMan_ExpiredTimer);

            sessMan.Init();

            DotNet.invokeMethodAsync('VC.Res.WebInterface', 'RefreshSession').then(data => { /*console.log(data);*/ });
        }
    };

    sessMan.WarningPrompt = function () {
        var modalBox = $("#model_SessionTimeout");

        var modalBG = $('.modal__bg');

        var modalCloseElems = $('.modal__bg, #model_SessionTimeout .modal__close, #model_SessionTimeout .modal__dismiss');
        var activeClass = 'is-active';

        if (typeof modalBox !== 'undefined') {
            setTimeout(function () {
                modalBox.addClass(activeClass);
                if (typeof modalBG !== 'undefined') {
                    modalBG.addClass(activeClass);
                }
            }, 15);

            $("#Btn_SessionTimeout_LogOut").off('click.sessManLogOut');
            $("#Btn_SessionTimeout_LogOut").on('click.sessManLogOut', sessMan.Logout);

            $("#Btn_SessionTimeout_Continue").off('click.sessManContinue');
            $("#Btn_SessionTimeout_Continue").on('click.sessManContinue', sessMan.Refresh);

            modalCloseElems.off('click.sessManWarningPromptOff');
            modalCloseElems.on('click.sessManWarningPromptOff', function () {
                if (modalBox.hasClass(activeClass)) {
                    modalBox.removeClass(activeClass);
                }
            });
        }
    };

    sessMan.Logout = function () {
        if (!window.location.origin) {
            window.location.origin = window.location.protocol + "//" + window.location.hostname + (window.location.port ? ':' + window.location.port : '');
        }

        window.location = window.location.origin + "/auth/logout";
    };

    sessMan.Disable = function () {
        clearTimeout(sessMan_WarningTimer);
        clearTimeout(sessMan_ExpiredTimer);
        sessMan_Disabled = true;
    };

}(window.sessMan = window.sessMan || {}, jQuery));