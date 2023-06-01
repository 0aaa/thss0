import { useEffect } from 'react'
import { useLocation, useNavigate } from 'react-router-dom'
import Toast from 'bootstrap/js/dist/toast'
import { toastsArr } from './consts'
import toast from '../models/toast'

const UseRedirect = (path) => {
    const navigate = useNavigate()
    useEffect(() => navigate(path))
}
const UseUpdate = (props, path) => {
    const location = useLocation()
    useEffect(() => { props.updateContent({...props.state}, path) }, [location])
}
const UseToast = (message) => {
    toastsArr.push(toast(message))
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