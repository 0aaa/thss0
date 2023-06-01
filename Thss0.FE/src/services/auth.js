import API_URL, { AUTH_TOKEN } from '../config/consts'
import { eraseErrors, handleErrors } from './errors'
import { UseToast } from '../config/hook'

async function makeRegister(target) {
    const fetchResult = await fetch(`${API_URL}user`, {
        method: 'POST'
        , headers: {
            'Content-Type': 'application/json'
        }
        , body: JSON.stringify({
            name: target.name
            , email: target.email
            , phoneNumber: target['phone-number']
            , password: target.password
            , passwordRepeat: target['password-repeat']
        })
    })
    if (fetchResult.ok) {
        eraseErrors()
        UseToast('Registered')
    } else {
        handleErrors(fetchResult)
    }
    return fetchResult.ok
}

async function getTokenAsync(target) {
    const fetchResult = await fetch(`${API_URL}account/token`, {
        method: 'POST'
        , headers: {
            'Content-Type': 'application/json'
        }
        , body: JSON.stringify({
            username: target.name.value
            , password: target.password.value
        })
    })
    if (fetchResult.ok) {
        eraseErrors()
        const data = await fetchResult.json()
        sessionStorage.setItem(AUTH_TOKEN, data.access_token)
        UseToast('Logged in')
        return data
    } else {
        handleErrors(fetchResult)
        UseToast(`Error: ${fetchResult.status}`)
        return null
    }
}

export {
    makeRegister
    , getTokenAsync
}