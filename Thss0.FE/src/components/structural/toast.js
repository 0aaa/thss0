const toast = msg =>
    `<div class="toast-container toast align-items-center text-bg-dark border-0 bottom-0 end-0 m-4 rounded-0" role="alert" aria-live="assertive" aria-atomic="true">
        <div class="d-flex">
            <div class="toast-body">
                ${msg}
            </div>
            <button class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
        </div>
    </div>`

export default toast