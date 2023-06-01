function updateContent(content, totalPages, localOrder) {
    return {
        type: 'updateContent'
        , content
        , totalPages
        , localOrder
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