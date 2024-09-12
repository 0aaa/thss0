import { useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import Toast from 'bootstrap/js/dist/toast'
import toast from '../components/structural/toast'

const UseRedirect = path => {
    const navigate = useNavigate()
    useEffect(() => navigate(path))
}

const UseUpdate = (props, path) => {
    // eslint-disable-next-line
    useEffect(() => { props.updateContent({ ...props }, path) }, [path])
}

const UseToast = msg => {
    const div = document.createElement('div')
    div.innerHTML = toast(msg)
    document.getElementById('root').appendChild(div)
    const toasts = document.querySelectorAll('.toast')
    toasts[toasts.length - 1] && new Toast(toasts[toasts.length - 1]).show()
}

export {
    UseRedirect
    , UseUpdate
    , UseToast
}