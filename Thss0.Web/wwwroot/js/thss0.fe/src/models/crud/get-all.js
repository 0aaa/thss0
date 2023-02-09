import React from 'react'
import { NavLink, Route, Routes, useParams } from 'react-router-dom'
import Get from './get'
import Add from './add'
import Edit from './edit'
import Delete from './delete'
import API_URL from '../../config/consts'

class GetAll extends React.Component {
    constructor(props) {
        super(props)
        this.url = API_URL + props.params.entityName
        this.state = {
            content: []
        }
    }
    async componentDidMount() {
        const response = await fetch(this.url)
        console.log(response)
        const content = (await response.json()).results
        this.setState({ content })
    }
    render() {
        return (
            <>
                <NavLink to={`/0/add/${this.props.params.entityName}`} className="btn btn-outline-primary">Add new</NavLink>
                <table className="table">
                    <thead>
                    </thead>
                    <tbody>
                        {this.state.content.map(cntnt =>
                            <tr id={cntnt.id}>
                                <td>
                                    <NavLink to={`/0/details/${encodeURIComponent(cntnt.url)}`}>
                                        {cntnt.name ?? cntnt.title}
                                    </NavLink>
                                </td>
                                <td>
                                    <NavLink to={`/0/edit/${encodeURIComponent(cntnt.url)}`}>
                                        Edit
                                    </NavLink>
                                </td>
                                <td>
                                    <NavLink to={`/0/delete/${encodeURIComponent(cntnt.url)}`}>
                                        Delete
                                    </NavLink>
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
                <Routes>
                    <Route path="/0/add/:entityName" component={Add} />
                    <Route path="/0/get/:href" component={Get} />
                    <Route path="/0/edit/:href" component={Edit} />
                    <Route path="/0/delete/:href" component={Delete} />
                </Routes>
            </>
        )
    }
}
function GetAllRouter(props) {
    return <GetAll {...props} params={useParams()} />
}
export default GetAllRouter