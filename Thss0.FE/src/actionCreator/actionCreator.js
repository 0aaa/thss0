function updateContent(content, totalPages, localOrder, currentPage) {
    return {
        type: 'updateContent'
        , content
        , totalPages
        , localOrder
        , currentPage
    }
}
function updateAuth(username = null) {
    return {
        type: 'updateAuth'
        , username
    }
}
export {
    updateContent
    , updateAuth
}