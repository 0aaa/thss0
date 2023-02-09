import React from "react"
import editRecord from "../../services/entity-service"

export default class Edit extends React.Component {
    constructor(props) {
        super(props)
        this.url = decodeURIComponent(props.match.params.href)
        this.state = {
            content: []
        }
    }
    handleEdit(event) {
        event.preventDefault()
        editRecord(this.props.match.params.entityName, event, event)
    }
    async componentDidMount() {
        const response = await fetch(this.url)
        const content = await response.json()
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
                                <tr className="form-group">
                                    <span id={cntnt + '-error'} className="alert alert-danger d-none" />
                                    <th>
                                        <label for={cntnt} className="col-form-label">{cntnt}</label>
                                    </th>
                                    <td>
                                        <input id={cntnt} value={this.state.content[cntnt]} className="form-control" />
                                    </td>
                                </tr>
                            )}
                        </tbody>
                    </table>
                    <input type="submit" value="Submit" className="btn btn-outline-primary" />
                </form>
                <input onClick={this.props.history.goBack()} value="Back" className="btn btn-outline-primary" />
            </>
        )
    }
}