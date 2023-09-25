import { Children } from 'react'
import { getRecords } from '../services/entities'
import { AUTH_TOKEN } from '../config/consts'
import { UseRedirect } from '../config/hooks'
import { HOME_PATH } from '../config/consts'
import { Modal } from 'bootstrap'

const Devices = props => {
    if (!sessionStorage.getItem(AUTH_TOKEN)) {
        UseRedirect(HOME_PATH)
        const modal = document.getElementById('loginModal')
        modal && new Modal(modal).show()
    }
    const devicesModal = document.getElementById('devices-body')
    if (!devicesModal) {
        return
    }
    devicesModal.innerHTML
        = `<div class="h-75 d-flex justify-content-center align-items-center gap-1">
            ${Children.toArray([...Array(3).keys()].map(() =>
                `<div class="spinner-grow text-primary" role="status">
                    <span class="visually-hidden">Loading...</span>
                </div>`
            ).join(''))}
        </div>`
    getRecords('devices').then(devices => {
        if (!devices) {
            return
        }
        devicesModal.innerHTML
            = `${(devices.content
                && `<table class="table">
                    <tbody>
                        ${Children.toArray(devices.content.map(device =>
                                `<tr>
                                    <td>
                                        ${device.name}
                                    </td>
                                    <td class="text-center">
                                        ${device.availability ? 'ready' : 'busy'}
                                    </td>
                                    <td class="text-end">
                                        <button id="${device.name}-btn" class="btn btn-outline-dark py-0 border-0 border-bottom rounded-0" ${!device.availability && 'disabled'} data-bs-dismiss="modal">Read</button>
                                    </td>
                                </tr>`
                            ).join(''))}
                    </tbody>
                </table>`)
                || `<div class="h-75 d-flex justify-content-center align-items-center gap-1">
                    ${Children.toArray([...Array(3).keys()].map(() =>
                        `<div class="spinner-grow text-primary" role="status">
                            <span class="visually-hidden">Loading...</span>
                        </div>`).join(''))}
                </div>`}`
        let deviceBtn
        for (let index = 0; index < devices.content.length; index++) {
            deviceBtn = document.getElementById(`${devices.content[index].name}-btn`)
            if (!deviceBtn) {
                continue
            }
            deviceBtn.addEventListener('click', async event => {
                event.preventDefault()
                await handleDevice({...props}, devices.content[index].name, () => event.preventDefault())
            })
        }        
    })
}
const handleDevice = async (props, name, preventDefault) => {
    const data = await getRecords(`devices/${name}`)
    console.log(data)
    document.getElementById('content').value = data.content
    props.updateContent(props, {target: {id: 'content', value: data.content}, preventDefault: () => preventDefault})
}
export default Devices