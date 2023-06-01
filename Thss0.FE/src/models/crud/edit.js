import React from 'react'
import { connect } from 'react-redux'
import { useNavigate, useParams } from 'react-router-dom'
import { updateContent } from '../../actionCreator/actionCreator'
import { editRecord, getRecords } from '../../services/entities'

class Edit extends React.Component {
    constructor(props) {
        super(props)
        this.path = props.params.entityName + '/' + props.params.id
        this.state = {
            content: []
        }
    }
    async componentDidMount() {
        const content = await getRecords(this.path)
        delete content['creationTime']
        delete content['department']
        delete content['result']
        delete content['user']
        this.setState({ content })
    }
    render() {
        return (
            <>
                <h5>Edit</h5>
                <form onSubmit={(event) => this.props.handleEdit(event, this.path, this.props.params.entityName, this.props.navigate)}
                    className="w-50">
                    <table className="table">
                        <tbody>
                            {Object.keys(this.state.content).map(key =>
                                <tr key={key} className={`form-group ${key === 'id' ? 'd-none' : ''}`}>
                                    <th>
                                        <label htmlFor={key} className="col-form-label">{key.replace(/([A-Z]+)/g, ' $1').replace(/^./, key[0].toUpperCase())}</label>
                                    </th>
                                    <td>
                                        <span id={key + '-error'} className="alert alert-danger d-none" />
                                        <input type={key.endsWith('Time') ? 'datetime-local' : 'text'}
                                            id={key}
                                            defaultValue={this.state.content[key]}
                                            placeholder={key.replace(/([A-Z]+)/g, ' $1').replace(/^./, key[0].toUpperCase())}
                                            className="form-control" />
                                    </td>
                                </tr>
                            )}
                        </tbody>
                    </table>
                    <input type="submit" value="Submit" className="btn btn-outline-primary" />
                </form>
                <button onClick={() => this.props.navigate(-1)} className="btn btn-outline-primary">Back</button>
            </>
        )
    }
}
const EditRouter = (props) => <Edit {...props} params={useParams()} navigate={useNavigate()} />

const mapStateToProps = (state) => { return { content: state.content } }

const mapDispatchToProps = (dispatch) => {
    return {
        handleEdit: async (event, path, entityName, navigate) => {
            event.preventDefault()
            let formCollection = {};
            [...event.target.elements].forEach((element) => formCollection[element.id] = element.value)
            delete formCollection['']

            await editRecord(path, formCollection)
            const data = await getRecords(entityName)
            dispatch(updateContent(data.content))
            navigate(-1)
        }
    }
}
export default connect(mapStateToProps, mapDispatchToProps)(EditRouter)