import { configureStore } from '@reduxjs/toolkit'
import appReducer from '../reducers/reducer'

const appStore = configureStore({
    reducer: appReducer
    , preloadedState: {
        order: true
        , printBy: 20
        , currentPage: 1
        , localOrder: true
    }
    , middleware: mw => mw({ serializableCheck: false })
})
export default appStore