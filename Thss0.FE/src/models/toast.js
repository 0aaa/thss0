const toast = (message) =>
  '<div className="toast-container toast align-items-center text-bg-primary border-0 bottom-0 end-0 m-4" role="alert" aria-live="assertive" aria-atomic="true">'
    + '<div className="d-flex">'
      + '<div className="toast-body">'
        + message
      + '</div>'
      + '<button type="button" className="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>'
    + '</div>'
  + '</div>'
export default toast