import React, { Children } from 'react'
import { connect } from 'react-redux'
import { updateContent } from '../../actionCreator/actionCreator'
import { addRecord, getRecords } from '../../services/entities'
import { Carousel } from 'bootstrap'
import Devices from '../devices'

class Add extends React.Component {
    constructor(props) {
        super(props)
        this.state = { keys: [], roles: [] }
    }

    async componentDidMount() {
        document.getElementById('add-carousel') && new Carousel('#add-carousel')
    }

    componentDidUpdate() {
        const keys = Object.keys(this.props.content[0]).filter(key => !['id', 'creationTime', 'department', 'result', 'user', 'procedure', 'substance'].includes(key))
        if (this.state.keys.length === 0 || this.state.keys.join() !== keys.join()) {
            this.setState({ keys })
            this.state.roles.length === 0 && ['professional', 'client'].includes(this.props.entityName) && getRecords('roles').then(res => this.setState({roles: res.content}))
        }
    }

    async updateDatalist(event) {
        event.preventDefault()
        const currentKey = event.target.id.replace(/.{6}$/, '')
        const datalist = event.target.parentNode
        let contentArr = []
        let optionToAdd = null
        if (['departmentNames', 'userNames', 'procedure', 'resultNames', 'procedureNames'].includes(currentKey)
            && event.target.value.length > 3) {
            contentArr = (await getRecords(`search/${currentKey}/${encodeURIComponent(event.target.value)}`)).content
        }
        datalist.innerHTML = ''
        datalist.appendChild(event.target)
        event.target.focus()
        for (let index = -1; index < contentArr.length && event.target.value; index++) {
            optionToAdd = document.createElement('pre')
            optionToAdd.className = 'border bg-white p-2 mb-0 user-select-none'
            if (index > -1) {
                optionToAdd.innerHTML = contentArr[index].name ?? contentArr[index].userName ?? contentArr[index].content
            } else {
                if (currentKey.endsWith('Time') && event.target.value.match(/.{2}$/)[0] % 15) {
                    const timeToAdjust = new Date(event.target.value)
                    timeToAdjust.setMinutes(timeToAdjust.getMinutes() - timeToAdjust.getTimezoneOffset() + 15 - event.target.value.match(/.{2}$/)[0] % 15)
                    optionToAdd.innerHTML = timeToAdjust.toISOString().replace(/.{8}$/, '')
                } else {
                    optionToAdd.innerHTML = event.target.value
                }
            }
            datalist.appendChild(optionToAdd)
            this.drag(optionToAdd, currentKey)
        }
    }

    drag(dataListOption, currentKey) {
        let sourceX = 0
        let sourceY = 0
        let dragBuffer = null
        const addForm = document.getElementById('add-form')
        dataListOption.onmousedown = () => {
            dataListOption.style.position = 'fixed'
            sourceX = dataListOption.style.left
            sourceY = dataListOption.style.top
            dragBuffer = dataListOption

            document.onmouseup = event => {
                document.onmousemove = null
                document.onmouseup = null
                if (event.pageX > addForm.offsetParent.offsetLeft && event.pageX < addForm.offsetParent.offsetLeft + addForm.offsetWidth
                    && event.pageY > addForm.offsetTop && event.pageY < addForm.offsetTop + addForm.offsetHeight) {

                    dragBuffer.style.position = 'static'
                    document.getElementById(currentKey).appendChild(dragBuffer)
                    if (currentKey === 'password') {
                        const passwordClone = dragBuffer.cloneNode(true)
                        dragBuffer.className = 'd-none'
                        passwordClone.innerHTML = passwordClone.innerHTML.replace(/\S/g, '*')
                        document.getElementById(currentKey).appendChild(passwordClone)
                    }
                } else if (dragBuffer) {
                    dragBuffer.style.position = 'static'
                    dragBuffer.style.left = sourceX
                    dragBuffer.style.top = sourceY
                    document.getElementById(`${currentKey}-input`).parentNode.appendChild(dragBuffer)
                }
                dragBuffer = null
            }
            document.onmousemove = event => {
                if (dragBuffer) {
                    dragBuffer.style.left = `${event.pageX}px`
                    dragBuffer.style.top = `${event.clientY}px`
                }
            }
        }
    }

    printForm(event) {
        let currentIndex = 0
        if (event.target.className === 'active') {
            currentIndex = [...event.target.parentNode.childNodes].indexOf(event.target)
        } else {
            currentIndex = [...document.getElementsByClassName('carousel-item')].filter(slide => slide.parentNode.parentNode.id === 'add-carousel').findIndex(i => i.className.includes('active'))
            if (event.target.className.includes('carousel-control-next')) {
                currentIndex = (currentIndex < this.state.keys.length - 1 && currentIndex + 1) || 0
            } else {
                currentIndex = (currentIndex === 0 && this.state.keys.length - 1) || currentIndex - 1
            }
        }
        const lis = document.getElementsByClassName('add-li')
        for (let index = 0; index < lis.length; index++) {
            lis[index].className = ((index <= currentIndex || lis[index].childNodes[2]) && 'add-li d-block') || 'add-li d-none'
        }
    }

    render() {
        return this.state.keys.length
            && <div id="offcanvasAdd" tabIndex="-1" className="offcanvas offcanvas-end w-50" data-bs-scroll="true" aria-labelledby="offcanvasAddLabel" style={{ background: 'url(../../img/pob.jpg)' }}>
                <div className="offcanvas-header">
                    <h5 id="offcanvasAddLabel" className="d-flex ms-2 offcanvas-title">Add new {this.props.entityName.replace(/.$/, '')}</h5>
                    <button className="btn-close" data-bs-dismiss="offcanvas" aria-label="Close"></button>
                    <div id="details-error" className="alert alert-danger d-none"></div>
                </div>
                <div className="d-flex gap-1 offcanvas-body">
                    <form id="add-form" onSubmit={event => this.props.handleAdd(event, {...this.props})}
                            className="d-flex flex-column h-100 w-50">
                        <ul className="list-unstyled ms-2">
                            {Children.toArray(this.state.keys.map(key =>
                                <li id={key} className={`add-li ${this.state.keys.indexOf(key) > 0 && 'd-none'}`}>
                                    <span id={`${key}-error`} className="d-none"></span>
                                    <span>{key.replace(/([A-Z]+)/g, ' $1').replace(/^./, key[0].toUpperCase())}</span>
                                </li>
                            ))}
                        </ul>
                        <button type="submit" className="btn btn-dark border-0 rounded-0 mt-auto">Submit</button>
                    </form>
                    <div id="add-carousel" className="carousel slide w-50">
                        <div className="carousel-indicators">
                            {Children.toArray([...Array(this.state.keys.length).keys()].map(i =>
                                <button onClick={event => this.printForm(event)} className={(i === 0 && 'active') || ''} data-bs-target="#add-carousel" data-bs-slide-to={i} aria-current="true" aria-label={`Slide ${i}`}></button>
                            ))}
                        </div>
                        <div className="carousel-inner">
                            {Children.toArray(this.state.keys.map(key =>
                                {switch (key) {
                                    case 'content': 
                                        return <div className={`carousel-item ${(this.state.keys.indexOf(key) === 0 && 'active') || ''}`}>
                                            <div className="d-flex">
                                                <button data-bs-toggle="modal" data-bs-target="#devicesModal"
                                                        className="btn btn-outline-dark border-0 border-bottom rounded-0 mb-1 ms-auto text-white">
                                                    Find devices
                                                </button>
                                            </div>
                                            <textarea id={`${key}-input`} onChange={event => this.updateDatalist(event)}
                                                placeholder="Content" className="form-control border-0 rounded-0" />
                                        </div>
                                    case 'role':
                                        return <div className={`carousel-item ${(this.state.keys.indexOf(key) === 0 && 'active') || ''}`}>
                                            <select id={`${key}-input`} onChange={event => this.updateDatalist(event)} defaultValue="Role" className="form-control border-0 rounded-0">
                                                <option disabled hidden>Role</option>
                                                {Children.toArray(this.state.roles?.map(role =>
                                                    <option value={role['name']}>{role['name']}</option>
                                                ))}
                                            </select>
                                        </div>
                                    default:
                                        return <div className={`carousel-item ${(this.state.keys.indexOf(key) === 0 && 'active') || ''}`}>
                                            <input type={((key.endsWith('Time') || key === 'doB') && 'datetime-local') || (['password', 'email'].includes(key) && key) || 'text'}
                                                id={`${key}-input`}
                                                onChange={event => this.updateDatalist(event)}
                                                placeholder={key.replace(/([A-Z]+)/g, ' $1').replace(/^./, key[0].toUpperCase())}
                                                className="form-control border-0 rounded-0" />
                                        </div>
                                }}
                            ))}
                        </div>
                        <button onClick={event => this.printForm(event)} className="carousel-control-prev mt-auto mb-3"
                                data-bs-target="#add-carousel" data-bs-slide="prev" style={{ height: 'fit-content' }}>
                            <span className="carousel-control-prev-icon" aria-hidden="true"></span>
                            <span className="visually-hidden">Previous</span>
                        </button>
                        <button onClick={event => this.printForm(event)} className="carousel-control-next mt-auto mb-3"
                                data-bs-target="#add-carousel" data-bs-slide="next" style={{ height: 'fit-content' }}>
                            <span className="carousel-control-next-icon" aria-hidden="true"></span>
                            <span className="visually-hidden">Next</span>
                        </button>
                    </div>
                    {this.props.entityName === 'results'
                        && <div className="modal fade" id="devicesModal" tabIndex="-1" aria-labelledby="devicesModalLabel" aria-hidden="true">
                            <div className="modal-dialog modal-dialog-centered">
                                <div className="modal-content rounded-0">
                                    <div className="modal-header">
                                        <h1 id="devicesModalLabel" className="modal-title fs-5">Devices</h1>
                                        <button className="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                                    </div>
                                    <div id="devices-body" className="modal-body">{Devices(this.props)}</div>
                                    <div className="modal-footer">
                                        <button className="btn btn-outline-dark col-2 border-0 border-bottom rounded-0" data-bs-dismiss="modal">Close</button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    }
                </div>
            </div>
    }
}

const AddRouter = props => <Add {...props} />

const mapStateToProps = state => state

const mapDispatchToProps = dispatch => {
    return {
        handleAdd: async (event, stateCopy) => {
            event.preventDefault()
            const addCredentials = [...event.target.childNodes[0].childNodes]
            const addDictionary = {}
            let childsArrSize
            const carouselSlides = document.getElementById('add-carousel').childNodes[1].childNodes
            let controlBuffer
            for (let index = 0; index < addCredentials.length; index++) {
                if (addCredentials[index].childNodes[2]) {
                    addDictionary[addCredentials[index].id] = ''
                }
                childsArrSize = addCredentials[index].childNodes.length
                for (let jndex = 2; jndex < childsArrSize; jndex++) {
                    if (addCredentials[index].id !== 'password' || addCredentials[index].childNodes[2].className === 'd-none') {
                        addDictionary[addCredentials[index].id] += `${addCredentials[index].childNodes[2].innerHTML} `
                    }
                    addCredentials[index].childNodes[2].remove()
                }
                if (addDictionary[addCredentials[index].id]) {                    
                    addDictionary[addCredentials[index].id] = addDictionary[addCredentials[index].id].trimEnd()
                }
                controlBuffer = carouselSlides[index].childNodes[0]
                controlBuffer.value = ''
                carouselSlides[index].innerHTML = ''
                carouselSlides[index].appendChild(controlBuffer)
                carouselSlides[index].className = 'carousel-item'
                carouselSlides[0].parentNode.previousSibling.childNodes[index].className = ''
            }
            carouselSlides[0].className += ' active'
            carouselSlides[0].parentNode.previousSibling.childNodes[0].className = 'active'
            const lis = document.getElementsByClassName('add-li')
            for (let index = 1; index < lis.length; index++) {
                lis[index].className = 'add-li d-none'
            }
            await addRecord(stateCopy.entityName, addDictionary)
            const data = await getRecords(stateCopy.entityName)
            data && dispatch(updateContent({...stateCopy, content: data.content}))
        }
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(AddRouter)