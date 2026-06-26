class ModalManager {
    constructor(modalId) {
        this.$modal = $(`#${modalId}`);
        this.$title = this.$modal.find('#modal-title');
        this.$body = this.$modal.find('#modal-body-content');
        this.initEvents();
    }

    initEvents() {
        this.$modal.on('click', '#js-close-modal', () => this.close());
        this.$modal.on('click', (e) => {
            if ($(e.target).is(this.$modal)) this.close();
        });
        $(document).on('keydown', (e) => {
            if (e.key === 'Escape' && this.$modal.hasClass('is-active')) this.close();
        });
    }

    open(title, templateId, onSubmitCallback = null) {
        this.$title.text(title);
        const htmlContent = $(templateId).html();
        this.$body.html(htmlContent);
        this.$modal.addClass('is-active');

        if (onSubmitCallback) {
            this.$body.find('form').on('submit', function (e) {
                e.preventDefault();
                onSubmitCallback($(this));
            });
        }
    }

    close() {
        this.$modal.removeClass('is-active');
        setTimeout(() => this.$body.empty(), 250);
    }
}

$(function () {
    window.Modal = new ModalManager('app-modal');
});