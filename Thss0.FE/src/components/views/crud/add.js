import React, { Children } from 'react'
import { connect } from 'react-redux'
import { updateContent, updateModal } from '../../../actionCreator/actionCreator'
import { addRecord, getRecords } from '../../../services/entities'
import { Carousel } from 'bootstrap'

class Add extends React.Component {
    constructor(props) {
        super(props)
        this.state = { keys: [], roles: [] }
    }

    async componentDidMount() {
        document.getElementById('addCarousel') && new Carousel('#addCarousel')
        this.getKeys()
    }

    componentDidUpdate = () => this.getKeys()

    getKeys() {
        let keys = []
        if (this.props.detailedItem) {
            keys = Object.keys(this.props.detailedItem).filter(key => !['id', 'creationTime', 'department', 'result', 'user', 'procedure', 'substance'].includes(key)).reverse()
        }
        if (this.state.keys.join() !== keys.join()) {
            this.setState({ keys })
            this.state.roles.length === 0 && ['professional', 'client'].includes(this.props.entityName) && getRecords('role', this.props.printBy, this.props.currentPage, this.props.globalOrder).then(res => this.setState({ roles: res.content }))
        }
    }

    async updateDatalist(e) {
        e.preventDefault()
        const currentKey = e.target.id.replace(/.{6}$/, '')
        const datalist = e.target.parentNode
        let contentArr = []
        let optionToAdd = null
        if (['departmentNames', 'userNames', 'procedure', 'resultNames', 'procedureNames'].includes(currentKey)
                && e.target.value.length > 3) {
            contentArr = (await getRecords(`search/${encodeURIComponent(e.target.value)}/${currentKey}`, this.props.printBy, this.props.currentPage, this.props.globalOrder)).content
        }
        datalist.innerHTML = ''
        datalist.appendChild(e.target)
        e.target.focus()
        for (let i = -1; i < contentArr.length && e.target.value; i++) {
            optionToAdd = document.createElement('pre')
            optionToAdd.className = 'border bg-white p-2 mb-0 user-select-none bg-opacity-75'
            if (i > -1) {
                optionToAdd.innerHTML = contentArr[i].name
            } else if (currentKey.endsWith('Time') && e.target.value.match(/.{2}$/)[0] % 15) {
                const timeToAdjust = new Date(e.target.value)
                timeToAdjust.setMinutes(timeToAdjust.getMinutes() - timeToAdjust.getTimezoneOffset() + 15 - e.target.value.match(/.{2}$/)[0] % 15)
                optionToAdd.innerHTML = timeToAdjust.toISOString().replace(/.{8}$/, '')
            } else if (currentKey === 'photo') {
                const img = document.createElement('img')
                img.src = URL.createObjectURL(e.target.files[0])
                img.className = 'w-100'
                optionToAdd.appendChild(img)
            } else {
                optionToAdd.innerHTML = e.target.value
            }
            datalist.appendChild(optionToAdd)
            this.drag(optionToAdd, currentKey)
        }
    }

    drag(option, key) {
        let srcX = 0
        let srcY = 0
        let buff = null
        const form = document.getElementById('addForm')
        option.onmousedown = () => {
            option.style.position = 'fixed'
            srcX = option.style.left
            srcY = option.style.top
            buff = option
            buff.className = 'border bg-white p-2 mb-0 user-select-none w-25 bg-opacity-75'
            document.onmouseup = e => {
                document.onmousemove = null
                document.onmouseup = null
                if (e.pageX > form.offsetParent.offsetLeft && e.pageX < form.offsetParent.offsetLeft + form.offsetWidth
                        && e.pageY > form.offsetTop && e.pageY < form.offsetTop + form.offsetHeight) {

                    buff.style.position = 'static'
                    const li = document.getElementById(key)
                    if (li.childElementCount > 2 && ['name', 'photo', 'pob', 'dob', 'role', 'email', 'phoneNumber', 'password'].includes(key)) {
                        while (li.childElementCount > 2) {
                            li.childNodes[2].remove()
                        }
                    }
                    li.appendChild(buff)
                    buff.className = 'border bg-white p-2 mb-0 user-select-none w-100 bg-opacity-75'
                    if (key === 'password') {
                        const passwordClone = buff.cloneNode(true)
                        buff.className = 'd-none'
                        passwordClone.innerHTML = passwordClone.innerHTML.replace(/\S/g, '*')
                        li.appendChild(passwordClone)
                    }
                } else if (buff) {
                    buff.style.position = 'static'
                    buff.style.left = srcX
                    buff.style.top = srcY
                    document.getElementById(`${key}-input`).parentNode.appendChild(buff)
                    buff.className = 'border bg-white p-2 mb-0 user-select-none w-100 bg-opacity-75'
                }
                buff = null
            }
            document.onmousemove = e => {
                if (buff) {
                    buff.style.left = `${e.pageX}px`
                    buff.style.top = `${e.clientY}px`
                }
            }
        }
    }

    printForm(e) {
        let currentIndex = 0
        if (e.target.className === 'active') {
            currentIndex = [...e.target.parentNode.childNodes].indexOf(e.target)
        } else {
            currentIndex = [...document.getElementsByClassName('carousel-item')].filter(sl => sl.parentNode.parentNode.id === 'addCarousel').findIndex(i => i.className.includes('active'))
            if (e.target.className.includes('carousel-control-next')) {
                currentIndex = (currentIndex < this.state.keys.length - 1 && currentIndex + 1) || 0
            } else {
                currentIndex = (currentIndex === 0 && this.state.keys.length - 1) || currentIndex - 1
            }
        }
        const lis = document.getElementsByClassName('add-li')
        for (let i = 0; i < lis.length; i++) {
            lis[i].className = ((i <= currentIndex || lis[i].childNodes[2]) && 'add-li d-block') || 'add-li d-none'
        }
    }

    render() {
        return this.state.keys.length > 0
            && <div className="d-flex gap-1 offcanvas-body">
                <form id="addForm" onSubmit={e => this.props.handleAdd(e, {...this.props})}
                        className="d-flex flex-column h-100 w-50"
                        encType="multipart/form-data">
                    <ul className="list-unstyled ms-2 overflow-y-auto">
                        {Children.toArray(this.state.keys.map(k =>
                            <li id={k} className={`add-li ${this.state.keys.indexOf(k) > 0 && 'd-none'}`}>
                                <span id={`${k}Error`} className="d-none"></span>
                                <span>{k.replace(/([A-Z]+)/g, ' $1').replace(/^./, k[0].toUpperCase())}</span>                                
                            </li>
                        ))}
                    </ul>
                    <button type="submit" className="btn btn-dark border-0 rounded-0 mt-auto">Submit</button>
                </form>
                <div id="addCarousel" className="carousel slide w-50">
                    <div className="carousel-indicators">
                        {Children.toArray([...Array(this.state.keys.length).keys()].map(i =>
                            <button onClick={e => this.printForm(e)} className={(i === 0 && 'active') || ''} data-bs-target="#addCarousel" data-bs-slide-to={i} aria-current="true"></button>
                        ))}
                    </div>
                    <div className="carousel-inner h-75 overflow-y-auto">
                        {Children.toArray(this.state.keys.map(k => {
                            return {
                                'content': <div className={`carousel-item ${(this.state.keys.indexOf(k) === 0 && 'active') || ''}`}>
                                        <div className="d-flex">
                                            <button onClick={e => this.props.updateModal(e)} className="btn btn-outline-dark border-0 border-bottom rounded-0 mb-1 ms-auto text-white"
                                                    data-bs-toggle="modal" data-bs-target="#modalGen">
                                                Devices
                                            </button>
                                        </div>
                                        <textarea id={`${k}-input`} onChange={e => this.updateDatalist(e)} rows='20' className="form-control border-0 rounded-0 bg-white bg-opacity-75 text-dark" placeholder="Content" />
                                    </div>,
                                'role': <div className={`carousel-item ${(this.state.keys.indexOf(k) === 0 && 'active') || ''}`}>
                                        <select id={`${k}-input`} onChange={e => this.updateDatalist(e)} defaultValue="Role" className="form-control border-0 rounded-0">
                                            <option disabled hidden>Role</option>
                                            {Children.toArray(this.state.roles?.map(role =>
                                                <option value={role['name']}>{role['name']}</option>
                                            ))}
                                        </select>
                                    </div>
                                }[k]
                                || <div className={`carousel-item ${(this.state.keys.indexOf(k) === 0 && 'active') || ''}`}>
                                    <input type={((k.endsWith('Time') || k === 'doB') && 'datetime-local') || (['password', 'email'].includes(k) && k) || (k === 'photo' && 'file') || 'text'}
                                        id={`${k}-input`}
                                        onChange={e => this.updateDatalist(e)}
                                        placeholder={k.replace(/([A-Z]+)/g, ' $1').replace(/^./, k[0].toUpperCase())}
                                        className="form-control border-0 rounded-0" />
                                </div>
                        }))}
                    </div>
                    <button onClick={e => this.printForm(e)} className="carousel-control-prev mt-auto mb-3"
                            data-bs-target="#addCarousel" data-bs-slide="prev" style={{ height: 'fit-content' }}>
                        <span className="carousel-control-prev-icon"></span>
                        <span className="visually-hidden">Previous</span>
                    </button>
                    <button onClick={e => this.printForm(e)} className="carousel-control-next mt-auto mb-3"
                            data-bs-target="#addCarousel" data-bs-slide="next" style={{ height: 'fit-content' }}>
                        <span className="carousel-control-next-icon"></span>
                        <span className="visually-hidden">Next</span>
                    </button>
                </div>
            </div>
    }
}

const AddRouter = props => <Add {...props} />

const mapStateToProps = state => state

const mapDispatchToProps = dispatch => ({
    handleAdd: async (e, stateCopy) => {
        e.preventDefault()
        const addDictionary = {}
        await readForm(e, addDictionary)
        clearLi()
        await addRecord(stateCopy.entityName, addDictionary)
        const data = await getRecords(stateCopy.entityName, stateCopy.printBy, stateCopy.currentPage, stateCopy.globalOrder)
        data && dispatch(updateContent({...stateCopy, content: data.content}))
    }
    , updateModal: e => {
        e.preventDefault()
        dispatch(updateModal(e.target.innerHTML))
    }
})

export default connect(mapStateToProps, mapDispatchToProps)(AddRouter)

const readForm = async (e, addDictionary) => {
    const toSet = [...e.target.childNodes[0].childNodes]
    const slides = document.getElementById('addCarousel').childNodes[1].childNodes
    let buff
    for (let i = 0; i < toSet.length; i++) {
        if (toSet[i].childNodes[2]) {
            addDictionary[toSet[i].id] = ''
        }
        while (toSet[i].childElementCount > 2) {
            if (toSet[i].id === 'photo') {
                addDictionary['photo'] = Array.from(new Int8Array(await new Promise(resolve => {
                    const fr = new FileReader()
                    fr.onloadend = e => resolve(e.target.result)
                    fr.readAsArrayBuffer(document.querySelector('input[type=file]').files[0])
                })))
            } else if (toSet[i].id !== 'password' || toSet[i].childNodes[2].className === 'd-none') {
                addDictionary[toSet[i].id] += `${toSet[i].childNodes[2].innerHTML} `
            }
            toSet[i].childNodes[2].remove()
        }
        if (addDictionary[toSet[i].id] && toSet[i].id !== 'photo') {
            addDictionary[toSet[i].id] = addDictionary[toSet[i].id].trimEnd()
        }
        buff = slides[i].childNodes[0]
        buff.value = ''
        slides[i].innerHTML = ''
        slides[i].appendChild(buff)
        slides[i].className = 'carousel-item'
        slides[0].parentNode.previousSibling.childNodes[i].className = ''
    }
    slides[0].className += ' active'
    slides[0].parentNode.previousSibling.childNodes[0].className = 'active'
}

const clearLi = () => {
    const lis = document.getElementsByClassName('add-li')
    for (let i = 1; i < lis.length; i++) {
        lis[i].className = 'add-li d-none'
    }
}