import { configureStore } from '@reduxjs/toolkit'
import appReducer from '../reducers/reducer'

const appStore = configureStore({
    reducer: appReducer
    , preloadedState: {
        content: []
        , globalOrder: true
        , inPageOrder: true
        , printBy: 20
        , totalPages: 1
        , currentPage: 1
        , detailedItem: {}
        , btnColor: (document.documentElement.getAttribute('data-bs-theme') === 'light' && 'dark') || 'light'
    }
    , middleware: mw => mw({ serializableCheck: false })
})
export default appStore