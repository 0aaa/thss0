const appReducer = (state, action) => {
    switch (action.type) {
        case 'updateContent':
            return {
                ...state
                , content: [...action.content]?.sort((a, b) => (action.inPageOrder && a.name?.localeCompare(b.name)) || b.name?.localeCompare(a.name))
                , globalOrder: action.globalOrder
                , inPageOrder: action.globalOrder
                , totalPages: action.totalPages
                , currentPage: action.currentPage
            }
        case 'updateDetailed':
            return {
                ...state
                , detailedItem: action.detailedItem
            }
        case 'updateAuth':
            return {
                ...state
                , username: action.username
            }
        case 'updateTheme':
            return {
                ...state
                , btnColor: (state.btnColor === 'light' && 'dark') || 'light'
            }
        default:
            return {...state}
    }
}
export default appReducer 