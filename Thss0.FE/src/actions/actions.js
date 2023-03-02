function addRecord(url, credentials) {
    return {
        type: 'addRecord',
        url,
        credentials
    }
}
function editRecord(url, credentials) {
    return {
        type: 'editRecord',
        url,
        credentials
    }
}
function deleteRecord(url) {
    return {
        type: 'deleteRecord',
        url
    }
}
export {
    addRecord,
    editRecord,
    deleteRecord
}