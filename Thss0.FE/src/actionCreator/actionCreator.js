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
const updateDetailed = detailedItem => {
    return {
        type: 'updateDetailed'
        , detailedItem
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
const updateModal = modalName => {
    return {
        type: 'updateModal'
        , modalName
    }
}
const updateOffcanvas = offcanvasName => {
    return {
        type: 'updateOffcanvas'
        , offcanvasName: offcanvasName
    }
}
export {
    updateContent
    , updateDetailed
    , updateAuth
    , updateTheme
    , updateModal
    , updateOffcanvas
}