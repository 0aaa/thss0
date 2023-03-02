import { configureStore } from "@reduxjs/toolkit"
import appReducer from "../reducers/reducer"

const initState = {
    content: []
}
const appStore = configureStore({ initialState: initState, reducer: appReducer })
export default appStore