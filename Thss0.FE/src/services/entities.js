import { AUTH_TOKEN, API_URL } from '../config/consts'
import { UseToast } from '../config/hooks'
import { eraseErrors, handleErrors } from './errors'

const getRecords = async (path, globalOrder, printBy, currentPage) => {
    if (['professional', 'client'].includes(path.split('/')[0])) {        
        path = `users/${path}`
    }
    globalOrder = (globalOrder && `/${globalOrder}`) || ''
    printBy = (printBy && `/${printBy}`) || ''
    currentPage = (currentPage && `/${currentPage}`) || ''
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

const addRecord = async (path, addDictionary) => {
    const fetchResult = await fetch(API_URL + path, {
        method: 'POST'
        , headers: {
            'Accept': 'application/json'
            , 'Content-Type': 'application/json'
            , 'Authorization': `bearer ${sessionStorage.getItem(AUTH_TOKEN)}`
        }
        , body: JSON.stringify(addDictionary)
    })
    if (fetchResult.ok) {
        eraseErrors()
        UseToast('Added')
    } else {
        handleErrors(fetchResult)
        UseToast(`Error: ${fetchResult.status}`)
    }
}

const editRecord = async (path, crdntlsDctnry) => {
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

const deleteRecord = async path => {
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