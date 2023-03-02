import { addRecord, deleteRecord, editRecord } from '../services/entity-service'

async function appReducer(state, action) {
    switch (action.type) {
        case 'addRecord':
            await addRecord(action.url, action.credentials)
            break
        case 'editRecord':
            editRecord(action.url, action.credentials)
            break
        case 'deleteRecord':
            await deleteRecord(action.url)
            break
        default:
            return { ...state }
    }
    const response = await fetch(action.url) // URL for ALL to set.
    state.content = await response.json()
    return { ...state, content: [state.content] }
}
export default appReducer 