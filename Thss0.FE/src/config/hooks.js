import { useEffect } from 'react'
import { useLocation, useNavigate } from 'react-router-dom'
import Toast from 'bootstrap/js/dist/toast'
import toast from '../models/toast'

const UseRedirect = (path) => {
    const navigate = useNavigate()
    useEffect(() => navigate(path))
}
const UseUpdate = (props, path) => {
    const location = useLocation()
    useEffect(() => { props.updateContent({...props}, path) }, [location])
}
const UseToast = (message) => {
    const div = document.createElement('div')
    div.innerHTML = toast(message)
    document.getElementById('root').appendChild(div)
    const toasts = document.querySelectorAll('.toast')
    if (toasts[toasts.length - 1]) {        
        new Toast(toasts[toasts.length - 1]).show()
    }
}
export {
    UseRedirect
    , UseUpdate
    , UseToast
}