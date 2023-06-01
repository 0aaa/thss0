import React from 'react'
import { connect } from 'react-redux'
import { useNavigate, useParams } from 'react-router-dom'
import { updateContent } from '../../actionCreator/actionCreator'
import { addRecord, getRecords } from '../../services/entities'

class Add extends React.Component {
    constructor(props) {
        super(props)
        this.state = {
            keys: []
        }
    }
    async componentDidMount() {
        const data = (await getRecords(this.props.params.entityName, this.props.order, this.props.printBy, this.props.currentPage)).content[0]
        delete data['id']
        delete data['creationTime']
        delete data['department']
        delete data['result']
        delete data['user']
        delete data['procedure']
        this.setState({ keys: Object.keys(data) })
    }
    async updateDatalist(event) {
        event.preventDefault()
        const datalist = document.getElementById(event.target.id + '-list')
        datalist.innerHTML = ''
        if (['departmentName', 'userNames', 'procedure', 'substance'].includes(event.target.id)
                && event.target.value.length > 2) {
            const data = await getRecords(`search/${event.target.id}/${event.target.value}`)
            
            let optionToAdd = null
            for (let index = 0; index < data.content.length; index++) {
                optionToAdd = document.createElement('option')
                optionToAdd.value = data.content[index].name;
                datalist.appendChild(optionToAdd)
            }
        }
    }
    render() {
        return (
            <>
                <h5>Add new {this.props.params.entityName.replace(/.$/, '')}</h5>
                <form onSubmit={(event) => this.props.handleAdd(event, this.props.params.entityName)}
                    className="w-50">
                    <table className="table">
                        <tbody>
                            {this.state.keys.map(key =>
                                <tr key={key} className="form-group">
                                    <th>
                                        <label htmlFor={key} className="col-form-label">{key.replace(/([A-Z]+)/g, ' $1').replace(/^./, key[0].toUpperCase())}</label>
                                    </th>
                                    <td>
                                        <span id={key + '-error'} className="d-none"></span>
                                        <input type={key.endsWith('Time') ? 'datetime-local' : 'text'} id={key} list={key + '-list'}
                                            onChange={(event) => this.updateDatalist(event)}
                                            placeholder={key.replace(/([A-Z]+)/g, ' $1').replace(/^./, key[0].toUpperCase())} className="form-control" />
                                        <datalist id={key + '-list'}></datalist>
                                    </td>
                                </tr>
                            )}
                        </tbody>
                    </table>
                    <button onClick={(event) => this.props.updateContent({...this.props.state}, 'devices', event)}>Find devices</button>
                    <input type="submit" className="btn btn-outline-primary" />
                </form>
                <button onClick={() => this.props.navigate(-1)} className="btn btn-outline-primary">Back</button>
            </>
        )
    }
}
const AddRouter = (props) => <Add {...props} params={useParams()} navigate={useNavigate()} />

const mapStateToProps = (state) => { return state }

const mapDispatchToProps = (dispatch) => {
    return {
        handleAdd: async (event, entityName) => {
            event.preventDefault()
            let formCollection = {};
            [...event.target.elements].forEach(element => formCollection[element.id] = element.value)
            delete formCollection['']

            await addRecord(entityName, formCollection)
            const data = await getRecords(entityName)
            dispatch(updateContent(data.content))
        }


        , updateContent: async (stateCopy, path, event = null) => {// Test.
            event?.preventDefault()
            if (event?.target.name !== 'name-order') {                
                const data = await getRecords(path, stateCopy.order, stateCopy.printBy, stateCopy.currentPage)
                console.log(data)
                if (!data) {
                    return
                }
                stateCopy.content = data.content
                stateCopy.totalPages = Math.ceil(data.total_amount / stateCopy.printBy)
            }
            dispatch(updateContent(stateCopy.content, stateCopy.totalPages, stateCopy.localOrder))
        }
    }
}
export default connect(mapStateToProps, mapDispatchToProps)(AddRouter)