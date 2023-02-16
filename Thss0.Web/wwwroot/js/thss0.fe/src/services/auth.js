import { useNavigate } from "react-router-dom"
import { useEffect } from "react"
import { AUTH_TOKEN, REGISTER_PATH } from "../config/consts"

async function makeRegister(event) {
    const userInput = {
        name: event.name,
        email: event.email,
        phoneNumber: event['phone-number'],
        password: event.password,
        passwordRepeat: event['password-repeat']
    }
    const fetchResult = await fetch('api/user/register', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(userInput)
    })
    console.log(`register: ${fetchResult.ok}`)
    return fetchResult.ok
}
async function getTokenAsync(event) {
    const userInput = {
        name: document.getElementById('name').value,
        password: document.getElementById('password').value
    }
    const fetchResult = await fetch('api/user/token', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(userInput)
    })
    const data = await fetchResult.json()
    if (fetchResult.ok) {
        sessionStorage.setItem(AUTH_TOKEN, data.access_token)
        return true
    } else {
        sessionStorage.removeItem(AUTH_TOKEN)
        alert(`Error: ${fetchResult.status}`)
        return false
    }
}
function Logout() {
    console.log('logout')
    const navigate = useNavigate()
    sessionStorage.removeItem(AUTH_TOKEN)
    useEffect(() => navigate(REGISTER_PATH))
}
export {
    makeRegister,
    getTokenAsync,
    Logout
}