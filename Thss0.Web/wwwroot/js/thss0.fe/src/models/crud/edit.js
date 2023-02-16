import React from "react"
import { useNavigate, useParams } from "react-router-dom"
import API_URL from "../../config/consts"
import { editRecord } from "../../services/entity-service"

class Edit extends React.Component {
    constructor(props) {
        super(props)
        this.url = API_URL + props.params.entityName + '/' + props.params.id
        this.state = {
            content: []
        }
        this.handleEdit = this.handleEdit.bind(this)
    }
    handleEdit(event) {
        event.preventDefault()
        let formCollection = {};
        [...event.target.elements].forEach((cntrl) => formCollection[cntrl.id] = cntrl.value)
        delete formCollection['']
        editRecord(this.url, formCollection)
        this.props.navigate(-1)
    }
    async componentDidMount() {
        const response = await fetch(this.url)
        const content = await response.json()
        delete content['creationTime']                  // For the Procedure type only.
        this.setState({ content })
    }
    render() {
        return (
            <>
                <h5>Edit</h5>
                <form onSubmit={this.handleEdit} className="w-50">
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
function EditRouter(props) {
    return <Edit {...props} params={useParams()} navigate={useNavigate()} />
}
export default EditRouter