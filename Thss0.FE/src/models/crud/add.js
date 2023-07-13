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
        let data = (await getRecords(this.props.params.entityName, this.props.order, this.props.printBy, this.props.currentPage))
        if (!data || !data.content[0]) {
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
        new Carousel('#add-form')
    }
    async updateDatalist(event) {
        event.preventDefault()
        this.props.updateContent({...this.props}, event)
        const datalist = document.getElementById(`${event.target.id}-list`)
        if (['departmentNames', 'userNames', 'procedure', 'resultNames', 'procedureNames'].includes(event.target.id)
                && event.target.value.length > 3 && datalist) {
            const data = await getRecords(`search/${event.target.id}/${encodeURIComponent(event.target.value)}`)
            datalist.innerHTML = ''
            let optionToAdd = null
            for (let index = 0; index < data?.content.length; index++) {
                optionToAdd = document.createElement('option')
                optionToAdd.innerHTML = data.content[index].name ?? data.content[index].userName ?? data.content[index].content
                datalist.appendChild(optionToAdd)
            }
        }
    }
    componentDidUpdate() {
        if (!this.props.content || this.props.content?.length > 1) {
            // this.props.updateContent({ ...this.props, totalPages: this.state.keys.length, currentPage: 1 })
            this.props.updateContent({ ...this.props, currentPage: 1 })
        }
    }
    getInputType(key) {
        switch (key) {
            case 'password':
            case 'email':
                return key
            case 'doB':
                return 'datetime-local'
            default:
                return 'text'
        }
    }
    getDdInner(key) {
        if(this.props.content && this.props.content[0] && this.props.content[0][key]?.length > 0) {
            if(key.includes('Names')) {
                return Children.toArray(this.props.content[0][key].split('\n').filter(e => e !== '').map((_, i) =>
                    this.props.content[0][key].split('\n')[i]))
            }
            else if(key === 'password') {
                return this.props.content[0]['password'].replace(/\S/g, '*')
            }
            else {
                return this.props.content[0][key]
            }
        }
    }
    render() {
        return (
            <div className="d-flex gap-1">
                <div className="w-50">
                    <h5 className="d-flex">Add new {this.props.params.entityName.replace(/.$/, '')}
                        <button onClick={() => this.props.navigate(-1)} className="btn btn-outline-dark border-0 border-bottom rounded-0 col-2 ms-auto">Back</button>
                    </h5>
                    <div id="details-error" className="alert alert-danger d-none"></div>
                    {Children.toArray(this.state.keys.map(key =>
                        <>
                            <span id={`${key}-error`} className="d-none"></span>
                            <dl>

                                <dt className={this.state.keys.indexOf(key) > 0 ? 'd-none' : ''}>{key.replace(/([A-Z]+)/g, ' $1').replace(/^./, key[0].toUpperCase())}</dt>
                                <dd>
                                    {/* {this.props.content && this.props.content[0] && this.props.content[0][key]?.length > 0
                                        ? key.includes('Names')
                                            ? Children.toArray(this.props.content[0][key].split('\n').filter(e => e !== '').map((_, i) =>
                                                this.props.content[0][key].split('\n')[i]))
                                            : key === 'password'
                                                ? this.props.content[0]['password'].replace(/\S/g, '*')
                                                :  this.props.content[0][key]
                                        : ''
                                    } */}
                                    {this.getDdInner(key)}
                                </dd>
                            </dl>
                        </>
                    ))}
                </div>
                <form onSubmit={event => this.props.handleAdd(event, this.props.params.entityName, this.state.keys.length)}
                        className="w-50">
                    <div id="add-form" className="carousel slide">
                        <div className="carousel-indicators">
                            {Children.toArray([...Array(this.state.keys.length).keys()].map(i =>
                                <button onClick={event => this.props.updateContent({...this.props, currentPage: i + 1}, event)} type="button" data-bs-target="#add-form" data-bs-slide-to={i} className={i === 0 ? 'active' : ''} aria-current="true" aria-label={`Slide ${i}`}></button>
                            ))}
                        </div>
                        <div className="carousel-inner">
                            {Children.toArray(this.state.keys.map(key =>
                                <div className={`carousel-item ${this.state.keys.indexOf(key) === 0 ? 'active' : ''}`}>
                                    <img src={key.endsWith('Time') ? '../../img/dob.jpg' : `../../img/${key}.jpg`} className="position-relative start-50 translate-middle-x vh-100" alt={key} />
                                    <div className="card-img-overlay">
                                        {key === 'content'
                                        ? <>
                                        <div className="d-flex">
                                            <label htmlFor={key} className="col-form-label ps-2 text-white">Content</label>
                                            <button type="button" data-bs-toggle="modal" data-bs-target="#devicesModal"
                                                    className="btn btn-outline-dark border-0 border-bottom rounded-0 mb-1 ms-auto text-white">
                                                Find devices
                                            </button>
                                        </div>
                                            <textarea id="content" onChange={event => this.updateDatalist(event)}
                                                placeholder="Content" className="form-control border-0 rounded-0"/>
                                        </>
                                        : <>
                                            <label htmlFor={key} className="col-form-label ps-2">
                                                {key.replace(/([A-Z]+)/g, ' $1').replace(/^./, key[0].toUpperCase())}
                                            </label>
                                            {/* <input type={key.endsWith('Time') ? 'datetime-local' : 'text'} id={key} list={`${key}-list`} */}
                                            <input type={this.getInputType(key)} id={key} list={`${key}-list`}
                                                onChange={event => this.updateDatalist(event)}
                                                placeholder={key.replace(/([A-Z]+)/g, ' $1').replace(/^./, key[0].toUpperCase())}
                                                className="form-control border-0 rounded-0"
                                                autoFocus />
                                            <datalist id={`${key}-list`}></datalist>
                                        </>
                                        }
                                    </div>
                                </div>
                            ))}
                        </div>
                        <button onClick={event => this.props.updateContent({...this.props, currentPage: (this.props.currentPage - 1)}, event)} className="carousel-control-prev mt-auto mb-3"
                                type="button" data-bs-target="#add-form" data-bs-slide="prev" style={{height: 'fit-content'}}>
                            <span className="carousel-control-prev-icon" aria-hidden="true"></span>
                            <span className="visually-hidden">Previous</span>
                        </button>
                        {this.props.currentPage === this.state.keys.length
                            ? <button className="carousel-control-next mt-auto mb-3"
                                    type="submit" data-bs-target="#add-form" style={{height: 'fit-content'}}>
                                <span className="carousel-control-next-icon" aria-hidden="true"></span>
                                <span className="visually-hidden">Next</span>
                            </button>
                            : <button onClick={event => this.props.updateContent({...this.props, currentPage: (this.props.currentPage + 1)}, event)} className="carousel-control-next mt-auto mb-3"
                                    type="button" data-bs-target="#add-form" data-bs-slide="next" style={{height: 'fit-content'}}>
                                <span className="carousel-control-next-icon" aria-hidden="true"></span>
                                <span className="visually-hidden">Next</span>
                            </button>
                        }
                    </div>
                </form>
                {this.props.params.entityName === 'results'
                    && <div className="modal fade" id="devicesModal" tabIndex="-1" aria-labelledby="devicesModalLabel" aria-hidden="true">
                        <div className="modal-dialog">
                            <div className="modal-content">
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

const AddRouter = (props) => <Add {...props} params={useParams()} navigate={useNavigate()} />

const mapStateToProps = (state) => { return state }

const mapDispatchToProps = (dispatch) => {
    return {
        handleAdd: async (event, entityName, totalPages) => {
            event.preventDefault()
            let formCollection = {};
            [...event.target.elements].forEach(element => formCollection[element.id] = element.value)
            delete formCollection['']

            await addRecord(entityName, formCollection)
            const data = await getRecords(entityName)
            if (data) {
                dispatch(updateContent(data.content, 1, true, 1))
            }
            const addForm = document.getElementById('add-form')
            addForm.childNodes[0].childNodes[totalPages - 1].className = ''
            addForm.childNodes[1].childNodes[totalPages - 1].className = 'carousel-item'
            addForm.childNodes[0].childNodes[0].className = 'active'
            addForm.childNodes[1].childNodes[0].className = 'carousel-item active'
            for (let index = 0; index < addForm.childNodes[1].childNodes.length; index++) {
                addForm.childNodes[1].childNodes[index].childNodes[1].childNodes[1].value = ''
            }
        }
        , updateContent: (stateCopy, event = null) => {
            event?.preventDefault()
            if (stateCopy.currentPage <= 0) {
                stateCopy.currentPage = 1
            }
            // else if (stateCopy.currentPage > stateCopy.totalPages) {
            //     stateCopy.currentPage = stateCopy.totalPages
            // }
            const dls = document.getElementsByTagName('dl')
            for (let index = 0; index < dls.length; index++) {
                if (index < stateCopy.currentPage || dls[index].childNodes[1].innerHTML !== '') {
                    dls[index].childNodes[0].className = 'd-block'
                } else {
                    dls[index].childNodes[0].className = 'd-none'                    
                }
            }
            if (!stateCopy.content || stateCopy.content?.length > 1) {
                stateCopy.content = []
            }
            if (event && event.target.id && event.target.value !== '') {
                stateCopy.content = [{...stateCopy.content[0]}]
                stateCopy.content[0][event.target.id] = event.target.value
            }
            dispatch(updateContent(stateCopy.content, stateCopy.totalPages, stateCopy.localOrder, stateCopy.currentPage))
        }
    }
}
export default connect(mapStateToProps, mapDispatchToProps)(AddRouter)