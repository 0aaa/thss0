function appReducer(state, action) {
    switch (action.type) {
        case 'updateState':
            return { ...state, content: action.payload }
        default:
            return { ...state }
    }
}
export default appReducer 