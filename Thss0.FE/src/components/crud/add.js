import React, { Children } from 'react'
import { connect } from 'react-redux'
import { useNavigate, useParams } from 'react-router-dom'
import { updateContent } from '../../actionCreator/actionCreator'
import { addRecord, getRecords } from '../../services/entities'
import { Carousel } from 'bootstrap'
import Devices from '../devices'

class Add extends React.Component {
    constructor(props) {
        super(props)
        this.state = {
            keys: []
        }
    }
    async componentDidMount() {
        let data = await getRecords(this.props.params.entityName, this.props.order, this.props.printBy, this.props.currentPage)
        if (!(data && data.content[0])) {
            return
        }
        data = data.content[0]
        delete data['id']
        delete data['creationTime']
        delete data['department']
        delete data['result']
        delete data['user']
        delete data['procedure']
        delete data['substance']
        this.setState({ keys: Object.keys(data) })
        new Carousel('#add-carousel')
    }
    async updateDatalist(event) {
        event.preventDefault()
        const currentKey = event.target.id.replace(/.{6}$/, '')
        const datalist = event.target.parentNode
        let contentArr = []
        let optionToAdd = null
        this.props.updateContent({ ...this.props }, event)
        if (['departmentNames', 'userNames', 'procedure', 'resultNames', 'procedureNames'].includes(currentKey)
            && event.target.value.length > 3) {
            contentArr = (await getRecords(`search/${currentKey}/${encodeURIComponent(event.target.value)}`)).content
        }
        datalist.innerHTML = ''
        datalist.appendChild(event.target)
        event.target.focus()
        for (let index = -1; index < contentArr.length && event.target.value; index++) {
            optionToAdd = document.createElement('pre')
            optionToAdd.className = 'border bg-white p-2 mb-0'
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
            this.drag(optionToAdd, null, currentKey, 0, 0, document.getElementById('add-form'))
        }
    }
    drag(dataListOption, dragBuffer, currentKey, sourceX, sourceY, addForm) {
        dataListOption.onmousedown = () => {
            dataListOption.style.position = 'fixed'
            sourceX = dataListOption.style.left
            sourceY = dataListOption.style.top
            dragBuffer = dataListOption

            document.onmouseup = event => {
                document.onmousemove = null
                document.onmouseup = null
                if (event.pageX > addForm.offsetLeft && event.pageX < addForm.offsetLeft + addForm.offsetWidth
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
    componentDidUpdate() {
        (!this.props.content || this.props.content?.length > 1) && this.props.updateContent({ ...this.props, currentPage: 1 })
    }
    getInputType(key) {
        return ((key.endsWith('Time') || key === 'doB') && 'datetime-local') || (['password', 'email'].includes(key) && key) || 'text'
    }
    getValue(key) {
        if (this.props.content?.at(0) && this.props.content[0][key]?.length > 0 && !key.includes('Names')) {
            return this.props.content[0][key]
        }
    }
    render() {
        return (
            <div className="d-flex gap-1" style={{ background: 'url(../../img/pob.jpg)' }}>
                <div className="w-50">
                    <h5 className="d-flex ms-2">Add new {this.props.params.entityName.replace(/.$/, '')}
                        <button onClick={() => this.props.navigate(-1)} className="btn btn-outline-dark border-0 border-bottom rounded-0 col-2 ms-auto">Back</button>
                    </h5>
                    <form id="add-form" onSubmit={event => this.props.handleAdd(event, this.props.params.entityName, this.state.keys.length)}
                        className="d-flex flex-column" style={{height: 'calc(100vh - 45px)'}}>
                        <div id="details-error" className="alert alert-danger d-none"></div>
                        <ul className="list-unstyled ms-2">
                            {Children.toArray(this.state.keys.map(key =>
                                <li id={key} className={`keys ${this.state.keys.indexOf(key) > 0 && 'd-none'}`}>
                                    <span id={`${key}-error`} className="d-none"></span>
                                    <span>{key.replace(/([A-Z]+)/g, ' $1').replace(/^./, key[0].toUpperCase())}</span>
                                    {this.getValue(key)}
                                </li>
                            ))}
                        </ul>
                        <button type="submit" className="btn btn-dark border-0 rounded-0 mt-auto">Submit</button>
                    </form>
                </div>
                <div id="add-carousel" className="carousel slide w-50">
                    <div className="carousel-indicators">
                        {Children.toArray([...Array(this.state.keys.length).keys()].map(i =>
                            <button onClick={event => this.props.updateContent({ ...this.props, currentPage: i + 1 }, event)} type="button" data-bs-target="#add-carousel" data-bs-slide-to={i} className={(i === 0 && 'active') || ''} aria-current="true" aria-label={`Slide ${i}`}></button>
                        ))}
                    </div>
                    <div className="carousel-inner">
                        {Children.toArray(this.state.keys.map(key =>
                            <div className={`carousel-item ${(this.state.keys.indexOf(key) === 0 && 'active') || ''}`}>
                                {(key === 'content'
                                    && <>
                                        <div className="d-flex">
                                            <button type="button" data-bs-toggle="modal" data-bs-target="#devicesModal"
                                                className="btn btn-outline-dark border-0 border-bottom rounded-0 mb-1 ms-auto text-white">
                                                Find devices
                                            </button>
                                        </div>
                                        <textarea id={`${key}-input`} onChange={event => this.updateDatalist(event)}
                                            placeholder="Content" className="form-control border-0 rounded-0" />
                                    </>)
                                    || <input type={this.getInputType(key)} id={`${key}-input`}
                                        onChange={event => this.updateDatalist(event)}
                                        placeholder={key.replace(/([A-Z]+)/g, ' $1').replace(/^./, key[0].toUpperCase())}
                                        className="form-control border-0 rounded-0"
                                        autoFocus />
                                }
                            </div>
                        ))}
                    </div>
                    <button onClick={event => this.props.updateContent({ ...this.props, currentPage: (this.props.currentPage - 1) }, event)} className="carousel-control-prev mt-auto mb-3"
                        type="button" data-bs-target="#add-carousel" data-bs-slide="prev" style={{ height: 'fit-content' }}>
                        <span className="carousel-control-prev-icon" aria-hidden="true"></span>
                        <span className="visually-hidden">Previous</span>
                    </button>
                    <button onClick={event => this.props.updateContent({ ...this.props, currentPage: (this.props.currentPage + 1) }, event)} className="carousel-control-next mt-auto mb-3"
                        type="button" data-bs-target="#add-carousel" data-bs-slide="next" style={{ height: 'fit-content' }}>
                        <span className="carousel-control-next-icon" aria-hidden="true"></span>
                        <span className="visually-hidden">Next</span>
                    </button>
                </div>
                {this.props.params.entityName === 'results'
                    && <div className="modal fade" id="devicesModal" tabIndex="-1" aria-labelledby="devicesModalLabel" aria-hidden="true">
                        <div className="modal-dialog modal-dialog-centered">
                            <div className="modal-content rounded-0">
                                <div className="modal-header">
                                    <h1 className="modal-title fs-5" id="devicesModalLabel">Devices</h1>
                                    <button type="button" className="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                                </div>
                                <div className="modal-body" id="devices-body">{Devices(this.props)}</div>
                                <div className="modal-footer">
                                    <button type="button" className="btn btn-outline-dark col-2 border-0 border-bottom rounded-0" data-bs-dismiss="modal">Close</button>
                                </div>
                            </div>
                        </div>
                    </div>
                }
            </div>
        )
    }
}

const AddRouter = props => <Add {...props} params={useParams()} navigate={useNavigate()} />

const mapStateToProps = state => state

const mapDispatchToProps = dispatch => {
    return {
        handleAdd: async (event, entityName, totalPages) => {
            event.preventDefault()
            const addCredentials = [...event.target.childNodes[1].childNodes]
            const addDictionary = {}
            const addForm = document.getElementById('add-carousel')
            for (let i = 0; i < addCredentials.length; i++) {
                if (addCredentials[i].childNodes[2]) {
                    addDictionary[addCredentials[i].id] = ''
                }
                for (let j = 2; j < addCredentials[i].childNodes.length; j++) {
                    if (addCredentials[i].id !== 'password' || addCredentials[i].childNodes[j].className === 'd-none') {
                        addDictionary[addCredentials[i].id] += `${addCredentials[i].childNodes[j].innerHTML} `
                    }
                }
            }
            await addRecord(entityName, addDictionary)
            const data = await getRecords(entityName)
            data && dispatch(updateContent(data.content, 1, true, 1))
            addForm.childNodes[0].childNodes[totalPages - 1].className = ''
            addForm.childNodes[1].childNodes[totalPages - 1].className = 'carousel-item'
            addForm.childNodes[0].childNodes[0].className = 'active'
            addForm.childNodes[1].childNodes[0].className = 'carousel-item active'
            for (let index = 0; index < addForm.childNodes[1].childNodes.length; index++) {
                addForm.childNodes[1].childNodes[index].childNodes[0].value = ''// Was used to clear the Datalist. Perhaps no more useful.
            }
        }
        , updateContent: (stateCopy, event = null) => {
            event?.preventDefault()
            const lis = document.getElementsByClassName('keys')
            if (stateCopy.currentPage <= 0) {
                stateCopy.currentPage = 1
            }
            for (let index = 0; index < lis.length; index++) {
                lis[index].className = ((index < stateCopy.currentPage || lis[index].childNodes[2]) && 'keys d-block') || 'keys d-none'
            }
            if (!stateCopy.content || stateCopy.content?.length > 1) {
                stateCopy.content = []
            }
            // if (event && event.target.id && event.target.value !== '') {
            if (event?.target?.id) {
                stateCopy.content = [{ ...stateCopy.content[0] }]
                stateCopy.content[0][event.target.id] = event.target.value
            }
            dispatch(updateContent(stateCopy.content, stateCopy.totalPages, stateCopy.localOrder, stateCopy.currentPage))
        }
    }
}
export default connect(mapStateToProps, mapDispatchToProps)(AddRouter)