import { AUTH_TOKEN, API_URL } from '../config/consts'
import { UseToast } from '../config/hooks'
import { eraseErrors, handleErrors } from './errors'

async function getRecords(path, globalOrder, printBy, currentPage) {
    globalOrder = globalOrder ? `/${globalOrder}` : ''
    printBy = printBy ? `/${printBy}` : ''
    currentPage = currentPage ? `/${currentPage}` : ''
    path += path === 'users' ? '/client' : ''
    const fetchResult = await fetch(API_URL + path + globalOrder + printBy + currentPage, {
        method: 'GET'
        , headers: {
            'Accept': 'application/json'
            , 'Content-Type': 'application/json'
            , 'Authorization': `bearer ${sessionStorage.getItem(AUTH_TOKEN)}`
        }
    })
    if (fetchResult.ok) {
        return await fetchResult.json()
    } else {
        UseToast(`Error: ${fetchResult.status}`)
        return null
    }
}

async function addRecord(path, crdntlsDctnry) {
    const fetchResult = await fetch(API_URL + path, {
        method: 'POST'
        , headers: {
            'Accept': 'application/json'
            , 'Content-Type': 'application/json'
            , 'Authorization': `bearer ${sessionStorage.getItem(AUTH_TOKEN)}`
        }
        , body: JSON.stringify(crdntlsDctnry)
    })
    if (fetchResult.ok) {
        eraseErrors()
        UseToast('Added')
    } else {
        handleErrors(fetchResult)
        UseToast(`Error: ${fetchResult.status}`)
    }
}

async function editRecord(path, crdntlsDctnry) {
    const fetchResult = await fetch(API_URL + path, {
        method: 'PUT'
        , headers: {
            'Accept': 'application/json'
            , 'Content-Type': 'application/json'
            , 'Authorization': `bearer ${sessionStorage.getItem(AUTH_TOKEN)}`
        }
        , body: JSON.stringify(crdntlsDctnry)
    })
    if (fetchResult.ok) {
        eraseErrors()
        UseToast('Edited')
    } else {
        handleErrors(fetchResult)
        UseToast(`Error: ${fetchResult.status}`)
    }
}

async function deleteRecord(path) {
    const fetchResult = await fetch(API_URL + path, {
        method: 'DELETE'
        , headers: {
            'Content-Type': 'application/json'
            , 'Authorization': `bearer ${sessionStorage.getItem(AUTH_TOKEN)}`
        }
    })
    if (fetchResult.ok) {
        eraseErrors()
        UseToast('Deleted')
    } else {
        handleErrors(fetchResult)
    }
}

export {
    getRecords
    , addRecord
    , editRecord
    , deleteRecord
}