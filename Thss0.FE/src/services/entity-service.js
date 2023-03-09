import {AUTH_TOKEN, API_URL} from "../config/consts"

async function getRecords(path) {
    const response = await fetch(API_URL + path)
    const data = await response.json()
    return data
}

async function addRecord(path, crdntlsDctnry) {
    // For the Procedure type only.
    crdntlsDctnry['realizationTime'] = new Date(crdntlsDctnry['realizationTime'])
    crdntlsDctnry['nextProcedureTime'] = new Date(crdntlsDctnry['nextProcedureTime'])
    //
    const fetchResult = await fetch(API_URL + path, {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': `bearer ${sessionStorage.getItem(AUTH_TOKEN)}`
        },
        body: JSON.stringify(crdntlsDctnry)
    })
    if (fetchResult.ok) {
        console.log(`${(fetchResult.json()).id} added`)
    } else {
        console.log('adding error')
    }
}

async function editRecord(path, crdntlsDctnry) {
    const fetchResult = await fetch(API_URL + path, {
        method: 'PUT',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': `bearer ${sessionStorage.getItem(AUTH_TOKEN)}`
        },
        body: JSON.stringify(crdntlsDctnry)
    })
    if (fetchResult.ok) {
        console.log(`edited ${crdntlsDctnry['id']}`)
    } else {
        console.log('editing error')
    }
}

async function deleteRecord(path) {
    const fetchResult = await fetch(API_URL + path, {
        method: 'DELETE',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `bearer ${sessionStorage.getItem(AUTH_TOKEN)}`
        }
    })
    if (fetchResult.ok) {
        console.log(`deleted ${path}`)
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
    getRecords,
    addRecord,
    editRecord,
    deleteRecord
}