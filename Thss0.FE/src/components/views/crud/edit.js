import React, { Children } from 'react'
import { connect } from 'react-redux'
import { updateContent } from '../../../actionCreator/actionCreator'
import { editRecord, getRecords } from '../../../services/entities'
import { Carousel, Offcanvas } from 'bootstrap'

class Edit extends React.Component {
    constructor(props) {
        super(props)
        this.state = { keys: [], roles: [] }
    }

    async componentDidMount() {
        //const keys = Object.keys(this.props.content[0]).filter(k => !['id', 'creationTime', 'department', 'result', 'user', 'procedure', 'substance'].includes(k))
        //if (this.state.keys.length === 0 || this.state.keys.join() !== keys.join()) {
        //    this.setState({ keys })
        //}
        document.getElementById('addCarousel') && new Carousel('#addCarousel')
        this.getKeys()
    }

    componentDidUpdate() {
        //const keys = Object.keys(this.props.content[0]).filter(k => !['id', 'creationTime', 'department', 'result', 'user', 'procedure', 'substance'].includes(k))
        //if (this.state.keys.length === 0 || this.state.keys.join() !== keys.join()) {
        //    this.setState({ keys })
        //}
        this.getKeys()
    }

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
        const currentKey = e.target.id.replace(/.{11}$/, '')
        const datalist = e.target.parentNode
        let contentArr = []
        let optionToAdd = null
        if (['departmentNames', 'userNames', 'procedure', 'resultNames', 'procedureNames'].includes(currentKey)
                && e.target.value.length > 3) {
            contentArr = (await getRecords(`search/${currentKey}/${encodeURIComponent(e.target.value)}`)).content
        }
        datalist.innerHTML = ''
        datalist.appendChild(e.target)
        e.target.focus()
        for (let i = -1; i < contentArr.length && e.target.value; i++) {
            optionToAdd = document.createElement('pre')
            optionToAdd.className = 'border bg-white p-2 mb-0 user-select-none bg-opacity-50'
            if (i > -1) {
                optionToAdd.innerHTML = contentArr[i].name ?? contentArr[i].userName ?? contentArr[i].content
            } else {
                if (currentKey.endsWith('Time') && e.target.value.match(/.{2}$/)[0] % 15) {
                    const timeToAdjust = new Date(e.target.value)
                    timeToAdjust.setMinutes(timeToAdjust.getMinutes() - timeToAdjust.getTimezoneOffset() + 15 - e.target.value.match(/.{2}$/)[0] % 15)
                    optionToAdd.innerHTML = timeToAdjust.toISOString().replace(/.{8}$/, '')
                } else {
                    optionToAdd.innerHTML = e.target.value
                }
            }
            datalist.appendChild(optionToAdd)
            this.drag(optionToAdd, `${currentKey}-edit`)
        }
    }

    drag(option, key) {
        let srcX = 0
        let srcY = 0
        let buff = null
        const form = document.getElementById('editForm')
        option.onmousedown = () => {
            option.style.position = 'fixed'
            srcX = option.style.left
            srcY = option.style.top
            buff = option
            buff.className = 'border bg-white p-2 mb-0 user-select-none w-25 bg-opacity-50'
            document.onmouseup = e => {
                document.onmousemove = null
                document.onmouseup = null
                const li = document.getElementById(key)
                const pres = li.getElementsByTagName('pre')
                if (e.pageX > form.offsetParent.offsetLeft && e.pageX < form.offsetParent.offsetLeft + form.offsetWidth
                        && e.pageY > form.offsetTop && e.pageY < form.offsetTop + form.offsetHeight) {

                    buff.style.position = 'static'
                    if (pres.length < 2) {
                        if (pres.length === 1) {
                            pres[pres.length - 1].className = 'd-none'
                        }
                        li.appendChild(buff)
                    } else {
                        li.replaceChild(buff, li.lastChild)
                    }
                    buff.className = 'border bg-white p-2 mb-0 user-select-none w-100 bg-opacity-50'
                    if (key === 'password-edit') {
                        const passwordClone = buff.cloneNode(true)
                        buff.className = 'd-none'
                        passwordClone.innerHTML = passwordClone.innerHTML.replace(/\S/g, '*')
                        document.getElementById(key).appendChild(passwordClone)
                    }
                } else if (buff) {
                    buff.style.position = 'static'
                    buff.style.left = srcX
                    buff.style.top = srcY
                    document.getElementById(`${key}-input`).parentNode.appendChild(buff)
                    buff.className = 'border bg-white p-2 mb-0 user-select-none w-100 bg-opacity-50'
                    if (pres.length > 0) {
                        pres[0].className = 'border bg-white p-2 mb-0 user-select-none bg-opacity-50'
                    }
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
        let currInd = 0
        if (e.target.className === 'active') {
            currInd = [...e.target.parentNode.childNodes].indexOf(e.target)
        } else {
            currInd = [...document.getElementsByClassName('carousel-item')].filter(sl => sl.parentNode.parentNode.id === 'editCarousel').findIndex(i => i.className.includes('active'))
            if (e.target.className.includes('carousel-control-next')) {
                currInd = (currInd < this.state.keys.length - 1 && currInd + 1) || 0
            } else {
                currInd = (currInd === 0 && this.state.keys.length - 1) || currInd - 1
            }
        }
        const lis = document.getElementsByClassName('edit-li')
        for (let i = 0; i < lis.length; i++) {
            //lis[i].className = ((i <= currInd || lis[i].childNodes[2]) && 'edit-li d-block') || 'edit-li d-none'
        }
    }

    render() {
        return <div className="d-flex gap-1 offcanvas-body">
            <form id="editForm" onSubmit={e => this.props.handleEdit(e, {...this.props})} className="d-flex flex-column h-100 w-50">
                <ul className="list-unstyled ms-2 overflow-y-auto">
                    {(this.props.detailedItem
                        && Children.toArray(this.state.keys.map(k =>
                            <li id={`${k}-edit`} className="edit-li">
                                <span id={`${k}Error`} className="d-none"></span>
                                <span>{k.replace(/([A-Z]+)/g, ' $1').replace(/^./, k[0].toUpperCase())}</span>
                                {(k !== 'photo' && Children.toArray(this.props.detailedItem[k]?.split('\n').filter(name => name !== '').map(name =>
                                    <pre className="border bg-white p-2 mb-0 user-select-none bg-opacity-50">{name}</pre>)))
                                || <>
                                    <img onMouseDown={e => this.drag(e.target, `${k}-edit`)} src={`data:image/jpeg;base64, ${btoa(String.fromCharCode.apply(null, new Uint8Array(this.props.detailedItem['photo'])))}`} alt="" className="w-100" />
                                    <input type="file"
                                        id={`${k}-edit-input`}
                                        onChange={e => this.updateDatalist(e)}
                                        className="form-control border-0 rounded-0" />
                                </>}
                            </li>)))
                    || <div className="h-75 d-flex justify-content-center align-items-center gap-1">
                        {Children.toArray([...Array(3).keys()].map(() =>
                            <div className="spinner-grow text-primary" role="status">
                                <span className="visually-hidden">Loading...</span>
                            </div>
                        ))}
                    </div>}
                </ul>
                <button type="submit" className="btn btn-dark border-0 rounded-0 mt-auto">Submit</button>
            </form>
            <div className="d-flex flex-column w-50">
                <div id="editCarousel" className="carousel slide h-100">
                    <div className="carousel-indicators">
                        {Children.toArray([...Array(this.state.keys.length).keys()].map(i =>
                            <button onClick={e => this.printForm(e)} className={(i === 0 && 'active') || ''} data-bs-target="#editCarousel" data-bs-slide-to={i} aria-current="true"></button>
                        ))}
                    </div>
                    <div className="carousel-inner">
                        {Children.toArray(this.state.keys.map((k, i) =>
                            k !== 'photo'
                            && <div className={`carousel-item ${(i === 0 && 'active') || ''}`}>
                                <input type={((k.endsWith('Time') || k === 'doB') && 'datetime-local') || (['password', 'email'].includes(k) && k) || 'text'}
                                    id={`${k}-edit-input`}
                                    onChange={e => this.updateDatalist(e)}
                                    placeholder={k.replace(/([A-Z]+)/g, ' $1').replace(/^./, k[0].toUpperCase())}
                                    className="form-control border-0 rounded-0" />
                            </div>
                        ))}
                    </div>
                    <button onClick={e => this.printForm(e)} className="carousel-control-prev mt-auto mb-3"
                            data-bs-target="#editCarousel" data-bs-slide="prev" style={{ height: 'fit-content' }}>
                        <span className="carousel-control-prev-icon" aria-hidden="true"></span>
                        <span className="visually-hidden">Previous</span>
                    </button>
                    <button onClick={e => this.printForm(e)} className="carousel-control-next mt-auto mb-3"
                            data-bs-target="#editCarousel" data-bs-slide="next" style={{ height: 'fit-content' }}>
                        <span className="carousel-control-next-icon" aria-hidden="true"></span>
                        <span className="visually-hidden">Next</span>
                    </button>
                </div>
                <button type="button" className="btn btn-dark border-0 rounded-0 mt-auto w-100" data-bs-dismiss="offcanvas">Cancel</button>
            </div>
        </div>
    }
}

const EditRouter = props => <Edit {...props} />

const mapStateToProps = state => ({
    content: state.content
    , detailedItem: state.detailedItem
})

const mapDispatchToProps = dispatch => ({
    handleEdit: async (e, stateCopy) => {
        e.preventDefault()
        Offcanvas.getInstance('#crud').hide()
        const toSet = [...e.target.childNodes[0].childNodes]
        const buffer = {}
        const slides = document.getElementById('editCarousel').childNodes[1].childNodes
        let childsArrSize
        let controlBuff
        for (let i = 0; i < toSet.length; i++) {
            if (toSet[i].childNodes[2]) {
                buffer[toSet[i].id] = ''
            }
            childsArrSize = toSet[i].childNodes.length
            // Test begin.
            console.log(toSet[i].id)
            if (toSet[i].id === 'photo-edit') {
                const fp = toSet[i].childNodes[2].innerHTML
                console.log(fp)
            }
            // Test end.
            for (let j = 2; j < childsArrSize; j++) {
                if (toSet[i].id !== 'password' || toSet[i].childNodes[2].className === 'd-none') {
                    buffer[toSet[i].id] += `${toSet[i].childNodes[2].innerHTML} `
                }
                toSet[i].childNodes[2].remove()
            }
            if (buffer[toSet[i].id]) {                    
                buffer[toSet[i].id] = buffer[toSet[i].id].trimEnd()
            }
            controlBuff = slides[i].childNodes[0]
            controlBuff.value = ''
            slides[i].innerHTML = ''
            slides[i].appendChild(controlBuff)
            slides[i].className = 'carousel-item'
            slides[0].parentNode.previousSibling.childNodes[i].className = ''
        }
        slides[0].className += ' active'
        slides[0].parentNode.previousSibling.childNodes[0].className = 'active'

        await editRecord(`${stateCopy.entityName}/${stateCopy.detailedItem['id']}`, buffer)
        const data = await getRecords(stateCopy.entityName)
        data && dispatch(updateContent({ ...stateCopy, content: data.content }))
    }    
})

export default connect(mapStateToProps, mapDispatchToProps)(EditRouter)