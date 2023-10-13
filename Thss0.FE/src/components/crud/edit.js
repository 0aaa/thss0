import React, { Children } from 'react'
import { connect } from 'react-redux'
import { updateContent } from '../../actionCreator/actionCreator'
import { editRecord, getRecords } from '../../services/entities'
import { Offcanvas } from 'bootstrap'

class Edit extends React.Component {
    constructor(props) {
        super(props)
        this.state = { keys: [] }
    }

    componentDidUpdate() {
        const keys = Object.keys(this.props.content[0]).filter(key => !['id', 'creationTime', 'department', 'result', 'user', 'procedure', 'substance'].includes(key))
        if (this.state.keys.length === 0 || this.state.keys.join() !== keys.join()) {
            this.setState({ keys })
        }
    }

    async updateDatalist(event) {
        event.preventDefault()
        const currentKey = event.target.id.replace(/.{11}$/, '')
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
            this.drag(optionToAdd, `${currentKey}-edit`)
        }
    }

    drag(dataListOption, currentKey) {
        let sourceX = 0
        let sourceY = 0
        let dragBuffer = null
        const editForm = document.getElementById('edit-form')
        dataListOption.onmousedown = () => {
            dataListOption.style.position = 'fixed'
            sourceX = dataListOption.style.left
            sourceY = dataListOption.style.top
            dragBuffer = dataListOption

            document.onmouseup = event => {
                document.onmousemove = null
                document.onmouseup = null
                if (event.pageX > editForm.offsetParent.offsetLeft && event.pageX < editForm.offsetParent.offsetLeft + editForm.offsetWidth
                    && event.pageY > editForm.offsetTop && event.pageY < editForm.offsetTop + editForm.offsetHeight) {

                    dragBuffer.style.position = 'static'
                    document.getElementById(currentKey).appendChild(dragBuffer)
                    if (currentKey === 'password-edit') {
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
            currentIndex = [...document.getElementsByClassName('carousel-item')].filter(slide => slide.parentNode.parentNode.id === 'edit-carousel').findIndex(i => i.className.includes('active'))
            if (event.target.className.includes('carousel-control-next')) {
                currentIndex = (currentIndex < this.state.keys.length - 1 && currentIndex + 1) || 0
            } else {
                currentIndex = (currentIndex === 0 && this.state.keys.length - 1) || currentIndex - 1
            }
        }
        const lis = document.getElementsByClassName('edit-li')
        for (let index = 0; index < lis.length; index++) {
            lis[index].className = ((index <= currentIndex || lis[index].childNodes[2]) && 'edit-li d-block') || 'edit-li d-none'
        }
    }

    render() {
        console.log(this.props.detailedItem)
        return <div className="offcanvas offcanvas-end w-50" data-bs-scroll="true" tabIndex="-1" id="offcanvasEdit" aria-labelledby="offcanvasEditLabel" style={{ background: 'url(../../img/pob.jpg)' }}>
            <div className="offcanvas-header">
                <h5 className="offcanvas-title" id="offcanvasEditLabel">Edit</h5>
                <button className="btn-close" data-bs-dismiss="offcanvas" aria-label="Close"></button>
            </div>
            <div className="d-flex gap-1 offcanvas-body">
                <form id="edit-form" onSubmit={event => this.props.handleEdit(event, {...this.props})}
                    className="d-flex flex-column h-100 w-50">
                    <ul className="list-unstyled ms-2">
                        {(this.props.detailedItem
                            && Children.toArray(this.state.keys.map(key =>
                                <li id={`${key}-edit`} className="edit-li">
                                    <span id={`${key}-error`} className="d-none"></span>
                                    <span>{key.replace(/([A-Z]+)/g, ' $1').replace(/^./, key[0].toUpperCase())}</span>
                                    {Children.toArray(this.props.detailedItem[key]?.split('\n').filter(name => name !== '').map(name =>
                                        <pre onMouseDown={event => this.drag(event.target, `${key}-edit`)} className="border bg-white p-2 mb-0 user-select-none">
                                            {name}
                                        </pre>
                                    ))}
                                </li>
                            )))
                            || <div className="h-75 d-flex justify-content-center align-items-center gap-1">
                                {Children.toArray([...Array(3).keys()].map(() =>
                                    <div className="spinner-grow text-primary" role="status">
                                        <span className="visually-hidden">Loading...</span>
                                    </div>
                                ))}
                            </div>
                        }
                    </ul>
                    <button type="submit" className="btn btn-dark border-0 rounded-0 mt-auto">Submit</button>
                </form>
                <div id="edit-carousel" className="carousel slide w-50">
                    <div className="carousel-indicators">
                        {Children.toArray([...Array(this.state.keys.length).keys()].map(i =>
                            <button onClick={event => this.printForm(event)} className={(i === 0 && 'active') || ''} data-bs-target="#edit-carousel" data-bs-slide-to={i} aria-current="true" aria-label={`Slide ${i}`}></button>
                        ))}
                    </div>
                    <div className="carousel-inner">
                        {Children.toArray(this.state.keys.map((key, i) =>
                            <div className={`carousel-item ${(i === 0 && 'active') || ''}`}>
                                <input type={((key.endsWith('Time') || key === 'doB') && 'datetime-local') || (['password', 'email'].includes(key) && key) || 'text'}
                                    id={`${key}-edit-input`}
                                    onChange={event => this.updateDatalist(event)}
                                    placeholder={key.replace(/([A-Z]+)/g, ' $1').replace(/^./, key[0].toUpperCase())}
                                    className="form-control border-0 rounded-0" />
                            </div>
                        ))}
                    </div>
                    <button onClick={event => this.printForm(event)} className="carousel-control-prev mt-auto mb-3"
                        data-bs-target="#edit-carousel" data-bs-slide="prev" style={{ height: 'fit-content' }}>
                        <span className="carousel-control-prev-icon" aria-hidden="true"></span>
                        <span className="visually-hidden">Previous</span>
                    </button>
                    <button onClick={event => this.printForm(event)} className="carousel-control-next mt-auto mb-3"
                        data-bs-target="#edit-carousel" data-bs-slide="next" style={{ height: 'fit-content' }}>
                        <span className="carousel-control-next-icon" aria-hidden="true"></span>
                        <span className="visually-hidden">Next</span>
                    </button>
                </div>
            </div>
        </div>
    }
}

const EditRouter = props => <Edit {...props} />

const mapStateToProps = state => {
    return {
        content: state.content
        , detailedItem: state.detailedItem
    }
}

const mapDispatchToProps = dispatch => {
    return {
        handleEdit: async (event, stateCopy) => {
            event.preventDefault()
            Offcanvas.getInstance('#offcanvasEdit').hide()
            const editCredentials = [...event.target.childNodes[0].childNodes]
            const editDictionary = {}
            let childsArrSize
            const carouselSlides = document.getElementById('edit-carousel').childNodes[1].childNodes
            let controlBuffer
            for (let index = 0; index < editCredentials.length; index++) {
                if (editCredentials[index].childNodes[2]) {
                    editDictionary[editCredentials[index].id] = ''
                }
                childsArrSize = editCredentials[index].childNodes.length
                for (let jndex = 2; jndex < childsArrSize; jndex++) {
                    if (editCredentials[index].id !== 'password' || editCredentials[index].childNodes[2].className === 'd-none') {
                        editDictionary[editCredentials[index].id] += `${editCredentials[index].childNodes[2].innerHTML} `
                    }
                    editCredentials[index].childNodes[2].remove()
                }
                if (editDictionary[editCredentials[index].id]) {                    
                    editDictionary[editCredentials[index].id] = editDictionary[editCredentials[index].id].trimEnd()
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

            // await editRecord(`${stateCopy.entityName}/${stateCopy.detailedItem['id']}`, editDictionary)
            const data = await getRecords(stateCopy.entityName)
            data && dispatch(updateContent({ ...stateCopy, content: data.content }))
        }
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(EditRouter)