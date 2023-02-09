import AUTH_TOKEN from "../config/consts"

export async function addRecord(entityName, dctnry) {
    const fetchResult = await fetch(`/api/${entityName}`, {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': `bearer ${sessionStorage.getItem(AUTH_TOKEN)}`
        },
        body: JSON.stringify(dctnry)
    })
    if (fetchResult.ok) {
        // The state must be updated.
        console.log(`${(await fetchResult.json()).id} added`)
    } else {
        console.log('adding error')
    }
}
export default async function editRecord(entityName, recordId, dctnry) {

    const fetchResult = await fetch(`/api/${entityName}`, {
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
    if (fetchResult.ok) {
        // The state must be updated.
        console.log(`${recordId} edited`)
    } else {
        console.log('editing error')
    }
}
export async function deleteRecord(entityName, recordId) {
    const fetchResult = await fetch(`/api/${entityName}/${recordId}`, {
        method: 'DELETE',
        headers: {
            'Authorization': `bearer ${sessionStorage.getItem(AUTH_TOKEN)}`
        }
    })
    if (fetchResult.ok) {
        // The state must be updated.
        console.log(`${recordId} deleted`)
    } else {
    // To adjust.
        document.getElementById('delete-error').innerHTML = ''
        const err = await fetchResult.json()
        if (err.errors) {
            for (var e in err.errors) {
                for (var r in err.errors[e]) {
                    const message = document.createElement('p')
                    message.append(err.errors[e][r])
                    document.getElementById('delete-error').append(message)
                }
            }
        }
    }
}