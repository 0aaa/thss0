import React from "react"
import { connect } from "react-redux"
import { useNavigate, useParams } from "react-router-dom"
import { updateState } from "../../actions/actions"
import { editRecord, getRecords } from "../../services/entity-service"

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
        delete content['creationTime']                  // For the Procedure type only.
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
                            {Object.keys(this.state.content).map(cntnt =>
                                <tr key={cntnt} className={`form-group ${cntnt === 'id' ? 'd-none' : ''}`}>
                                    <th>
                                        <label htmlFor={cntnt} className="col-form-label">{cntnt}</label>
                                    </th>
                                    <td>
                                        <span id={cntnt + '-error'} className="alert alert-danger d-none" />
                                        <input type={cntnt.endsWith('Time') ? 'datetime-local' : 'text'}
                                            id={cntnt}
                                            defaultValue={this.state.content[cntnt].length > 0 ? this.state.content[cntnt] : 'Empty'}
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

function mapStateToProps(state) {
    return { content: state.content }
}
function mapDispatchToProps(dispatch) {
    return {
        handleEdit: async (event, path, entityName, navigate) => {
            event.preventDefault()
            let formCollection = {};
            [...event.target.elements].forEach((cntrl) => formCollection[cntrl.id] = cntrl.value)
            delete formCollection['']
            await editRecord(path, formCollection)
            const data = getRecords(entityName)
            dispatch(updateState(data))
            navigate(-1)
        }
    }
}
export default connect(mapStateToProps, mapDispatchToProps)(EditRouter)