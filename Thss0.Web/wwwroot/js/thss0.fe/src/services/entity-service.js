import AUTH_TOKEN from "../config/consts"

async function addRecord(requestUrl, crdntlsDctnry) {
    // For the Procedure type only.
    // crdntlsDctnry['realizationTime'] = new Date(crdntlsDctnry['realizationTime'])
    // crdntlsDctnry['nextProcedureTime'] = new Date(crdntlsDctnry['nextProcedureTime'])
    //
    const fetchResult = await fetch(requestUrl, {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': `bearer ${sessionStorage.getItem(AUTH_TOKEN)}`
        },
        body: JSON.stringify(crdntlsDctnry)
    })
    if (fetchResult.ok) {
        // The state must be updated.
        console.log(`${(await fetchResult.json()).id} added`)
    } else {
        console.log('adding error')
    }
}

async function editRecord(requestUrl, crdntlsDctnry) {
    const fetchResult = await fetch(requestUrl, {
        method: 'PUT',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': `bearer ${sessionStorage.getItem(AUTH_TOKEN)}`
        },
        body: JSON.stringify(crdntlsDctnry)
    })
    if (fetchResult.ok) {
        // The state must be updated.
        console.log(`edited ${crdntlsDctnry['id']}`)
    } else {
        console.log('editing error')
    }
}

async function deleteRecord(requestUrl) {
    const fetchResult = await fetch(requestUrl, {
        method: 'DELETE',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `bearer ${sessionStorage.getItem(AUTH_TOKEN)}`
        }
    })
    if (fetchResult.ok) {
        // The state must be updated.
        console.log(`deleted ${requestUrl}`)
    } else {
        // To adjust.
        /*document.getElementById('delete-error').innerHTML = ''
        const err = await fetchResult.json()
        if (err.errors) {
            for (var e in err.errors) {
                for (var r in err.errors[e]) {
                    const message = document.createElement('p')
                    message.append(err.errors[e][r])
                    document.getElementById('delete-error').append(message)
                }
            }
        }*/
    }
}
export {
    addRecord,
    editRecord,
    deleteRecord
}