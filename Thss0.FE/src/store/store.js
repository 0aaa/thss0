import { configureStore } from "@reduxjs/toolkit"
import appReducer from "../reducers/reducer"

const appStore = configureStore({
    reducer: appReducer
    , middleware: mdlwre => mdlwre({ serializableCheck: false })
})
export default appStore