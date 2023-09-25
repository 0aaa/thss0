import React, { Children } from 'react'
import { connect } from 'react-redux'
import { useNavigate, useParams } from 'react-router-dom'
import { updateContent } from '../../actionCreator/actionCreator'
import { editRecord, getRecords } from '../../services/entities'

class Edit extends React.Component {
    constructor(props) {
        super(props)
        this.path = `${props.params.entityName}/${props.params.id}`
        this.state = {
            content: []
        }
    }
    async componentDidMount() {
        const content = await getRecords(this.path)
        if (!content) {
            return
        }
        delete content['creationTime']
        delete content['department']
        delete content['result']
        delete content['user']
        this.setState({ content })
    }
    render() {
        return (
            <form onSubmit={event => this.props.handleEdit(event, this.path, this.props.params.entityName, this.props.navigate)}>
                <legend className="d-flex">Edit
                    <div className="btn-group w-25 ms-auto me-2">
                        <input type="submit" value="Submit" className="btn btn-outline-dark border-0 border-bottom rounded-0 col-6" />
                        <button type="button" onClick={() => this.props.navigate(-1)} className="btn btn-outline-dark border-0 border-bottom rounded-0 col-6">Back</button>
                    </div>
                </legend>
                <table className="table">
                    <tbody>
                        {Children.toArray(Object.keys(this.state.content).map(key =>
                            <tr className={`form-group ${(key === 'id' && 'd-none') || ''}`}>
                                <th className="p-0">
                                    <label htmlFor={key} className="col-form-label p-3">{key.replace(/([A-Z]+)/g, ' $1').replace(/^./, key[0].toUpperCase())}</label>
                                </th>
                                <td className="p-0">
                                    <span id={`${key}-error`} className="alert alert-danger d-none" />
                                    <input type={(key.endsWith('Time') && 'datetime-local') || 'text'}
                                        id={key}
                                        defaultValue={this.state.content[key]}
                                        placeholder={key.replace(/([A-Z]+)/g, ' $1').replace(/^./, key[0].toUpperCase())}
                                        className="form-control border-0 rounded-0 p-3" />
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </form>
        )
    }
}
const EditRouter = props => <Edit {...props} params={useParams()} navigate={useNavigate()} />

const mapStateToProps = state => { return { content: state.content } }

const mapDispatchToProps = dispatch => {
    return {
        handleEdit: async (event, path, entityName, navigate) => {
            event.preventDefault()
            let formCollection = {};
            [...event.target.elements].forEach(element => formCollection[element.id] = element.value)
            delete formCollection['']

            await editRecord(path, formCollection)
            const data = await getRecords(entityName)
            data && dispatch(updateContent(data.content))
            navigate(-1)
        }
    }
}
export default connect(mapStateToProps, mapDispatchToProps)(EditRouter)