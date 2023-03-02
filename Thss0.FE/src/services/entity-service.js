import AUTH_TOKEN from "../config/consts"

function addRecord(requestUrl, crdntlsDctnry) {
    // For the Procedure type only.
    // crdntlsDctnry['realizationTime'] = new Date(crdntlsDctnry['realizationTime'])
    // crdntlsDctnry['nextProcedureTime'] = new Date(crdntlsDctnry['nextProcedureTime'])
    //
    const fetchResult = fetch(requestUrl, {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': `bearer ${sessionStorage.getItem(AUTH_TOKEN)}`
        },
        body: JSON.stringify(crdntlsDctnry)
    }).then()
    if (fetchResult.ok) {
        console.log(`${(fetchResult.json().then()).id} added`)
    } else {
        console.log('adding error')
    }
}

function editRecord(requestUrl, crdntlsDctnry) {
    const fetchResult = fetch(requestUrl, {
        method: 'PUT',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': `bearer ${sessionStorage.getItem(AUTH_TOKEN)}`
        },
        body: JSON.stringify(crdntlsDctnry)
    }).then()
    if (fetchResult.ok) {
        console.log(`edited ${crdntlsDctnry['id']}`)
    } else {
        console.log('editing error')
    }
}

function deleteRecord(requestUrl) {
    const fetchResult = fetch(requestUrl, {
        method: 'DELETE',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `bearer ${sessionStorage.getItem(AUTH_TOKEN)}`
        }
    }).then()
    if (fetchResult.ok) {
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