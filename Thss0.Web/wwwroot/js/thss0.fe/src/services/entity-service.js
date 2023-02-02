async function addRecord(entityName, dctnry) {
    const FETCH_RESULT = await fetch(`/api/${entityName}`, {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': `bearer ${sessionStorage.getItem(AUTH_TOKEN)}`
        },
        body: JSON.stringify(dctnry)
    })
}
async function editRecord(entityName, recordId, dctnry) {

    const FETCH_RESULT = await fetch(`/api/${entityName}`, {
        method: 'PUT',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': `bearer ${sessionStorage.getItem(AUTH_TOKEN)}`
        },
        body: JSON.stringify({
            entityId: recordId,
            dctnry
        })
    })
}
async function deleteRecord(entityName, recordId) {
    const FETCH_RESULT = await fetch(`/api/${entityName}/${recordId}`, {
        method: 'DELETE',
        headers: {
            'Authorization': `bearer ${sessionStorage.getItem(AUTH_TOKEN)}`
        }
    })
}