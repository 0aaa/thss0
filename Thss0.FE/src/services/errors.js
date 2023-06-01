import { UseToast } from "../config/hook"

async function handleErrors(fetchResult) {
    const err = await fetchResult.json()
    console.log(err)
    if (err) {
        printErrors(err)
    }
}

function printErrors(err) {
    let errorSpan = null
    for (var e in err) {
        errorSpan = document.getElementById(`${e.replace(/^./, e[0].toLowerCase())}-error`)
        errorSpan.innerHTML = ''
        errorSpan.innerHTML = `<svg xmlns="http://www.w3.org/2000/svg" class="bi flex-shrink-0 me-2" width=16 height=16>
              <path d="M8.982 1.566a1.13 1.13 0 0 0-1.96 0L.165 13.233c-.457.778.091 1.767.98 1.767h13.713c.889 0 1.438-.99.98-1.767L8.982 1.566zM8 5c.535 0 .954.462.9.995l-.35 3.507a.552.552 0 0 1-1.1 0L7.1 5.995A.905.905 0 0 1 8 5zm.002 6a1 1 0 1 1 0 2 1 1 0 0 1 0-2z"/>
            </svg>${err[e]}`
        errorSpan.className = 'alert alert-danger d-flex align-items-center'
        UseToast(err[e])
    }
}

function eraseErrors() {
    const errorSpans = document.getElementsByClassName('alert')
    for (let index = 0; index < errorSpans.length; index++) {
        errorSpans[index].innerHTML = ''
        errorSpans[index].className = 'd-none'        
    }
}

export {
    handleErrors
    , eraseErrors
}