import { login, register } from './authApi.js';
import { initCustomRules, loginRules, registerRules } from './authValidator.js';
import { handleApiErrors } from '../helpers/handleApiErrors.js';

$(function () {
    initCustomRules();

    function togglePanels() {
        $('#loginPanel, #registerPanel').toggleClass('hidden');
        $('#loginError, #registerError').hide();
    }

    $('.toggle-auth-panels').on('click', togglePanels);

    $('#loginForm').validate({
        ...loginRules,
        submitHandler: function (form, e) {
            e.preventDefault();
            const errorBox = $('#loginError').hide();
            const btn = $(form).find('button[type="submit"]');

            btn.prop('disabled', true).addClass('loading');

            login($('#loginEmail').val(), $('#loginPassword').val())
                .done(function (response) {
                    localStorage.setItem("accessToken", response.accessToken);
                    window.location.href = '/';
                })
                .fail(function (xhr) {
                    handleApiErrors(xhr, errorBox, "Помилка авторизації.");
                })
                .always(function () {
                    btn.prop('disabled', false).removeClass('loading');
                });
        }
    });

    $('#registerForm').validate({
        ...registerRules,
        submitHandler: function (form, e) {
            e.preventDefault();
            const errorBox = $('#registerError').hide();
            const btn = $(form).find('button[type="submit"]');

            btn.prop('disabled', true).addClass('loading');

            register($('#registerEmail').val(), $('#registerPassword').val())
                .done(function () {
                    alert("Реєстрація успішна! Тепер ви можете увійти.");
                    togglePanels();
                })
                .fail(function (xhr) {
                    handleApiErrors(xhr, errorBox, "Помилка реєстрації.");
                })
                .always(function () {
                    btn.prop('disabled', false).removeClass('loading');
                });
        }
    });

    window.togglePanels = togglePanels;
});