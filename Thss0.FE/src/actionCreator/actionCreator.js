const updateContent = stateCopy => {
    return {
        type: 'updateContent'
        , content: stateCopy.content
        , currentIndex: stateCopy.currentIndex
        , globalOrder: stateCopy.globalOrder
        , inPageOrder: stateCopy.inPageOrder
        , printBy: stateCopy.printBy
        , totalPages: stateCopy.totalPages
        , currentPage: stateCopy.currentPage
    }
}
const updateDetailed = (detailedItem, offcanvasName) => {
    return {
        type: 'updateDetailed'
        , detailedItem
        , offcanvasName
    }
}
const updateAuth = (username = null) => {
    return {
        type: 'updateAuth'
        , username
    }
}
const updateTheme = () => {
    return {
        type: 'updateTheme'
    }
}
const updateModal = (modalName, payload) => {
    return {
        type: 'updateModal'
        , modalName
        , payload
    }
}
export {
    updateContent
    , updateDetailed
    , updateAuth
    , updateTheme
    , updateModal
}