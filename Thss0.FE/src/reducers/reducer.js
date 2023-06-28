function appReducer(state, action) {
    switch (action.type) {
        case 'updateContent':
            return {
                ...state
                , content: [...action.content]?.sort((a, b) => action.localOrder ? a.name?.localeCompare(b.name) : b.name?.localeCompare(a.name))
                , totalPages: action.totalPages
                , localOrder: action.localOrder
                , currentPage: action.currentPage
            }
        case 'updateAuth':
            return {
                ...state
                , username: action.username
            }
        default:
            return { ...state }
    }
}
export default appReducer 