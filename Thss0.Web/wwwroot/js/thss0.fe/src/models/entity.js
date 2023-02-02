import Details from "./sub_models/details"

class Entity extends React.Component {
    constructor(props) {
        super(props)
        this.url = `/api/${props.match.params.entity_name}`
        this.state = {
            content: []
        }
    }
    async componentDidMount() {
        const RESPONSE = await fetch(this.url)
        const CONTENT = (await RESPONSE.json()).results
        this.setState({ CONTENT })
    }
    render() {
        return (
            <>
                <table className='table'>
                    <thead>
                    </thead>
                    <tbody>
                        {this.state.CONTENT.map(cntnt =>
                            <tr>
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
                            </tr>)
                        }
                    </tbody>
                </table>
                <Route path='/0/details/:href' component={Details} />
                <Route path="/0/edit/:href" component={Edit} />
                <Route path="/0/delete/:href" component={Delete} />
            </>
        )
    }
}
export default Entity