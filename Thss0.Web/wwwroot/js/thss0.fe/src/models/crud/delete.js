import React from "react"
import { useNavigate, useParams } from "react-router-dom"
import API_URL from "../../config/consts"
import { deleteRecord } from "../../services/entity-service"

class Delete extends React.Component {
    constructor(props) {
        super(props)
        this.url = API_URL + props.params.entityName + '/' + props.params.id
        this.state = {
            content: []
        }
        this.handleDelete = this.handleDelete.bind(this)
    }
    handleDelete(event) {
        event.preventDefault()
        deleteRecord(this.url)
        this.props.navigate(-1)
    }
    async componentDidMount() {
        const response = await fetch(this.url)
        const content = await response.json()
        this.setState({ content })
    }
    render() {
        return (
            <>
                <h5>Delete {this.state.content['name']}</h5>
                <div id="delete-error" className="alert alert-danger d-none"></div>
                <form onSubmit={this.handleDelete} className="w-50">
                    {Object.keys(this.state.content).map(cntnt =>
                        <dl key={cntnt}>
                            <dt>{cntnt}</dt>
                            <dd>{this.state.content[cntnt].length > 0 ? this.state.content[cntnt] : 'Empty'}</dd>
                        </dl>
                    )}
                    <input type="submit" value="Delete" className="btn btn-outline-danger" />
                </form>
                <button onClick={() => this.props.navigate(-1)} className="btn btn-outline-primary">Back</button>
            </>
        )
    }
}
function DeleteRouter(props) {
    return <Delete {...props} params={useParams()} navigate={useNavigate()} />
}
export default DeleteRouter