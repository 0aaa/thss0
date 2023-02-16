import React from 'react'
import { NavLink, useParams } from 'react-router-dom'
import API_URL from '../../config/consts'

class List extends React.Component {
    constructor(props) {
        super(props)
        this.url = API_URL + props.params.entityName
        this.state = {
            content: []
        }
    }
    async componentDidMount() {
        const response = await fetch(this.url)
        const content = await response.json()
        this.setState({ content })
    }
    render() {
        return (
            <>
                <NavLink to={`/add/${this.props.params.entityName}`} className="btn btn-outline-primary">Add new</NavLink>
                <table className="table">
                    <thead>
                    </thead>
                    <tbody>
                        {this.state.content.map(cntnt =>
                            <tr id={cntnt.id} key={cntnt.id}>
                                <td>
                                    <NavLink to={`/details/${this.props.params.entityName}/${cntnt.id}`}>
                                        {cntnt.name ?? cntnt.title}
                                    </NavLink>
                                </td>
                                <td>
                                    <NavLink to={`/edit/${this.props.params.entityName}/${cntnt.id}`}>
                                        Edit
                                    </NavLink>
                                </td>
                                <td>
                                    <NavLink to={`/delete/${this.props.params.entityName}/${cntnt.id}`}>
                                        Delete
                                    </NavLink>
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </>
        )
    }
}
function ListRouter(props) {
    return <List {...props} params={useParams()} />
}
export default ListRouter